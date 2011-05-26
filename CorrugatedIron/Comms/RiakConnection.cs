// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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
using System.Net.Sockets;
using System.Threading;
using CorrugatedIron.Config;
using CorrugatedIron.Encoding;

namespace CorrugatedIron.Comms
{
    public interface IRiakConnection : IDisposable
    {
        bool IsIdle { get; }
        void BeginIdle();
        void EndIdle();

        RiakResult<TResult> Read<TResult>()
            where TResult : new();
        RiakResult Write<TRequest>(TRequest request);
        RiakResult<TResult> WriteRead<TRequest, TResult>(TRequest request)
            where TResult : new();
    }

    public class RiakConnection : IRiakConnection
    {
        private readonly IRiakConnectionConfiguration _connectionConfiguration;
        private readonly MessageEncoder _encoder;
        private readonly object _idleTimerLock = new object();
        private TcpClient _client;
        private NetworkStream _clientStream;
        private Timer _idleTimer;

        public bool IsIdle
        {
            get { return _client == null; }
        }

        private TcpClient Client
        {
            get
            {
                return _client ??
                       (_client = new TcpClient(_connectionConfiguration.HostAddress, _connectionConfiguration.HostPort));
            }
        }

        private NetworkStream ClientStream
        {
            get { return _clientStream ?? (_clientStream = Client.GetStream()); }
        }

        public RiakConnection(IRiakConnectionConfiguration connectionConfiguration)
        {
            _connectionConfiguration = connectionConfiguration;
            _encoder = new MessageEncoder();
        }

        public static byte[] ToClientId(int id)
        {
            return BitConverter.GetBytes(id);
        }

        public void BeginIdle()
        {
            if (IsIdle) return;
            if (_idleTimer != null) return;

            lock (_idleTimerLock)
            {
                if (IsIdle) return;
                if (_idleTimer != null) return;

                _idleTimer = new Timer(_ => GoIdle(), null, 0, _connectionConfiguration.IdleTimeout);
            }
        }

        public void EndIdle()
        {
            CleanUpTimer();
        }

        public RiakResult<TResult> Read<TResult>()
            where TResult : new()
        {
            try
            {
                var result = _encoder.Decode<TResult>(ClientStream);
                ClientStream.Flush();
                return RiakResult<TResult>.Success(result);
            }
            catch (Exception ex)
            {
                return RiakResult<TResult>.Error(ResultCode.CommunicationError, ex.Message);
            }
        }

        public RiakResult Write<TRequest>(TRequest request)
        {
            try
            {
                _encoder.Encode(request, ClientStream);
                ClientStream.Flush();
                return RiakResult.Success();
            }
            catch (Exception ex)
            {
                return RiakResult.Error(ResultCode.CommunicationError, ex.Message);
            }
        }

        public RiakResult<TResult> WriteRead<TRequest, TResult>(TRequest request)
            where TResult : new()
        {
            var writeResult = Write(request);
            if (writeResult.IsSuccess)
            {
                return Read<TResult>();
            }
            return RiakResult<TResult>.Error(writeResult.ResultCode, writeResult.ErrorMessage);
        }

        public void Dispose()
        {
            CleanUp();
        }

        private void GoIdle()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            var client = _client;
            _client = null;
            var clientStream = _clientStream;
            _clientStream = null;

            if (clientStream != null)
            {
                clientStream.Dispose();
            }
            if (client != null)
            {
                client.Close();
            }
            CleanUpTimer();
        }

        private void CleanUpTimer()
        {
            if (_idleTimer == null) return;

            lock (_idleTimerLock)
            {
                // ignore R#'s warning, this IS possible across threads.
                if (_idleTimer == null) return;
                _idleTimer.Dispose();
                _idleTimer = null;
            }
        }
    }
}
