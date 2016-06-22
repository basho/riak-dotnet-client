// <copyright file="RiakPbcSocket.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

namespace RiakClient.Comms
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using Auth;
    using Commands;
    using Config;
    using Exceptions;
    using Extensions;
    using Messages;
    using ProtoBuf;

    internal class RiakPbcSocket : IDisposable
    {
        private const int SizeOfInt = sizeof(int);
        private const int PbMsgHeaderSize = SizeOfInt + sizeof(byte);

        private static readonly Type TtbType = typeof(TsTtbMsg);
        private readonly string server;
        private readonly int port;
        private readonly Timeout connectTimeout;
        private readonly Timeout readTimeout;
        private readonly Timeout writeTimeout;
        private readonly RiakSecurityManager securityManager;
        private readonly bool checkCertificateRevocation = false;

        private Stream networkStream = null;

        public RiakPbcSocket(IRiakNodeConfiguration nodeConfig, IRiakAuthenticationConfiguration authConfig)
        {
            server = nodeConfig.HostAddress;
            port = nodeConfig.PbcPort;
            readTimeout = nodeConfig.NetworkReadTimeout;
            writeTimeout = nodeConfig.NetworkWriteTimeout;
            connectTimeout = nodeConfig.NetworkConnectTimeout;
            securityManager = new RiakSecurityManager(server, authConfig);
            checkCertificateRevocation = authConfig.CheckCertificateRevocation;
        }

        private Stream NetworkStream
        {
            get
            {
                if (networkStream == null)
                {
                    SetUpNetworkStream();
                }

                return networkStream;
            }
        }

        public void Write(MessageCode messageCode)
        {
            var messageBody = new byte[PbMsgHeaderSize];

            var size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(1));
            Array.Copy(size, messageBody, SizeOfInt);
            messageBody[SizeOfInt] = (byte)messageCode;

            NetworkStream.Write(messageBody, 0, PbMsgHeaderSize);
        }

        public RiakResult Write(IRiakCommand command)
        {
            var ttbcmd = command as IRiakTtbCommand;
            if (ttbcmd == null)
            {
                RpbReq request = command.ConstructPbRequest();
                if (request.IsMessageCodeOnly)
                {
                    Write(request.MessageCode);
                    return RiakResult.Success();
                }
                else
                {
                    return DoWrite(s => Serializer.Serialize(s, request), request.GetType());
                }
            }
            else
            {
                return DoWrite(ttbcmd.GetSerializer(), TtbType);
            }
        }

        public void Write<T>(T message) where T : class
        {
            DoWrite(s => Serializer.Serialize(s, message), typeof(T));
        }

        public MessageCode Read(MessageCode expectedCode)
        {
            int size = 0;
            MessageCode messageCode = ReceiveHeader(out size);

            if (expectedCode != messageCode)
            {
                string errorMessage = string.Format("Expected return code {0} received {1}", expectedCode, messageCode);
                throw new RiakException(errorMessage, false);
            }

            return messageCode;
        }

        public RiakResult Read(IRiakCommand command)
        {
            bool done = false;
            var ttbcmd = command as IRiakTtbCommand;
            do
            {
                MessageCode expectedCode;
                if (ttbcmd == null)
                {
                    expectedCode = command.ExpectedCode;
                    Type expectedType = MessageCodeTypeMapBuilder.GetTypeFor(expectedCode);
                    int size = DoRead(expectedCode, expectedType);

                    RpbResp response = DeserializeInstance(expectedType, size);
                    command.OnSuccess(response);

                    var streamingResponse = response as IRpbStreamingResp;
                    if (streamingResponse == null)
                    {
                        done = true;
                    }
                    else
                    {
                        done = streamingResponse.done;
                    }
                }
                else
                {
                    expectedCode = MessageCode.TsTtbMsg;
                    int size = DoRead(expectedCode, TtbType);

                    byte[] resultBuffer = ReceiveAll(new byte[size - 1]);
                    RpbResp response = new TsTtbResp(resultBuffer);
                    ttbcmd.OnSuccess(response);

                    done = true;
                }
            }
            while (done == false);

            return RiakResult.Success();
        }

        public T Read<T>() where T : new()
        {
            Type expectedType = typeof(T);
            MessageCode expectedCode = MessageCodeTypeMapBuilder.GetMessageCodeFor(expectedType);

            int size = DoRead(expectedCode, expectedType);
            return DeserializeInstance<T>(size);
        }

        public void Dispose()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            // NB: since networkStream owns the underlying socket there is no need to close socket as well
            if (networkStream != null)
            {
                networkStream.Close();
                networkStream.Dispose();
                networkStream = null;
            }
        }

        private int DoRead(MessageCode expectedCode, Type expectedType)
        {
            int size = 0;
            MessageCode messageCode = ReceiveHeader(out size);

            if (!MessageCodeTypeMapBuilder.Contains(messageCode))
            {
                throw new RiakInvalidDataException((byte)messageCode);
            }

            if (expectedCode != messageCode)
            {
                string errorMessage = string.Format("Expected return code {0} received {1}", expectedCode, messageCode);
                throw new RiakException(errorMessage, false);
            }

            /*
             * Removed #if DEBUG because this seems like a good check
             * This message code validation is here to make sure that the caller
             * is getting exactly what they expect.
             * TODO: FUTURE - does this check impact performance?
             */
            Type typeForMessageCode = MessageCodeTypeMapBuilder.GetTypeFor(messageCode);
            if (typeForMessageCode != expectedType)
            {
                string receivedMessageCodeTypeName = MessageCodeTypeMapBuilder.GetTypeNameFor(messageCode);
                throw new InvalidOperationException(
                    string.Format(
                        "Attempt to decode message to type '{0}' when received type '{1}'.",
                        expectedType.Name,
                        receivedMessageCodeTypeName));
            }

            return size;
        }

        private MessageCode ReceiveHeader(out int size)
        {
            size = 0;

            byte[] header = ReceiveAll(new byte[5]);
            size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));
            MessageCode messageCode = (MessageCode)header[sizeof(int)];

            if (messageCode == MessageCode.RpbErrorResp)
            {
                var error = DeserializeInstance<RpbErrorResp>(size);
                string errorMessage = error.errmsg.FromRiakString();
                throw new RiakException((int)error.errcode, errorMessage, false);
            }

            return messageCode;
        }

        private RiakResult DoWrite(Action<MemoryStream> serializer, Type messageType)
        {
            byte[] messageBody;
            int messageLength = 0;

            using (var memStream = new MemoryStream())
            {
                // add a buffer to the start of the array to put the size and message code
                memStream.Position += PbMsgHeaderSize;
                serializer(memStream);
                messageBody = memStream.GetBuffer();
                messageLength = (int)memStream.Position;
            }

            // check to make sure something was written, otherwise we'll have to create a new array
            if (messageLength == PbMsgHeaderSize)
            {
                messageBody = new byte[PbMsgHeaderSize];
            }

            MessageCode messageCode = MessageCodeTypeMapBuilder.GetMessageCodeFor(messageType);
            var size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)messageLength - PbMsgHeaderSize + 1));
            Array.Copy(size, messageBody, SizeOfInt);
            messageBody[SizeOfInt] = (byte)messageCode;

            if (NetworkStream.CanWrite)
            {
                NetworkStream.Write(messageBody, 0, messageLength);
            }
            else
            {
                string errorMessage = string.Format("Failed to send data to server - Can't write: {0}:{1}", server, port);
                throw new RiakException(errorMessage, true);
            }

            return RiakResult.Success();
        }

        private void SetUpNetworkStream()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;

            // https://social.msdn.microsoft.com/Forums/en-US/313cf28c-2a6d-498e-8188-7a0639dbd552/tcpclientbeginconnect-issue
            IAsyncResult result = socket.BeginConnect(server, port, null, null);
            if (result.AsyncWaitHandle.WaitOne(connectTimeout))
            {
                socket.EndConnect(result);
            }
            else
            {
                socket.Close();
                string errorMessage = string.Format("Connection to remote server timed out: {0}:{1}", server, port);
                throw new RiakException(errorMessage, true);
            }

            if (!socket.Connected)
            {
                string errorMessage = string.Format("Unable to connect to remote server: {0}:{1}", server, port);
                throw new RiakException(errorMessage, true);
            }

            socket.ReceiveTimeout = (int)readTimeout;
            socket.SendTimeout = (int)writeTimeout;
            this.networkStream = new NetworkStream(socket, true);
            SetUpSslStream(this.networkStream);
        }

        private void SetUpSslStream(Stream networkStream)
        {
            if (!securityManager.IsSecurityEnabled)
            {
                return;
            }

            Write(MessageCode.RpbStartTls);

            // NB: the following will throw an exception if the returned code is not the expected code
            // TODO: FUTURE -> should throw a RiakSslException
            Read(MessageCode.RpbStartTls);

            // http://stackoverflow.com/questions/9934975/does-sslstream-dispose-disposes-its-inner-stream
            var sslStream = new SslStream(
                networkStream,
                false,
                securityManager.ServerCertificateValidationCallback,
                securityManager.ClientCertificateSelectionCallback);

            sslStream.ReadTimeout = (int)readTimeout;
            sslStream.WriteTimeout = (int)writeTimeout;

            if (securityManager.ClientCertificatesConfigured)
            {
                sslStream.AuthenticateAsClient(
                    targetHost: server,
                    clientCertificates: securityManager.ClientCertificates,
                    enabledSslProtocols: SslProtocols.Default,
                    checkCertificateRevocation: checkCertificateRevocation);
            }
            else
            {
                sslStream.AuthenticateAsClient(server);
            }

            // NB: very important! Must make the Stream being using going forward the SSL Stream!
            this.networkStream = sslStream;

            RpbAuthReq authRequest = securityManager.GetAuthRequest();
            Write(authRequest);
            Read(MessageCode.RpbAuthResp);
        }

        private RpbResp DeserializeInstance(Type respType, int size)
        {
            if (size <= 1)
            {
                return Activator.CreateInstance(respType) as RpbResp;
            }

            byte[] resultBuffer = ReceiveAll(new byte[size - 1]);

            using (var memStream = new MemoryStream(resultBuffer))
            {
                return Serializer.NonGeneric.Deserialize(respType, memStream) as RpbResp;
            }
        }

        private T DeserializeInstance<T>(int size) where T : new()
        {
            if (size <= 1)
            {
                return new T();
            }

            var resultBuffer = ReceiveAll(new byte[size - 1]);

            using (var memStream = new MemoryStream(resultBuffer))
            {
                return Serializer.Deserialize<T>(memStream);
            }
        }

        private byte[] ReceiveAll(byte[] resultBuffer)
        {
            int totalBytesReceived = 0;
            int lengthToReceive = resultBuffer.Length;

            if (!NetworkStream.CanRead)
            {
                string errorMessage = "Unable to read data from the source stream - Can't read.";
                throw new RiakException(errorMessage, true);
            }

            while (lengthToReceive > 0)
            {
                int bytesReceived = NetworkStream.Read(resultBuffer, totalBytesReceived, lengthToReceive);
                if (bytesReceived == 0)
                {
                    // NB: Based on the docs, this isn't necessarily an exception
                    // http://msdn.microsoft.com/en-us/library/system.net.sockets.networkstream.read(v=vs.110).aspx
                    break;
                }

                totalBytesReceived += bytesReceived;
                lengthToReceive -= bytesReceived;
            }

            return resultBuffer;
        }
    }
}