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
using System.Threading.Tasks;

namespace CorrugatedIron.Comms
{
    internal class RiakPbcSocket : IDisposable
    {
        private readonly string _server;
        private readonly int _port;
        private readonly int _receiveTimeout;
        private readonly int _sendTimeout;
        private static readonly Dictionary<MessageCode, Type> MessageCodeToTypeMap;
        private static readonly Dictionary<Type, MessageCode> TypeToMessageCodeMap;
        private Socket _pbcSocket;

        private Socket PbcSocket
        {
            get
            {
                if(_pbcSocket == null)
                {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.NoDelay = true;
                    socket.Connect(_server, _port);

                    if(!socket.Connected)
                    {
                        throw new RiakException("Unable to connect to remote server: {0}:{1}".Fmt(_server, _port));
                    }
                    socket.ReceiveTimeout = _receiveTimeout;
                    socket.SendTimeout = _sendTimeout;

                    _pbcSocket = socket;

                }
                return _pbcSocket;
            }
        }

        public bool IsConnected
        {
            get { return _pbcSocket != null && _pbcSocket.Connected; }
        }

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
                { MessageCode.SearchQueryResp, typeof(RpbSearchQueryResp) }
            };

            TypeToMessageCodeMap = new Dictionary<Type, MessageCode>();

            foreach(var item in MessageCodeToTypeMap)
            {
                TypeToMessageCodeMap.Add(item.Value, item.Key);
            }
        }

        public RiakPbcSocket(string server, int port, int receiveTimeout, int sendTimeout)
        {
            _server = server;
            _port = port;
            _receiveTimeout = receiveTimeout;
            _sendTimeout = sendTimeout;
        }

        public Task WriteAsync(MessageCode messageCode)
        {
            const int sizeSize = sizeof(int);
            const int codeSize = sizeof(byte);
            const int headerSize = sizeSize + codeSize;

            var messageBody = new byte[headerSize];

            var size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(1));
            Array.Copy(size, messageBody, sizeSize);
            messageBody[sizeSize] = (byte)messageCode;

            // send content
            return SocketWriteAsync(messageBody);
        }

        public Task WriteAsync<T>(T message) where T : class
        {
            const int sizeSize = sizeof(int);
            const int codeSize = sizeof(byte);
            const int headerSize = sizeSize + codeSize;

            // write to bytes
            byte[] data = null;
            using(var memStream = new MemoryStream())
            {
                // add a buffer to the start of the array to put the size and message code
                memStream.Position += headerSize;
                Serializer.Serialize(memStream, message);
                data = memStream.ToArray();
            }

            // fill header
            var messageCode = TypeToMessageCodeMap[typeof(T)];
            var sizeBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data.Length - headerSize + 1));
            Array.Copy(sizeBytes, data, sizeSize);
            data[sizeSize] = (byte)messageCode;

            // send content
            return SocketWriteAsync(data);
        }

        public Task<MessageCode> ReadAsync(MessageCode expectedCode)
        {
            return SocketReadAsync(5)
                .ContinueWith((Task<byte[]> readHeaderTask) => {
                    var header = readHeaderTask.Result;
                    var size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));
                    var messageCode = (MessageCode)header[sizeof(int)];
                    
                    if(messageCode == MessageCode.ErrorResp)
                    {
                        return DeserializeInstanceAsync<RpbErrorResp>(size)
                            .ContinueWith<MessageCode>((Task<RpbErrorResp> finishedTask) => {
                                var error = finishedTask.Result;
                                throw new RiakException(error.errcode, error.errmsg.FromRiakString(), false);
                            });
                    }
                    
                    if (expectedCode != messageCode)
                    {
                        throw new RiakException("Expected return code {0} received {1}".Fmt(expectedCode, messageCode));
                    }
                    
                    return TaskResult(messageCode);
                }).Unwrap();
        }

        public Task<T> ReadAsync<T>() where T : new()
        {
            return SocketReadAsync(5)
                .ContinueWith((Task<byte[]> readHeaderTask) => {
                    var header = readHeaderTask.Result;
                    var size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));  
                    var messageCode = (MessageCode)header[sizeof(int)];

                    if (messageCode == MessageCode.ErrorResp)
                    {
                        return DeserializeInstanceAsync<RpbErrorResp>(size)
                            .ContinueWith<T>((Task<RpbErrorResp> finishedTask) => {
                                var error = finishedTask.Result;
                                throw new RiakException(error.errcode, error.errmsg.FromRiakString(), false);
                            });
                    }
                    
                    if (!MessageCodeToTypeMap.ContainsKey(messageCode))
                    {
                        throw new RiakInvalidDataException((byte)messageCode);
                    }

#if DEBUG
                    // This message code validation is here to make sure that the caller
                    // is getting exactly what they expect. This "could" be removed from
                    // production code, but it's a good thing to have in here for dev.
                    if(MessageCodeToTypeMap[messageCode] != typeof(T))
                    {
                        throw new InvalidOperationException(string.Format("Attempt to decode message to type '{0}' when received type '{1}'.", typeof(T).Name, MessageCodeToTypeMap[messageCode].Name));
                    }
#endif

                    return DeserializeInstanceAsync<T>(size);
                }).Unwrap();
        }

        private Task<T> DeserializeInstanceAsync<T>(int size) where T : new()
        {
            if (size <= 1)
            {
                return TaskResult(new T());
            }

            return SocketReadAsync(size - 1)
                .ContinueWith((Task<byte[]> finishedTask) => {
                    using (var memStream = new MemoryStream(finishedTask.Result))
                    {
                        return Serializer.Deserialize<T>(memStream);
                    }
                });
        }

        // write to socket async
        private Task SocketWriteAsync(byte[] data)
        {
            var source = new TaskCompletionSource<object>();

            // start writing
            var maxBufferSize = 1024 * 4;
            var bytesToSend = data.Length;
            var position = 0;

            // write chunks as a continuation behaviour
            Action writeChunkAsync = null;
            writeChunkAsync = (() => {
                var startWrite = PbcSocket.BeginSend(data, position, Math.Min(bytesToSend, maxBufferSize),
                                                     SocketFlags.None, null, 0);
                Task<int>.Factory.FromAsync(startWrite, PbcSocket.EndSend)
                    .ContinueWith((Task<int> writeTask) => {
                        if (writeTask.IsFaulted)
                        {
                            source.SetException(writeTask.Exception);
                        }
                        else
                        {
                            if (writeTask.Result == 0)
                            {
                                source.SetException(
                                    new RiakException("Failed to send data to server - Timed Out: {0}:{1}"
                                                  .Fmt(_server, _port)));
                            }
                            else
                            {

                                // continue if necessary
                                position += writeTask.Result;
                                bytesToSend -= writeTask.Result;
                                if (bytesToSend > 0)
                                {
                                    writeChunkAsync();
                                }
                                else
                                {
                                    source.SetResult(new object());
                                }
                            }
                        }
                    });
            });

            // start writing and give back deferred task
            writeChunkAsync();
            return source.Task;
        }

        // read from socket async
        private Task<byte[]> SocketReadAsync(int size)
        {
            var source = new TaskCompletionSource<byte[]>();
            var data = new byte[size];

            // start reading
            PbcSocket.ReceiveTimeout = 0;
            int totalBytesReceived = 0;
            int lengthToReceive = size;

            // read in chunks as a continuation behaviour
            Action readChunkAsync = null;
            readChunkAsync = (() => {
                var startRead = PbcSocket.BeginReceive(data, totalBytesReceived, lengthToReceive,
                                                       SocketFlags.None, null, 0); 
                Task.Factory.FromAsync<int>(startRead, PbcSocket.EndReceive)
                    .ContinueWith((Task<int> readTask) => {
                        if (readTask.IsFaulted)
                        {
                            source.SetException(readTask.Exception);
                        }
                        else
                        {
                            if (readTask.Result == 0)
                            {
                                source.SetException(
                                    new RiakException("Unable to read data from the source stream - Timed Out."));
                            }
                            else
                            {

                                // continue if necessary
                                totalBytesReceived += readTask.Result;
                                lengthToReceive -= readTask.Result;
                                if (lengthToReceive > 0)
                                {
                                    readChunkAsync();
                                }
                                else
                                {
                                    source.SetResult(data);
                                }
                            }
                        }
                    });
            });

            // start reading and give back delayed buffer task
            readChunkAsync();
            return source.Task;
        }

        private Task<T> TaskResult<T>(T result)
        {
            var source = new TaskCompletionSource<T>();
            source.SetResult(result);
            return source.Task;
        }

        public void Disconnect()
        {
            if(_pbcSocket != null)
            {
                _pbcSocket.Disconnect(false);
                _pbcSocket.Dispose();
                _pbcSocket = null;
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
