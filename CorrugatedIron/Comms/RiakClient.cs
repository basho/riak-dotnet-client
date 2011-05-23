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

using System.Linq;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Util;

namespace CorrugatedIron.Comms
{
    public interface IRiakClient
    {
        RiakResult Ping();
        RiakResult SetClientId(string clientId);
        RiakResult<RiakObject> Get(string bucket, string key, uint rVal = Constants.Defaults.RVal);
        RiakResult<RiakObject> Put(RiakObject value, RiakPutOptions options = null);
    }

    public class RiakClient : IRiakClient
    {
        private readonly IRiakConnectionManager _connectionManager;

        public RiakClient(IRiakConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public RiakResult Ping()
        {
            return _connectionManager.UseConnection(conn => conn.WriteRead<RpbPingReq, RpbPingResp>(new RpbPingReq()));
        }

        public RiakResult SetClientId(string clientId)
        {
            return _connectionManager.UseConnection(conn => conn.WriteRead<RpbSetClientIdReq, RpbSetClientIdResp>(new RpbSetClientIdReq { ClientId = clientId.ToRiakString() }));
        }

        public RiakResult<RiakObject> Get(string bucket, string key, uint rVal = Constants.Defaults.RVal)
        {
            var request = new RpbGetReq { Bucket = bucket.ToRiakString(), Key = key.ToRiakString(), R = rVal };
            var result = _connectionManager.UseConnection(conn => conn.WriteRead<RpbGetReq, RpbGetResp>(request));

            if (!result.IsSuccess)
            {
                return RiakResult<RiakObject>.Error(result.ResultCode, result.ErrorMessage);
            }

            if (result.Value.VectorClock == null)
            {
                return RiakResult<RiakObject>.Error(ResultCode.NotFound);
            }
            return RiakResult<RiakObject>.Success(new RiakObject(bucket, key, result.Value.Content.First(), result.Value.VectorClock));
        }

        public RiakResult<RiakObject> Put(RiakObject value, RiakPutOptions options = null)
        {
            options = options ?? new RiakPutOptions();

            var request = value.ToMessage();
            options.Populate(request);

            var result = _connectionManager.UseConnection(conn => conn.WriteRead<RpbPutReq, RpbPutResp>(request));

            if (!result.IsSuccess)
            {
                return RiakResult<RiakObject>.Error(result.ResultCode, result.ErrorMessage);
            }

            return RiakResult<RiakObject>.Success(options.ReturnBody
                ? new RiakObject(value.Bucket, value.Key, result.Value.Content.First(), result.Value.VectorClock)
                : value);
        }
    }
}
