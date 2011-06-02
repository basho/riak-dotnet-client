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
using System.Collections.Generic;
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
        RiakResult<IEnumerable<string>> ListBuckets();
        RiakResult<IEnumerable<string>> ListKeys(string bucket);
        RiakResult<RiakBucketProperties> GetBucketProperties(string bucket);
        RiakResult SetBucketProperties(string bucket, RpbBucketProps properties);
    }

    public class RiakClient : IRiakClient
    {
        private readonly IRiakCluster _cluster;
        private byte[] _clientId;

        public RiakClient(IRiakCluster cluster)
            : this(cluster, GetClientId())
        {
        }

        public RiakClient(IRiakCluster cluster, int clientId)
            : this(cluster, RiakConnection.ToClientId(clientId))
        {
        }

        public RiakClient(IRiakCluster cluster, byte[] clientId)
        {
            _cluster = cluster;
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
  
        /// <summary>
        /// Ping this instance of Riak
        /// </summary>
        /// <returns>Returns true if the Riak instance has returned a 'pong' response. 
        /// Returns false if Riak is unavailable or returns a 'pang' response. </returns>
        public RiakResult Ping()
        {
            return _connectionManager.UseConnection(_clientId, 
                                                    conn => conn.WriteRead<RpbPingReq, RpbPingResp>(new RpbPingReq()), 
                                                    false);
        }
  
        /// <summary>
        /// Get the specified <paramref name="key"/> from the <paramref name="bucket"/>.
        /// Optionally can be read from <paramref name="rval"/> instances.
        /// </summary>
        /// <param name='bucket'>
        /// The name of the bucket containing the <paramref name="key"/>
        /// </param>
        /// <param name='key'>
        /// The key.
        /// </param>
        /// <param name='rVal'>
        /// The number of nodes required to successfully respond to the read before the read is considered a success.
        /// </param>
        /// <remarks>If a node does not respond, that does not necessarily mean that the 
        /// <paramref name="bucket"/>/<paramref name="key"/> combination is not available. It simply means
        /// that less than <paramref name="rVal"> nodes successfully responded to the read request. Unfortunatley, 
        /// the Riak API does not allow us to distinguish between a 404 resulting from less than <paramref name="rVal"/>
        /// nodes successfully responding and a <paramref name="bucket"/>/<paramref name="key"/> combination
        /// not being found in Riak.</remarks>
        public RiakResult<RiakObject> Get(string bucket, string key, uint rVal = Constants.Defaults.RVal)
        {
            var request = new RpbGetReq { Bucket = bucket.ToRiakString(), Key = key.ToRiakString(), R = rVal };
            var result = _cluster.UseConnection(_clientId, conn => conn.PbcWriteRead<RpbGetReq, RpbGetResp>(request));

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

            var result = _cluster.UseConnection(_clientId, conn => conn.PbcWriteRead<RpbPutReq, RpbPutResp>(request));

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
            var result = _cluster.UseConnection(_clientId, conn => conn.PbcWriteRead<RpbDelReq, RpbDelResp>(request));

            return result;
        }

        public RiakResult<RpbMapRedResp> MapReduce(string request, string requestType = Constants.ContentTypes.ApplicationJson)
        {
            var mapRedReq = new RpbMapRedReq { Request = request.ToRiakString(), ContentType = requestType.ToRiakString() };
            return MapReduce(mapRedReq);
        }

        public RiakResult<RpbMapRedResp> MapReduce(RpbMapRedReq request)
        {
            var result = _cluster.UseConnection(_clientId, conn => conn.PbcWriteRead<RpbMapRedReq, RpbMapRedResp>(request));
            return result;
        }
        
        public RiakResult<IEnumerable<string>> ListBuckets()
        {
            var lbReq = new RpbListBucketsReq();
            var result = _cluster.UseConnection(_clientId,
                                                          conn =>
                                                          conn.PbcWriteRead<RpbListBucketsReq, RpbListBucketsResp>(lbReq));

            if (result.IsSuccess)
            {
                var buckets = result.Value.Buckets.Select(b => b.FromRiakString());
                return RiakResult<IEnumerable<string>>.Success(buckets);
            }
            return RiakResult<IEnumerable<string>>.Error(result.ResultCode, result.ErrorMessage);
        }
  
        public RiakResult<IEnumerable<string>> ListKeys(string bucket)
        {
            var lkReq = new RpbListKeysReq {Bucket = bucket.ToRiakString()};
            var result = _cluster.UseConnection(_clientId,
                                                          conn => conn.PbcWriteRead<RpbListKeysReq, RpbListKeysResp>(lkReq));

            if (result.IsSuccess)
            {
                var keys = result.Value.Keys.Select(k => k.FromRiakString());
                return RiakResult<IEnumerable<string>>.Success(keys);
            }
            return RiakResult<IEnumerable<string>>.Error(result.ResultCode, result.ErrorMessage);
        }

        public RiakResult<RiakBucketProperties> GetBucketProperties(string bucket)
        {
            var bpReq = new RpbGetBucketReq {Bucket = bucket.ToRiakString()};
            var result = _cluster.UseConnection(_clientId,
                                                          conn => conn.PbcWriteRead<RpbGetBucketReq, RpbGetBucketResp>(bpReq));

            if (result.IsSuccess)
            {
                var props = new RiakBucketProperties(result.Value.Props);
                return RiakResult<RiakBucketProperties>.Success(props);
            }
            return RiakResult<RiakBucketProperties>.Error(result.ResultCode, result.ErrorMessage);
        }

        public RiakResult SetBucketProperties(string bucket, RpbBucketProps properties)
        {
            var bpReq = new RpbSetBucketReq {Bucket = bucket.ToRiakString(), Props = properties};
            var result = _cluster.UseConnection(_clientId,
                                                          conn =>
                                                          conn.PbcWriteRead<RpbSetBucketReq, RpbSetBucketResp>(bpReq));
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
