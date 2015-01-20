// Copyright (c) 2011 - 2014 OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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

using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using CorrugatedIron.Auth;
using CorrugatedIron.Config;
using CorrugatedIron.Exceptions;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using ProtoBuf;

namespace CorrugatedIron.Comms
{
    internal class RiakPbcSocket : IDisposable
    {
        private readonly string server;
        private readonly int port;
        private readonly int readTimeout;
        private readonly int writeTimeout;
        private readonly RiakSecurityManager securityManager;
        private readonly bool checkCertificateRevocation = false;

        private Stream networkStream = null;

        public RiakPbcSocket(IRiakNodeConfiguration nodeConfig, IRiakAuthenticationConfiguration authConfig)
        {
            server = nodeConfig.HostAddress;
            port = nodeConfig.PbcPort;
            readTimeout = nodeConfig.NetworkReadTimeout;
            writeTimeout = nodeConfig.NetworkWriteTimeout;
            securityManager = new RiakSecurityManager(server, authConfig);
            checkCertificateRevocation = authConfig.CheckCertificateRevocation;
        }

        /*
         * TODO: future
         * Read/Write in this class should *only* deal with byte[]
         * Serialization/Deserialization should be other class
         */
        public void Write(MessageCode messageCode)
        {
            const int sizeSize = sizeof(int);
            const int codeSize = sizeof(byte);
            const int headerSize = sizeSize + codeSize;

            var messageBody = new byte[headerSize];

            var size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(1));
            Array.Copy(size, messageBody, sizeSize);
            messageBody[sizeSize] = (byte)messageCode;

            NetworkStream.Write(messageBody, 0, headerSize);
        }

        public void Write<T>(T message) where T : class, ProtoBuf.IExtensible
        {
            const int sizeSize = sizeof(int);
            const int codeSize = sizeof(byte);
            const int headerSize = sizeSize + codeSize;
            byte[] messageBody;
            int messageLength = 0;

            using (var memStream = new MemoryStream())
            {
                // add a buffer to the start of the array to put the size and message code
                memStream.Position += headerSize;
                Serializer.Serialize(memStream, message);
                messageBody = memStream.GetBuffer();
                messageLength = (int)memStream.Position;
            }

            // check to make sure something was written, otherwise we'll have to create a new array
            if (messageLength == headerSize)
            {
                messageBody = new byte[headerSize];
            }

            byte messageCode = MessageCodeTypeMapBuilder.GetMessageCodeFor(typeof(T));
            var size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)messageLength - headerSize + 1));
            Array.Copy(size, messageBody, sizeSize);
            messageBody[sizeSize] = (byte)messageCode;

            if (NetworkStream.CanWrite)
            {
                NetworkStream.Write(messageBody, 0, messageLength);
            }
            else
            {
                string errorMessage = "Failed to send data to server - Can't write: {0}:{1}".Fmt(server, port);
                throw new RiakException(errorMessage, true);
            }
        }

        public MessageCode Read(MessageCode expectedCode)
        {
            var header = ReceiveAll(new byte[5]);
            var size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));
            var messageCode = (MessageCode)header[sizeof(int)];

            if (messageCode == MessageCode.RpbErrorResp)
            {
                var error = DeserializeInstance<RpbErrorResp>(size);
                string errorMessage = error.errmsg.FromRiakString();
                throw new RiakException(error.errcode, errorMessage, false);
            }

            if (expectedCode != messageCode)
            {
                string errorMessage = "Expected return code {0} received {1}".Fmt(expectedCode, messageCode);
                // TODO should this really mark the node offline?
                throw new RiakException(errorMessage, true);
            }

            return messageCode;
        }

        public T Read<T>() where T : ProtoBuf.IExtensible, new()
        {
            var header = ReceiveAll(new byte[5]);

            var size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));

            var messageCode = (MessageCode)header[sizeof(int)];
            if (messageCode == MessageCode.RpbErrorResp)
            {
                var error = DeserializeInstance<RpbErrorResp>(size);
                string errorMessage = error.errmsg.FromRiakString();
                throw new RiakException(error.errcode, errorMessage, false);
            }

            if (!MessageCodeTypeMapBuilder.Contains(messageCode))
            {
                throw new RiakInvalidDataException((byte)messageCode);
            }

            // TODO: removed #if DEBUG because this seems like a good check
            // This message code validation is here to make sure that the caller
            // is getting exactly what they expect. This "could" be removed from
            // production code, but it's a good thing to have in here for dev.
            var t_type = typeof(T);
            Type type_for_message_code = MessageCodeTypeMapBuilder.GetTypeFor(messageCode);
            if (type_for_message_code != t_type)
            {
                string received_message_code_type_name = MessageCodeTypeMapBuilder.GetTypeNameFor(messageCode);
                throw new InvalidOperationException(
                    String.Format("Attempt to decode message to type '{0}' when received type '{1}'.",
                    t_type.Name, received_message_code_type_name));
            }

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
            }
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

        private void SetUpNetworkStream()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.Connect(server, port);
            if (!socket.Connected)
            {
                string errorMessage = "Unable to connect to remote server: {0}:{1}".Fmt(server, port);
                throw new RiakException(errorMessage, true);
            }
            socket.ReceiveTimeout = readTimeout;
            socket.SendTimeout = writeTimeout;
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
            // TODO: should throw a RiakSslException
            Read(MessageCode.RpbStartTls);

            // TODO: validate_sessions from Ruby client
            // validates hostname, and CRL

            // http://stackoverflow.com/questions/9934975/does-sslstream-dispose-disposes-its-inner-stream
            var sslStream = new SslStream(networkStream, false,
                securityManager.ServerCertificateValidationCallback,
                securityManager.ClientCertificateSelectionCallback);

            sslStream.ReadTimeout = readTimeout;
            sslStream.WriteTimeout = writeTimeout;

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
                    // TODO: Based on the docs, this isn't necessarily an exception
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
