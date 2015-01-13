// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Exceptions;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Text;
using System.Diagnostics;

namespace CorrugatedIron.Comms
{
    internal class RiakPbcSocket : IDisposable
    {
        private static readonly byte[] emptyBytes = new byte[0];
        private static readonly EncryptionPolicy encryptionPolicy = EncryptionPolicy.RequireEncryption;
        // TODO private static readonly bool checkCertificateRevocation = true ;

        private readonly string server;
        private readonly int port;
        private readonly int receiveTimeout;
        private readonly int sendTimeout;
        private static readonly Dictionary<MessageCode, Type> MessageCodeToTypeMap;
        private static readonly Dictionary<Type, MessageCode> TypeToMessageCodeMap;

        private Stream networkStream = null;
        private bool isConnected = false;

        private Stream PbcStream
        {
            get
            {
                if (networkStream == null)
                {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.NoDelay = true;
                    socket.Connect(server, port);
                    if (socket.Connected)
                    {
                        isConnected = true;
                    }
                    else
                    {
                        throw new RiakException("Unable to connect to remote server: {0}:{1}".Fmt(server, port));
                    }
                    socket.ReceiveTimeout = receiveTimeout;
                    socket.SendTimeout = sendTimeout;
                    networkStream = new NetworkStream(socket, true);

                    Write(MessageCode.StartTls);
                    // TODO: error if the following is not read i.e. "expect_message"
                    Read(MessageCode.StartTls);

                    // TODO: validate_sessions
                    // validates hostname, and CRL

                    // TODO select client certs
                    // TODO private key file vs pfx?
                    // http://stackoverflow.com/questions/18462064/associate-a-private-key-with-the-x509certificate2-class-in-net
                    var cert = new X509Certificate2(@"C:\Users\lbakken\Projects\basho\CorrugatedIron\tools\test-ca\certs\riakuser-client-cert.pfx");
                    var clientCertificates = new X509CertificateCollection();
                    clientCertificates.Add(cert);
                     
                    //create the SSL stream starting from the NetworkStream associated
                    //with the TcpClient instance
                    var serverCertificateValidationCallback = new RemoteCertificateValidationCallback(ServerValidationCallback);
                    var clientCertificateSelectionCallback = new LocalCertificateSelectionCallback(ClientCertificateSelectionCallback);

                    // http://stackoverflow.com/questions/9934975/does-sslstream-dispose-disposes-its-inner-stream
                    var sslStream = new SslStream(networkStream, false,
                        serverCertificateValidationCallback,
                        clientCertificateSelectionCallback,
                        encryptionPolicy);

                    // TODO - only use this if client cert auth provided via configuration
                    string targetHost = "riak-test"; // TODO
                    var sslProtocol = SslProtocols.Tls;
                    sslStream.AuthenticateAsClient(targetHost, clientCertificates, sslProtocol, false);

                    networkStream = sslStream;
 
                    // TODO credentials stored elsewhere
                    var userBytes = Encoding.ASCII.GetBytes("riakuser");
                    var authRequest = new RpbAuthReq { user = userBytes, password = emptyBytes };
                    Write(authRequest);
                    // TODO: expect_message here
                    Read(MessageCode.AuthResp);
                }

                return networkStream;
            }
        }
        
        private X509Certificate ClientCertificateSelectionCallback(object sender, string targetHost,
            X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            if (localCertificates.Count > 0)
            {
                return localCertificates[0];
            }
            else
            {
                return new X509Certificate2(@"C:\Users\lbakken\Projects\basho\CorrugatedIron\tools\test-ca\certs\riakuser-client-cert.pfx");
            }
        }

        private bool ServerValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // TODO
            bool rv = sslPolicyErrors == SslPolicyErrors.None;
            Debug.WriteLine("SslPolicyErrors: {0}", sslPolicyErrors);
            return rv;
        }

        public bool IsConnected
        {
            get
            {
                // TODO: replace with something else?
                // http://stackoverflow.com/questions/1387459/how-to-check-if-tcpclient-connection-is-closed
                // http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.connected%28v=vs.110%29.aspx
                // TODO: every Read/Write to the stream should catch exceptions and set this to false
                return isConnected;
            }
        }

        public RiakPbcSocket(string server, int port, int receiveTimeout, int sendTimeout)
        {
            this.server = server;
            this.port = port;
            this.receiveTimeout = receiveTimeout;
            this.sendTimeout = sendTimeout;
        }

        public void Write(MessageCode messageCode)
        {
            const int sizeSize = sizeof(int);
            const int codeSize = sizeof(byte);
            const int headerSize = sizeSize + codeSize;

            var messageBody = new byte[headerSize];

            var size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(1));
            Array.Copy(size, messageBody, sizeSize);
            messageBody[sizeSize] = (byte)messageCode;

            PbcStream.Write(messageBody, 0, headerSize);
        }

        public void Write<T>(T message) where T : class, ProtoBuf.IExtensible
        {
            const int sizeSize = sizeof(int);
            const int codeSize = sizeof(byte);
            const int headerSize = sizeSize + codeSize;
            const int sendBufferSize = 1024 * 4;
            byte[] messageBody;
            long messageLength = 0;

            using (var memStream = new MemoryStream())
            {
                // add a buffer to the start of the array to put the size and message code
                memStream.Position += headerSize;
                Serializer.Serialize(memStream, message);
                messageBody = memStream.GetBuffer();
                messageLength = memStream.Position;
            }

            // check to make sure something was written, otherwise we'll have to create a new array
            if (messageLength == headerSize)
            {
                messageBody = new byte[headerSize];
            }

            var messageCode = TypeToMessageCodeMap[typeof(T)];
            var size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)messageLength - headerSize + 1));
            Array.Copy(size, messageBody, sizeSize);
            messageBody[sizeSize] = (byte)messageCode;

            int bytesToSend = (int)Math.Min(messageLength, sendBufferSize);
            if (PbcStream.CanWrite)
            {
                PbcStream.Write(messageBody, 0, bytesToSend);
            }
            else
            {
                throw new RiakException("Failed to send data to server - Can't write: {0}:{1}".Fmt(server, port));
            }
        }

        public MessageCode Read(MessageCode expectedCode)
        {
            var header = ReceiveAll(new byte[5]);
            var size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));
            var messageCode = (MessageCode)header[sizeof(int)];

            if (messageCode == MessageCode.ErrorResp)
            {
                var error = DeserializeInstance<RpbErrorResp>(size);
                throw new RiakException(error.errcode, error.errmsg.FromRiakString(), false);
            }

            if (expectedCode != messageCode)
            {
                throw new RiakException("Expected return code {0} received {1}".Fmt(expectedCode, messageCode));
            }

            return messageCode;
        }

        public T Read<T>() where T : new()
        {
            var header = ReceiveAll(new byte[5]);

            var size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));

            var messageCode = (MessageCode)header[sizeof(int)];
            if (messageCode == MessageCode.ErrorResp)
            {
                var error = DeserializeInstance<RpbErrorResp>(size);
                throw new RiakException(error.errcode, error.errmsg.FromRiakString());
            }

            if (!MessageCodeToTypeMap.ContainsKey(messageCode))
            {
                throw new RiakInvalidDataException((byte)messageCode);
            }

            // TODO: removed #if DEBUG because this seems like a good check
            // This message code validation is here to make sure that the caller
            // is getting exactly what they expect. This "could" be removed from
            // production code, but it's a good thing to have in here for dev.
            var t_type = typeof(T);
            if (MessageCodeToTypeMap[messageCode] != t_type)
            {
                throw new InvalidOperationException(
                    String.Format("Attempt to decode message to type '{0}' when received type '{1}'.",
                    t_type.Name, MessageCodeToTypeMap[messageCode].Name));
            }

            return DeserializeInstance<T>(size);
        }

        private T DeserializeInstance<T>(int size) where T : new()
        {
            if (size <= 1)
            {
                return new T();
            }

            var resultBuffer = ReceiveAll(new byte[size - 1]);

            using(var memStream = new MemoryStream(resultBuffer))
            {
                return Serializer.Deserialize<T>(memStream);
            }
        }

        private byte[] ReceiveAll(byte[] resultBuffer)
        {
            int totalBytesReceived = 0;
            int lengthToReceive = resultBuffer.Length;
            if (PbcStream.CanRead)
            {
                while (lengthToReceive > 0)
                {
                    int bytesReceived = PbcStream.Read(resultBuffer, totalBytesReceived, lengthToReceive);
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
            else
            {
                throw new RiakException("Unable to read data from the source stream - Can't read.");
            }
        }

        public void Disconnect()
        {
            if (networkStream != null)
            {
                networkStream.Close();
                networkStream.Dispose();
            }
            /*
             * TODO: since networkStream owns socket, this is most likely not necessary
            if (socket != null)
            {
                socket.Disconnect(false);
                socket.Dispose();
                socket = null;
            }
             */
        }

        public void Dispose()
        {
            Disconnect();
        }

        // TODO this should go somewhere else
        static RiakPbcSocket()
        {
            MessageCodeToTypeMap = new Dictionary<MessageCode, Type>
            {
                { MessageCode.ErrorResp, typeof(RpbErrorResp) },
                { MessageCode.GetClientIdResp, typeof(RpbGetClientIdResp) },
                { MessageCode.SetClientIdReq, typeof(RpbSetClientIdReq) },
                { MessageCode.GetServerInfoResp, typeof(RpbGetServerInfoResp) },
                { MessageCode.GetReq, typeof(RpbGetReq) },
                { MessageCode.GetResp, typeof(RpbGetResp) },
                { MessageCode.PutReq, typeof(RpbPutReq) },
                { MessageCode.PutResp, typeof(RpbPutResp) },
                { MessageCode.DelReq, typeof(RpbDelReq) },
                { MessageCode.ListBucketsReq, typeof(RpbListBucketsReq) },
                { MessageCode.ListBucketsResp, typeof(RpbListBucketsResp) },
                { MessageCode.ListKeysReq, typeof(RpbListKeysReq) },
                { MessageCode.ListKeysResp, typeof(RpbListKeysResp) },
                { MessageCode.GetBucketReq, typeof(RpbGetBucketReq) },
                { MessageCode.GetBucketResp, typeof(RpbGetBucketResp) },
                { MessageCode.SetBucketReq, typeof(RpbSetBucketReq) },
                { MessageCode.MapRedReq, typeof(RpbMapRedReq) },
                { MessageCode.MapRedResp, typeof(RpbMapRedResp) },
                { MessageCode.IndexReq, typeof(RpbIndexReq) },
                { MessageCode.IndexResp, typeof(RpbIndexResp) },
                { MessageCode.SearchQueryReq, typeof(RpbSearchQueryReq) },
                { MessageCode.SearchQueryResp, typeof(RpbSearchQueryResp) },
                { MessageCode.ResetBucketReq, typeof(RpbResetBucketReq) },
                { MessageCode.CsBucketReq, typeof(RpbCSBucketReq) },
                { MessageCode.CsBucketResp, typeof(RpbCSBucketResp) },
                { MessageCode.CounterUpdateReq, typeof(RpbCounterUpdateReq) },
                { MessageCode.CounterUpdateResp, typeof(RpbCounterUpdateResp) },
                { MessageCode.CounterGetReq, typeof(RpbCounterGetReq) },
                { MessageCode.CounterGetResp, typeof(RpbCounterGetResp) },

                { MessageCode.YokozunaIndexGetReq, typeof(RpbYokozunaIndexGetReq) },
                { MessageCode.YokozunaIndexGetResp , typeof(RpbYokozunaIndexGetResp) },
                { MessageCode.YokozunaIndexPutReq , typeof(RpbYokozunaIndexPutReq) },
                { MessageCode.YokozunaIndexDeleteReq , typeof(RpbYokozunaIndexDeleteReq) },
                { MessageCode.YokozunaSchemaGetReq , typeof(RpbYokozunaSchemaGetReq) },
                { MessageCode.YokozunaSchemaGetResp , typeof(RpbYokozunaSchemaGetResp) },
                { MessageCode.YokozunaSchemaPutReq , typeof(RpbYokozunaSchemaPutReq) },

                { MessageCode.DtFetchReq, typeof(DtFetchReq) },
                { MessageCode.DtFetchResp, typeof(DtFetchResp) },
                { MessageCode.DtUpdateReq, typeof(DtUpdateReq) },
                { MessageCode.DtUpdateResp, typeof(DtUpdateResp) },

                { MessageCode.GetBucketTypeReq, typeof(RpbGetBucketTypeReq) },
                { MessageCode.SetBucketTypeReq, typeof(RpbSetBucketTypeReq) },
                { MessageCode.AuthReq, typeof(RpbAuthReq) }
            };

            TypeToMessageCodeMap = new Dictionary<Type, MessageCode>();

            foreach(var item in MessageCodeToTypeMap)
            {
                TypeToMessageCodeMap.Add(item.Value, item.Key);
            }
        }
    }
}
