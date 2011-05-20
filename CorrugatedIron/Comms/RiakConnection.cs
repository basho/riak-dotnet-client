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
using System.Linq;
using System.Net.Sockets;
using CorrugatedIron.Config;
using CorrugatedIron.Encoding;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Util;

namespace CorrugatedIron.Comms
{
    public interface IRiakConnection : IDisposable
    {
        RiakResult Ping();
        RiakResult SetClientId(string clientId);
        RiakResult<RiakObject> Get(string bucket, string key, uint rVal = Constants.Defaults.RVal);
        RiakResult<RiakObject> Put(RiakObject value, RiakPutOptions options = null);
		RiakResult Delete(string bucket, string key, uint rwVal = Constants.Defaults.RVal);
        RiakResult<RpbMapRedResp> MapReduce(string request, string requestType = Constants.ContentTypes.ApplicationJson);
    }

    public class RiakConnection : IRiakConnection
    {
        private readonly IRiakConnectionConfiguration _connectionConfiguration;
        private readonly MessageEncoder _encoder;
        private TcpClient _client;
        private NetworkStream _clientStream;

        private TcpClient Client
        {
            get {
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

        public RiakResult Ping()
        {
            var result = WriteRead<RpbPingReq, RpbPingResp>(new RpbPingReq());
            return result;
        }

        public RiakResult SetClientId(string clientId)
        {
            var result = WriteRead<RpbSetClientIdReq, RpbSetClientIdResp>(new RpbSetClientIdReq { ClientId = clientId.ToRiakString() });
            return result;
        }

        public RiakResult<RiakObject> Get(string bucket, string key, uint rVal = Constants.Defaults.RVal)
        {
            var request = new RpbGetReq { Bucket = bucket.ToRiakString(), Key = key.ToRiakString(), R = rVal };
            var result = WriteRead<RpbGetReq, RpbGetResp>(request);
            if (result.IsError)
            {
                return RiakResult<RiakObject>.Error(result.ErrorMessage);
            }

            return RiakResult<RiakObject>.Success(new RiakObject(bucket, key, result.Value.Content.First(), result.Value.VectorClock));
        }

        public RiakResult<RiakObject> Put(RiakObject value, RiakPutOptions options = null)
        {
            options = options ?? new RiakPutOptions();

            var request = value.ToMessage();
            options.Populate(request);

            var result = WriteRead<RpbPutReq, RpbPutResp>(request);

            if (result.IsError)
            {
                return RiakResult<RiakObject>.Error(result.ErrorMessage);
            }

            return RiakResult<RiakObject>.Success(options.ReturnBody
                ? new RiakObject(value.Bucket, value.Key, result.Value.Content.First(), result.Value.VectorClock)
                : value);
        }
		
		public RiakResult Delete(string bucket, string key, uint rwVal = Constants.Defaults.RVal)
		{
			var request = new RpbDelReq { Bucket = bucket.ToRiakString(), Key = key.ToRiakString(), Rw = rwVal };
			var result = WriteRead<RpbDelReq, RpbDelResp>(request);
			
			return result;
		}
		
		public RiakResult<RpbMapRedResp> MapReduce(string request, string requestType = Constants.ContentTypes.ApplicationJson)
		{
			var mrrequest = new RpbMapRedReq { Request = request.ToRiakString(), ContentType = requestType.ToRiakString() };
			var result = WriteRead<RpbMapRedReq, RpbMapRedResp>(mrrequest);
			
			return result;
		}

        private RiakResult<TResult> Read<TResult>()
            where TResult : new()
        {
            try
            {
                var result = _encoder.Decode<TResult>(ClientStream);
                return RiakResult<TResult>.Success(result);
            }
            catch (Exception ex)
            {
                return RiakResult<TResult>.Error(ex.Message);
            }
        }

        private RiakResult Write<TRequest>(TRequest request)
        {
            try
            {
                _encoder.Encode(request, ClientStream);
                return RiakResult.Success();
            }
            catch (Exception ex)
            {
                return RiakResult.Error(ex.Message);
            }
        }

        private RiakResult<TResult> WriteRead<TRequest, TResult>(TRequest request)
            where TResult : new()
        {
            var writeResult = Write(request);
            if (!writeResult.IsError)
            {
                return Read<TResult>();
            }
            return RiakResult<TResult>.Error(writeResult.ErrorMessage);
        }

        public void Dispose()
        {
            if (_clientStream != null)
            {
                _clientStream.Dispose();
            }
            if (_client != null)
            {
                _client.Close();
            }
        }
    }
}
