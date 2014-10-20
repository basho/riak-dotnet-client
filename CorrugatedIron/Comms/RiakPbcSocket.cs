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

using System.IO;
using System.Threading.Tasks;
using CorrugatedIron.Comms.Sockets;
using CorrugatedIron.Exceptions;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace CorrugatedIron.Comms
{
    public class RiakPbcSocket : IDisposable
    {
        private readonly int _socketConnectAttempts;
        private readonly DnsEndPoint _endPoint;
        private readonly int _receiveTimeout;
        private readonly int _sendTimeout;
        private readonly TimeSpan _idleTimeout;
        private readonly int _receiveBufferSize;
        private readonly int _sendBufferSize;
        private readonly SocketAwaitablePool _socketAwaitablePool;
        private readonly BlockingBufferManager _blockingBufferManager;
        private static readonly Dictionary<MessageCode, Type> MessageCodeToTypeMap;
        private static readonly Dictionary<Type, MessageCode> TypeToMessageCodeMap;

        private Socket _socket;
        private DateTime _lastActivity;

        public RiakPbcSocket(string server, int port, int receiveTimeout, int sendTimeout, int idleTimeout,
            SocketAwaitablePool socketAwaitablePool, BlockingBufferManager blockingBufferManager,
            int receiveBufferSize = 8192, int sendBufferSize = 8192, int socketConnectAttempts = 3)
        {
            _endPoint = new DnsEndPoint(server, port);
            _receiveTimeout = receiveTimeout;
            _sendTimeout = sendTimeout;
            _idleTimeout = new TimeSpan(0, 0, 0, 0, idleTimeout);
            _socketAwaitablePool = socketAwaitablePool;
            _blockingBufferManager = blockingBufferManager;
            _receiveBufferSize = receiveBufferSize;
            _sendBufferSize = sendBufferSize;
            _socketConnectAttempts = socketConnectAttempts;
            _socket = null;
        }

        private async Task ConnectAsync(EndPoint endPoint)
        {
            var result = SocketError.Fault;
            for (var i = 0; i < _socketConnectAttempts; i++)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    UseOnlyOverlappedIO = true,
                    NoDelay = true,
                    ReceiveTimeout = _receiveTimeout,
                    SendTimeout = _sendTimeout,
                    ReceiveBufferSize = _receiveBufferSize,
                    SendBufferSize = _sendBufferSize
                };
                
                var awaitable = _socketAwaitablePool.Take();
                try
                {
                    awaitable.RemoteEndPoint = endPoint;

                    var socketAwaitable = _socket.ConnectAsync(awaitable);
                    _lastActivity = DateTime.UtcNow;

                    result = await socketAwaitable;

                    if (result == SocketError.Success)
                    {
                        break;
                    }
                }
                finally
                {
                    awaitable.Clear();
                    _socketAwaitablePool.Add(awaitable);
                }
            }

            if (result != SocketError.Success)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                throw new RiakException("Unable to connect to remote server: {0}:{1} error code {2}".Fmt(_endPoint.Host, _endPoint.Port, result));
            }
        }

        private async Task ReceiveAsync(ArraySegment<byte> buffer)
        {
            var awaitable = _socketAwaitablePool.Take();
            awaitable.Buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset, buffer.Count);

            try
            {
                while (true)
                {       
                    var result = await _socket.ReceiveAsync(awaitable);
                    _lastActivity = DateTime.UtcNow;

                    if (result != SocketError.Success)
                    {
                        throw new RiakException("Unable to read data from the source stream: {0}:{1} error code {2}"
                            .Fmt(_endPoint.Host, _endPoint.Port, result));
                    }

                    if (awaitable.Arguments.BytesTransferred == 0)
                    {
                        throw new RiakException("Unable to read data from the source stream: {0}:{1} remote server closed connection"
                            .Fmt(_endPoint.Host, _endPoint.Port));
                    }

                    if (awaitable.Arguments.Offset + awaitable.Arguments.BytesTransferred >= buffer.Offset + buffer.Count)
                    {
                        break;
                    }

                    awaitable.Buffer = new ArraySegment<byte>(
                        awaitable.Arguments.Buffer, 
                        awaitable.Arguments.Offset + awaitable.Arguments.BytesTransferred, 
                        awaitable.Arguments.Count - awaitable.Arguments.BytesTransferred);
                }
            }
            finally
            {
                awaitable.Clear();
                _socketAwaitablePool.Add(awaitable);
            }
        }

        private async Task SendAsync(ArraySegment<byte> buffer)
        {
            var awaitable = _socketAwaitablePool.Take();
            try
            {
                awaitable.Buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset, buffer.Count);

                while (true)
                {
                    var result = await _socket.SendAsync(awaitable);
                    _lastActivity = DateTime.UtcNow;

                    if (result != SocketError.Success)
                    {
                        throw new RiakException("Failed to send data to server - Timed Out: {0}:{1} error code {2}".Fmt(_endPoint.Host, _endPoint.Port, result));
                    }

                    if (awaitable.Arguments.BytesTransferred == 0)
                    {
                        throw new RiakException("Failed to send data to server - Timed Out: {0}:{1}".Fmt(_endPoint.Host, _endPoint.Port));
                    }

                    if (awaitable.Arguments.Offset + awaitable.Arguments.BytesTransferred >= buffer.Offset + buffer.Count)
                    {
                        break;
                    }

                    // Set the buffer to send the remaining data.
                    awaitable.Buffer = new ArraySegment<byte>(
                        awaitable.Arguments.Buffer,
                        awaitable.Arguments.Offset + awaitable.Arguments.BytesTransferred,
                        awaitable.Arguments.Count - awaitable.Arguments.BytesTransferred);
                }
            }
            finally
            {
                awaitable.Clear();
                _socketAwaitablePool.Add(awaitable);
            }
        }

        public bool HasSocket
        {
            get
            {
                return _socket != null;
            }
        }

        public bool IsConnected
        {
            get
            {
                return HasSocket 
                    && _socket.Connected
                    && _lastActivity != null
                    && DateTime.UtcNow.Subtract(_lastActivity) <= _idleTimeout ; //This will not detect half closed sockets
            }
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
                { MessageCode.CounterGetResp, typeof(RpbCounterGetResp) }
            };

            TypeToMessageCodeMap = new Dictionary<Type, MessageCode>();

            foreach(var item in MessageCodeToTypeMap)
            {
                TypeToMessageCodeMap.Add(item.Value, item.Key);
            }
        }

        private async Task<Socket> GetConnectedSocket()
        {
            if (IsConnected)
            {
                return _socket;
            }

            await ConnectAsync(_endPoint).ConfigureAwait(false);

            return _socket;
        }

        public async Task Write(MessageCode messageCode)
        {
            const int sizeSize = sizeof(int);
            const int codeSize = sizeof(byte);

            var buffer = _blockingBufferManager.GetBuffer();
            try
            {
                var size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(codeSize));

                var messageBody = new ArraySegment<byte>(
                    buffer.Array,
                    buffer.Offset,
                    sizeSize + codeSize);

                Array.Copy(size, 0, messageBody.Array, messageBody.Offset, sizeSize);
                messageBody.Array[messageBody.Offset + sizeSize] = (byte) messageCode;

                await GetConnectedSocket().ConfigureAwait(false);
                await SendAsync(messageBody).ConfigureAwait(false);
            }
            catch (RiakException)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                throw;
            }
            catch (SocketException)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                throw;
            }
            finally
            {
                _blockingBufferManager.ReleaseBuffer(buffer);
            }
        }

        public async Task Write<T>(T message) where T : class
        {
            const int sizeSize = sizeof(int);
            const int codeSize = sizeof(byte);
    
            var messageCode = TypeToMessageCodeMap[typeof(T)];

            if (message == null)
            {
                await Write(messageCode).ConfigureAwait(false);
                return;
            }

            var buffer = _blockingBufferManager.GetBuffer();
            try
            {
                using (var stream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, true))
                {
                    stream.Position = sizeSize + codeSize;

                    Serializer.Serialize(stream, message);
                    var messageLength = (int)stream.Position - sizeSize;

                    var size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(messageLength));
                    Array.Copy(size, 0, buffer.Array, buffer.Offset, sizeSize);
                    buffer.Array[buffer.Offset + sizeSize] = (byte)messageCode;

                    var messageBody = new ArraySegment<byte>(
                        buffer.Array,
                        buffer.Offset,
                        sizeSize + messageLength);

                    await GetConnectedSocket().ConfigureAwait(false);
                    await SendAsync(messageBody).ConfigureAwait(false);
                }
            }
            catch (RiakException)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                throw;
            }
            catch (SocketException)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                throw;
            }
            finally
            {
                _blockingBufferManager.ReleaseBuffer(buffer);
            }
        }

        public async Task<MessageCode> Read(MessageCode expectedCode)
        {
            const int sizeSize = sizeof(int);
            const int codeSize = sizeof(byte);

            var buffer = _blockingBufferManager.GetBuffer();
            try
            {
                await GetConnectedSocket().ConfigureAwait(false);

                var headerBuffer = new ArraySegment<byte>(buffer.Array, buffer.Offset, sizeSize+codeSize);

                await ReceiveAsync(headerBuffer).ConfigureAwait(false);

                var messageLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerBuffer.Array, headerBuffer.Offset));
                var messageCode = (MessageCode)headerBuffer.Array[headerBuffer.Offset + sizeSize];
                
                if (messageCode == MessageCode.ErrorResp)
                {
                    if (messageLength - codeSize  < 1)
                    {
                        var error = new RpbErrorResp();
                        throw new RiakException(error.errcode, error.errmsg.FromRiakString(), false);
                    }

                    var errorBuffer = new ArraySegment<byte>(headerBuffer.Array, headerBuffer.Offset + sizeSize + codeSize, messageLength - codeSize);

                    await ReceiveAsync(errorBuffer).ConfigureAwait(false);
                    
                    using (var stream = new MemoryStream(errorBuffer.Array, errorBuffer.Offset, errorBuffer.Count))
                    {
                        var error = Serializer.Deserialize<RpbErrorResp>(stream);
                        throw new RiakException(error.errcode, error.errmsg.FromRiakString(), false);
                    }
                }

                if (expectedCode != messageCode)
                {
                    throw new RiakException("Expected return code {0} received {1}".Fmt(expectedCode, messageCode));
                }

                return messageCode;
            }
            catch (RiakException)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                throw;
            }
            catch (SocketException)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                throw;
            }
            finally
            {
                _blockingBufferManager.ReleaseBuffer(buffer);
            }
        }

        public async Task<T> Read<T>() where T : new()
        {
            const int sizeSize = sizeof (int);
            const int codeSize = sizeof (byte);

            var buffer = _blockingBufferManager.GetBuffer();
            try
            {
                await GetConnectedSocket().ConfigureAwait(false);

                var headerBuffer = new ArraySegment<byte>(buffer.Array, buffer.Offset, sizeSize + codeSize);

                await ReceiveAsync(headerBuffer).ConfigureAwait(false);

                var messageLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerBuffer.Array, headerBuffer.Offset));
                var messageCode = (MessageCode)headerBuffer.Array[headerBuffer.Offset + sizeSize];

                if (messageCode == MessageCode.ErrorResp)
                {
                    if (messageLength - codeSize < 1)
                    {
                        var error = new RpbErrorResp();
                        throw new RiakException(error.errcode, error.errmsg.FromRiakString(), false);
                    }

                    if (messageLength > buffer.Count)
                    {
                        throw new RiakInvalidDataException(0);
                    }

                    var errorBuffer = new ArraySegment<byte>(headerBuffer.Array, headerBuffer.Offset + sizeSize + codeSize, messageLength - codeSize);

                    await ReceiveAsync(errorBuffer).ConfigureAwait(false);
 
                    using (var stream = new MemoryStream(errorBuffer.Array, errorBuffer.Offset, errorBuffer.Count))
                    {
                        var error = Serializer.Deserialize<RpbErrorResp>(stream);  
                        throw new RiakException(error.errcode, error.errmsg.FromRiakString(), false);
                    }
                }

                if (messageLength > buffer.Count)
                {
                    throw new RiakInvalidDataException(0);
                }

                if (!MessageCodeToTypeMap.ContainsKey(messageCode))
                {
                    throw new RiakInvalidDataException((byte) messageCode);
                }

                if (messageLength > buffer.Count)
                {
                    throw new RiakInvalidDataException(0);
                }
#if DEBUG
                // This message code validation is here to make sure that the caller
                // is getting exactly what they expect. This "could" be removed from
                // production code, but it's a good thing to have in here for dev.
                if (MessageCodeToTypeMap[messageCode] != typeof (T))
                {
                    throw new InvalidOperationException(
                        string.Format("Attempt to decode message to type '{0}' when received type '{1}'.",
                            typeof (T).Name, MessageCodeToTypeMap[messageCode].Name));
                }
#endif

                if (messageLength - codeSize <= 1)
                {
                    return new T();
                }

                var bodyBuffer = new ArraySegment<byte>(headerBuffer.Array, headerBuffer.Offset + sizeSize + codeSize, messageLength - codeSize);

                await ReceiveAsync(bodyBuffer).ConfigureAwait(false);
 
                using (var stream = new MemoryStream(bodyBuffer.Array, bodyBuffer.Offset, bodyBuffer.Count))
                {
                    var message = Serializer.Deserialize<T>(stream);
                    return message;
                }
            }
            catch (RiakException)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                throw;
            }
            catch (SocketException)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                throw;
            }
            finally
            {
                _blockingBufferManager.ReleaseBuffer(buffer);
            }
        }

        public async Task Disconnect()
        {
            if (_socket == null) return;

            var awaitable = _socketAwaitablePool.Take();
            try
            {
                await _socket.DisonnectAsync(awaitable);
            }
            finally
            {
                awaitable.Clear();
                _socketAwaitablePool.Add(awaitable);
            }

            if (_socket != null)
            {
                _socket.Dispose();
                _socket = null;
            }
        }

        public void Dispose()
        {
            Disconnect().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}