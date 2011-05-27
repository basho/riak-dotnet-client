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
using System.Net.NetworkInformation;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Util;

namespace CorrugatedIron.Comms
{
    public interface IRiakClient
    {
        RiakResult Ping();
        RiakResult<RiakObject> Get(string bucket, string key, uint rVal = Constants.Defaults.RVal);
        RiakResult<RiakObject> Put(RiakObject value, RiakPutOptions options = null);
        RiakResult Delete(string bucket, string key, uint rwVal = Constants.Defaults.RVal);
        RiakResult<RpbMapRedResp> MapReduce(string request, string requestType = Constants.ContentTypes.ApplicationJson);
        RiakResult<RpbMapRedResp> MapReduce(RpbMapRedReq request);
        RiakResult<RpbListBucketsResp> ListBuckets();
        RiakResult<RpbListKeysResp> ListKeys(string bucket);
    }

    public class RiakClient : IRiakClient
    {
        private readonly IRiakConnectionManager _connectionManager;
        private byte[] _clientId;

        public RiakClient(IRiakConnectionManager connectionManager)
            : this(connectionManager, GetClientId())
        {
        }

        public RiakClient(IRiakConnectionManager connectionManager, int clientId)
            : this(connectionManager, RiakConnection.ToClientId(clientId))
        {
        }

        public RiakClient(IRiakConnectionManager connectionManager, byte[] clientId)
        {
            _connectionManager = connectionManager;
            ClientId = clientId;
        }

        public byte[] ClientId
        {
            get { return _clientId; }
            set
            {
                System.Diagnostics.Debug.Assert(value != null && value.Length == Constants.ClientIdLength,
                    "Client ID must be exactly {0} bytes long.".Fmt(Constants.ClientIdLength));

                _clientId = value;
            }
        }

        public RiakResult Ping()
        {
            return _connectionManager.UseConnection(_clientId, conn => conn.WriteRead<RpbPingReq, RpbPingResp>(new RpbPingReq()), false);
        }

        public RiakResult<RiakObject> Get(string bucket, string key, uint rVal = Constants.Defaults.RVal)
        {
            var request = new RpbGetReq { Bucket = bucket.ToRiakString(), Key = key.ToRiakString(), R = rVal };
            var result = _connectionManager.UseConnection(_clientId, conn => conn.WriteRead<RpbGetReq, RpbGetResp>(request));

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

            var result = _connectionManager.UseConnection(_clientId, conn => conn.WriteRead<RpbPutReq, RpbPutResp>(request));

            if (!result.IsSuccess)
            {
                return RiakResult<RiakObject>.Error(result.ResultCode, result.ErrorMessage);
            }

            return RiakResult<RiakObject>.Success(options.ReturnBody
                ? new RiakObject(value.Bucket, value.Key, result.Value.Content.First(), result.Value.VectorClock)
                : value);
        }

        public RiakResult Delete(string bucket, string key, uint rwVal = Constants.Defaults.RVal)
        {
            var request = new RpbDelReq { Bucket = bucket.ToRiakString(), Key = key.ToRiakString(), Rw = rwVal };
            var result = _connectionManager.UseConnection(_clientId, conn => conn.WriteRead<RpbDelReq, RpbDelResp>(request));

            return result;
        }

        public RiakResult<RpbMapRedResp> MapReduce(string request, string requestType = Constants.ContentTypes.ApplicationJson)
        {
            var mapRedReq = new RpbMapRedReq { Request = request.ToRiakString(), ContentType = requestType.ToRiakString() };
            return MapReduce(mapRedReq);
        }

        public RiakResult<RpbMapRedResp> MapReduce(RpbMapRedReq request)
        {
            var result = _connectionManager.UseConnection(_clientId, conn => conn.WriteRead<RpbMapRedReq, RpbMapRedResp>(request));
            return result;
        }

        public RiakResult<RpbListBucketsResp> ListBuckets()
        {
            var lbReq = new RpbListBucketsReq();
            var result = _connectionManager.UseConnection(_clientId,
                                                          conn =>
                                                          conn.WriteRead<RpbListBucketsReq, RpbListBucketsResp>(lbReq));

            return result;
        }

        public RiakResult<RpbListKeysResp> ListKeys(string bucket)
        {
            var lkReq = new RpbListKeysReq {Bucket = bucket.ToRiakString()};
            var result = _connectionManager.UseConnection(_clientId,
                                                          conn => conn.WriteRead<RpbListKeysReq, RpbListKeysResp>(lkReq));
            return result;
        }

        private static byte[] GetClientId()
        {
            var nic = NetworkInterface.GetAllNetworkInterfaces().First(i => i.OperationalStatus == OperationalStatus.Up);
            var mac = nic.GetPhysicalAddress().GetAddressBytes();
            Array.Resize(ref mac, 4);
            return mac;
        }
    }
}
