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
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using CorrugatedIron.Comms;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Models.Rest;
using CorrugatedIron.Util;

namespace CorrugatedIron
{
    public interface IRiakClient
    {
        RiakResult Ping();
        void Ping(Action<RiakResult> callback);

        RiakResult<RiakObject> Get(string bucket, string key, uint rVal = RiakConstants.Defaults.RVal);

        void Get(string bucket, string key, Action<RiakResult<RiakObject>> callback,
                 uint rVal = RiakConstants.Defaults.RVal);

        RiakResult<RiakObject> Get(RiakObjectId objectId, uint rVal = RiakConstants.Defaults.RVal);
        void Get(RiakObjectId objectId, Action<RiakResult<RiakObject>> callback, uint rVal = RiakConstants.Defaults.RVal);

        IEnumerable<RiakResult<RiakObject>> Get(IEnumerable<RiakObjectId> bucketKeyPairs,
                                                uint rVal = RiakConstants.Defaults.RVal);

        RiakResult<RiakObject> Put(RiakObject value, RiakPutOptions options = null);
        void Put(RiakObject value, Action<RiakResult<RiakObject>> callback, RiakPutOptions options = null);
        IEnumerable<RiakResult<RiakObject>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null);

        void Put(IEnumerable<RiakObject> values, Action<IEnumerable<RiakResult<RiakObject>>> callback,
                 RiakPutOptions options = null);

        RiakResult Delete(string bucket, string key, uint rwVal = RiakConstants.Defaults.RVal);
        void Delete(string bucket, string key, Action<RiakResult> callback, uint rwVal = RiakConstants.Defaults.RVal);
        RiakResult Delete(RiakObjectId objectId, uint rwVal = RiakConstants.Defaults.RVal);
        void Delete(RiakObjectId objectId, Action<RiakResult> callback, uint rwVal = RiakConstants.Defaults.RVal);
        IEnumerable<RiakResult> Delete(IEnumerable<RiakObjectId> objectIds, uint rwVal = RiakConstants.Defaults.RVal);

        void Delete(IEnumerable<RiakObjectId> objectIds, Action<IEnumerable<RiakResult>> callback,
                    uint rwVal = RiakConstants.Defaults.RVal);

        IEnumerable<RiakResult> DeleteBucket(string bucket, uint rwVal = RiakConstants.Defaults.RVal);

        void DeleteBucket(string bucket, Action<IEnumerable<RiakResult>> callback,
                          uint rwVal = RiakConstants.Defaults.RVal);

        RiakResult<RiakMapReduceResult> MapReduce(RiakMapReduceQuery query);
        void MapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakMapReduceResult>> callback);

        RiakResult<RiakStreamedMapReduceResult> StreamMapReduce(RiakMapReduceQuery query);
        void StreamMapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakStreamedMapReduceResult>> callback);

        RiakResult<IEnumerable<string>> ListBuckets();
        void ListBuckets(Action<RiakResult<IEnumerable<string>>> callback);

        RiakResult<IEnumerable<string>> ListKeys(string bucket);
        void ListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback);

        RiakResult<IEnumerable<string>> StreamListKeys(string bucket);
        void StreamListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback);

        RiakResult<RiakBucketProperties> GetBucketProperties(string bucket, bool extended = false);
        void GetBucketProperties(string bucket, Action<RiakResult<RiakBucketProperties>> callback, bool extended = false);

        RiakResult SetBucketProperties(string bucket, RiakBucketProperties properties);
        void SetBucketProperties(string bucket, RiakBucketProperties properties, Action<RiakResult> callback);

        IList<RiakObject> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks);

        RiakResult<RiakServerInfo> GetServerInfo();
        void GetServerInfo(Action<RiakResult<RiakServerInfo>> callback);
    }

    public class RiakClient : IRiakClient
    {
        private readonly IRiakCluster _cluster;
        private byte[] _clientId;

        public RiakClient(IRiakCluster cluster)
        {
            _cluster = cluster;
            ClientId = GetClientId();
        }

        public byte[] ClientId
        {
            get { return _clientId; }
            set
            {
                if (value == null || value.Length != RiakConstants.ClientIdLength)
                {
                    throw new ArgumentException(
                        "Client ID must be exactly {0} bytes long.".Fmt(RiakConstants.ClientIdLength), "value");
                }

                _clientId = value;
            }
        }

        #region IRiakClient Members

        /// <summary>
        /// Ping this instance of Riak
        /// </summary>
        /// <returns>Returns true if the Riak instance has returned a 'pong' response. 
        /// Returns false if Riak is unavailable or returns a 'pang' response. </returns>
        public RiakResult Ping()
        {
            return _cluster.UseConnection(_clientId,
                                          conn => conn.PbcWriteRead<RpbPingReq, RpbPingResp>(new RpbPingReq()));
        }

        public void Ping(Action<RiakResult> callback)
        {
            ExecAsync(() => callback(Ping()));
        }

        /// <summary>
        /// Get the specified <paramref name="key"/> from the <paramref name="bucket"/>.
        /// Optionally can be read from <paramref name="rVal"/> instances. By default, the server's
        /// r-value will be used, but can be overridden by <paramref name="rVal"/>.
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
        /// that less than <paramref name="rVal" /> nodes successfully responded to the read request. Unfortunatley, 
        /// the Riak API does not allow us to distinguish between a 404 resulting from less than <paramref name="rVal"/>
        /// nodes successfully responding and a <paramref name="bucket"/>/<paramref name="key"/> combination
        /// not being found in Riak.
        /// </remarks>
        public RiakResult<RiakObject> Get(string bucket, string key, uint rVal = RiakConstants.Defaults.RVal)
        {
            var request = new RpbGetReq {Bucket = bucket.ToRiakString(), Key = key.ToRiakString(), R = rVal};
            RiakResult<RpbGetResp> result = _cluster.UseConnection(_clientId,
                                                                   conn =>
                                                                   conn.PbcWriteRead<RpbGetReq, RpbGetResp>(request));

            if (!result.IsSuccess)
            {
                return RiakResult<RiakObject>.Error(result.ResultCode, result.ErrorMessage);
            }

            if (result.Value.VectorClock == null)
            {
                return RiakResult<RiakObject>.Error(ResultCode.NotFound);
            }

            var o = new RiakObject(bucket, key, result.Value.Content.First(), result.Value.VectorClock);

            if (result.Value.Content.Count > 1)
            {
                o.Siblings =
                    result.Value.Content.Select(c => new RiakObject(bucket, key, c, result.Value.VectorClock)).ToList();
            }

            return RiakResult<RiakObject>.Success(o);
        }

        public RiakResult<RiakObject> Get(RiakObjectId objectId, uint rVal = RiakConstants.Defaults.RVal)
        {
            return Get(objectId.Bucket, objectId.Key, rVal);
        }

        public void Get(string bucket, string key, Action<RiakResult<RiakObject>> callback,
                        uint rVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(() => callback(Get(bucket, key, rVal)));
        }

        public void Get(RiakObjectId objectId, Action<RiakResult<RiakObject>> callback,
                        uint rVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(() => callback(Get(objectId.Bucket, objectId.Key, rVal)));
        }

        public IEnumerable<RiakResult<RiakObject>> Get(IEnumerable<RiakObjectId> bucketKeyPairs,
                                                       uint rVal = RiakConstants.Defaults.RVal)
        {
            List<RpbGetReq> requests =
                bucketKeyPairs.Select(
                    bk => new RpbGetReq {Bucket = bk.Bucket.ToRiakString(), Key = bk.Key.ToRiakString(), R = rVal}).
                    ToList();
            RiakResult<IEnumerable<RiakResult<RpbGetResp>>> results = _cluster.UseConnection(_clientId, conn =>
                                                                                                            {
                                                                                                                List
                                                                                                                    <
                                                                                                                        RiakResult
                                                                                                                            <
                                                                                                                                RpbGetResp
                                                                                                                                >
                                                                                                                        >
                                                                                                                    responses
                                                                                                                        =
                                                                                                                        requests
                                                                                                                            .
                                                                                                                            Select
                                                                                                                            (conn
                                                                                                                                 .
                                                                                                                                 PbcWriteRead
                                                                                                                                 <
                                                                                                                                 RpbGetReq
                                                                                                                                 ,
                                                                                                                                 RpbGetResp
                                                                                                                                 >)
                                                                                                                            .
                                                                                                                            ToList
                                                                                                                            ();
                                                                                                                return
                                                                                                                    RiakResult
                                                                                                                        <
                                                                                                                            IEnumerable
                                                                                                                                <
                                                                                                                                    RiakResult
                                                                                                                                        <
                                                                                                                                            RpbGetResp
                                                                                                                                            >
                                                                                                                                    >
                                                                                                                            >
                                                                                                                        .
                                                                                                                        Success
                                                                                                                        (responses);
                                                                                                            });

            return results.Value.Zip(bucketKeyPairs, Tuple.Create).Select(result =>
                                                                              {
                                                                                  if (!result.Item1.IsSuccess)
                                                                                  {
                                                                                      return
                                                                                          RiakResult<RiakObject>.Error(
                                                                                              result.Item1.ResultCode,
                                                                                              result.Item1.ErrorMessage);
                                                                                  }

                                                                                  if (result.Item1.Value.VectorClock ==
                                                                                      null)
                                                                                  {
                                                                                      return
                                                                                          RiakResult<RiakObject>.Error(
                                                                                              ResultCode.NotFound);
                                                                                  }

                                                                                  var o =
                                                                                      new RiakObject(
                                                                                          result.Item2.Bucket,
                                                                                          result.Item2.Key,
                                                                                          result.Item1.Value.Content.
                                                                                              First(),
                                                                                          result.Item1.Value.VectorClock);

                                                                                  if (result.Item1.Value.Content.Count >
                                                                                      1)
                                                                                  {
                                                                                      o.Siblings =
                                                                                          result.Item1.Value.Content.
                                                                                              Select(
                                                                                                  c =>
                                                                                                  new RiakObject(
                                                                                                      result.Item2.
                                                                                                          Bucket,
                                                                                                      result.Item2.Key,
                                                                                                      c,
                                                                                                      result.Item1.Value
                                                                                                          .VectorClock))
                                                                                              .ToList();
                                                                                  }

                                                                                  return
                                                                                      RiakResult<RiakObject>.Success(o);
                                                                              });
        }

        public RiakResult<RiakObject> Put(RiakObject value, RiakPutOptions options = null)
        {
            options = options ?? new RiakPutOptions();

            RpbPutReq request = value.ToMessage();
            options.Populate(request);

            RiakResult<RpbPutResp> result = _cluster.UseConnection(_clientId,
                                                                   conn =>
                                                                   conn.PbcWriteRead<RpbPutReq, RpbPutResp>(request));

            if (!result.IsSuccess)
            {
                return RiakResult<RiakObject>.Error(result.ResultCode, result.ErrorMessage);
            }

            RiakObject finalResult = options.ReturnBody
                                         ? new RiakObject(value.Bucket, value.Key, result.Value.Content.First(),
                                                          result.Value.VectorClock)
                                         : value;

            if (options.ReturnBody && result.Value.Content.Count > 1)
            {
                finalResult.Siblings =
                    result.Value.Content.Select(
                        c => new RiakObject(value.Bucket, value.Key, c, result.Value.VectorClock)).ToList();
            }

            return RiakResult<RiakObject>.Success(finalResult);
        }

        public void Put(RiakObject value, Action<RiakResult<RiakObject>> callback, RiakPutOptions options = null)
        {
            ExecAsync(() => callback(Put(value, options)));
        }

        public IEnumerable<RiakResult<RiakObject>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null)
        {
            options = options ?? new RiakPutOptions();

            List<RpbPutReq> messages = values.Select(v =>
                                                         {
                                                             RpbPutReq m = v.ToMessage();
                                                             options.Populate(m);
                                                             return m;
                                                         }).ToList();

            RiakResult<IEnumerable<RiakResult<RpbPutResp>>> results = _cluster.UseConnection(_clientId, conn =>
                                                                                                            {
                                                                                                                List
                                                                                                                    <
                                                                                                                        RiakResult
                                                                                                                            <
                                                                                                                                RpbPutResp
                                                                                                                                >
                                                                                                                        >
                                                                                                                    responses
                                                                                                                        =
                                                                                                                        messages
                                                                                                                            .
                                                                                                                            Select
                                                                                                                            (conn
                                                                                                                                 .
                                                                                                                                 PbcWriteRead
                                                                                                                                 <
                                                                                                                                 RpbPutReq
                                                                                                                                 ,
                                                                                                                                 RpbPutResp
                                                                                                                                 >)
                                                                                                                            .
                                                                                                                            ToList
                                                                                                                            ();
                                                                                                                return
                                                                                                                    RiakResult
                                                                                                                        <
                                                                                                                            IEnumerable
                                                                                                                                <
                                                                                                                                    RiakResult
                                                                                                                                        <
                                                                                                                                            RpbPutResp
                                                                                                                                            >
                                                                                                                                    >
                                                                                                                            >
                                                                                                                        .
                                                                                                                        Success
                                                                                                                        (responses);
                                                                                                            });

            return results.Value.Zip(values, Tuple.Create).Select(t =>
                                                                      {
                                                                          if (t.Item1.IsSuccess)
                                                                          {
                                                                              RiakObject finalResult =
                                                                                  options.ReturnBody
                                                                                      ? new RiakObject(t.Item2.Bucket,
                                                                                                       t.Item2.Key,
                                                                                                       t.Item1.Value.
                                                                                                           Content.First
                                                                                                           (),
                                                                                                       t.Item1.Value.
                                                                                                           VectorClock)
                                                                                      : t.Item2;

                                                                              if (options.ReturnBody &&
                                                                                  t.Item1.Value.Content.Count > 1)
                                                                              {
                                                                                  finalResult.Siblings =
                                                                                      t.Item1.Value.Content.Select(
                                                                                          c =>
                                                                                          new RiakObject(
                                                                                              t.Item2.Bucket,
                                                                                              t.Item2.Key, c,
                                                                                              t.Item1.Value.VectorClock))
                                                                                          .ToList();
                                                                              }

                                                                              return
                                                                                  RiakResult<RiakObject>.Success(
                                                                                      finalResult);
                                                                          }
                                                                          return
                                                                              RiakResult<RiakObject>.Error(
                                                                                  t.Item1.ResultCode,
                                                                                  t.Item1.ErrorMessage);
                                                                      });
        }

        public void Put(IEnumerable<RiakObject> values, Action<IEnumerable<RiakResult<RiakObject>>> callback,
                        RiakPutOptions options = null)
        {
            ExecAsync(() => callback(Put(values, options)));
        }

        public RiakResult Delete(string bucket, string key, uint rwVal = RiakConstants.Defaults.RVal)
        {
            var request = new RpbDelReq {Bucket = bucket.ToRiakString(), Key = key.ToRiakString(), Rw = rwVal};
            RiakResult<RpbDelResp> result = _cluster.UseConnection(_clientId,
                                                                   conn =>
                                                                   conn.PbcWriteRead<RpbDelReq, RpbDelResp>(request));

            return result;
        }

        public RiakResult Delete(RiakObjectId objectId, uint rwVal = RiakConstants.Defaults.RVal)
        {
            return Delete(objectId.Bucket, objectId.Key, rwVal);
        }

        public void Delete(string bucket, string key, Action<RiakResult> callback,
                           uint rwVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(() => callback(Delete(bucket, key, rwVal)));
        }

        public void Delete(RiakObjectId objectId, Action<RiakResult> callback, uint rwVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(() => callback(Delete(objectId.Bucket, objectId.Key, rwVal)));
        }

        public IEnumerable<RiakResult> Delete(IEnumerable<RiakObjectId> objectIds,
                                              uint rwVal = RiakConstants.Defaults.RVal)
        {
            List<RpbDelReq> requests =
                objectIds.Select(
                    id => new RpbDelReq {Bucket = id.Bucket.ToRiakString(), Key = id.Key.ToRiakString(), Rw = rwVal}).
                    ToList();
            RiakResult<IEnumerable<RiakResult>> results = _cluster.UseConnection(_clientId, conn =>
                                                                                                {
                                                                                                    List
                                                                                                        <
                                                                                                            RiakResult
                                                                                                                <
                                                                                                                    RpbDelResp
                                                                                                                    >>
                                                                                                        responses =
                                                                                                            requests.
                                                                                                                Select(
                                                                                                                    conn
                                                                                                                        .
                                                                                                                        PbcWriteRead
                                                                                                                        <
                                                                                                                        RpbDelReq
                                                                                                                        ,
                                                                                                                        RpbDelResp
                                                                                                                        >)
                                                                                                                .ToList();
                                                                                                    return
                                                                                                        RiakResult
                                                                                                            <
                                                                                                                IEnumerable
                                                                                                                    <
                                                                                                                        RiakResult
                                                                                                                        >
                                                                                                                >.
                                                                                                            Success(
                                                                                                                responses);
                                                                                                });

            return results.Value;
        }

        public void Delete(IEnumerable<RiakObjectId> objectIds, Action<IEnumerable<RiakResult>> callback,
                           uint rwVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(() => callback(Delete(objectIds, rwVal)));
        }

        public IEnumerable<RiakResult> DeleteBucket(string bucket, uint rwVal = RiakConstants.Defaults.RVal)
        {
            // TODO: change this to do the whole op with a single connection and with streaming
            RiakResult<IEnumerable<string>> keys = ListKeys(bucket);
            List<RiakObjectId> objectIds = keys.Value.Select(key => new RiakObjectId(bucket, key)).ToList();

            return Delete(objectIds, rwVal);
        }

        public void DeleteBucket(string bucket, Action<IEnumerable<RiakResult>> callback,
                                 uint rwVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(() => callback(DeleteBucket(bucket, rwVal)));
        }

        public RiakResult<RiakMapReduceResult> MapReduce(RiakMapReduceQuery query)
        {
            RpbMapRedReq request = query.ToMessage();
            RiakResult<IEnumerable<RpbMapRedResp>> response = _cluster.UseConnection(_clientId,
                                                                                     conn =>
                                                                                     conn.PbcWriteRead
                                                                                         <RpbMapRedReq, RpbMapRedResp>(
                                                                                             request, r => !r.Done));

            if (response.IsSuccess)
            {
                return RiakResult<RiakMapReduceResult>.Success(new RiakMapReduceResult(response.Value));
            }

            return RiakResult<RiakMapReduceResult>.Error(response.ResultCode, response.ErrorMessage);
        }

        public void MapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakMapReduceResult>> callback)
        {
            ExecAsync(() => callback(MapReduce(query)));
        }

        public RiakResult<RiakStreamedMapReduceResult> StreamMapReduce(RiakMapReduceQuery query)
        {
            RpbMapRedReq request = query.ToMessage();
            RiakResult<IEnumerable<RpbMapRedResp>> response = _cluster.UseStreamConnection(_clientId,
                                                                                           (conn, onFinish) =>
                                                                                           conn.PbcWriteStreamRead
                                                                                               <RpbMapRedReq,
                                                                                               RpbMapRedResp>(request,
                                                                                                              r =>
                                                                                                              !r.Done,
                                                                                                              onFinish));

            if (response.IsSuccess)
            {
                return RiakResult<RiakStreamedMapReduceResult>.Success(new RiakStreamedMapReduceResult(response.Value));
            }
            return RiakResult<RiakStreamedMapReduceResult>.Error(response.ResultCode, response.ErrorMessage);
        }

        public void StreamMapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakStreamedMapReduceResult>> callback)
        {
            ExecAsync(() => callback(StreamMapReduce(query)));
        }

        public RiakResult<IEnumerable<string>> ListBuckets()
        {
            var lbReq = new RpbListBucketsReq();
            RiakResult<RpbListBucketsResp> result = _cluster.UseConnection(_clientId,
                                                                           conn =>
                                                                           conn.PbcWriteRead
                                                                               <RpbListBucketsReq, RpbListBucketsResp>(
                                                                                   lbReq));

            if (result.IsSuccess)
            {
                IEnumerable<string> buckets = result.Value.Buckets.Select(b => b.FromRiakString());
                return RiakResult<IEnumerable<string>>.Success(buckets);
            }
            return RiakResult<IEnumerable<string>>.Error(result.ResultCode, result.ErrorMessage);
        }

        public void ListBuckets(Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(ListBuckets()));
        }

        public RiakResult<IEnumerable<string>> ListKeys(string bucket)
        {
            var lkReq = new RpbListKeysReq {Bucket = bucket.ToRiakString()};
            RiakResult<IEnumerable<RpbListKeysResp>> result = _cluster.UseConnection(_clientId,
                                                                                     conn =>
                                                                                     conn.PbcWriteRead
                                                                                         <RpbListKeysReq,
                                                                                         RpbListKeysResp>(lkReq,
                                                                                                          lkr =>
                                                                                                          !lkr.Done));

            if (result.IsSuccess)
            {
                List<string> keys = result.Value.SelectMany(r => r.KeyNames).ToList();
                return RiakResult<IEnumerable<string>>.Success(keys);
            }
            return RiakResult<IEnumerable<string>>.Error(result.ResultCode, result.ErrorMessage);
        }

        public void ListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(ListKeys(bucket)));
        }

        public RiakResult<IEnumerable<string>> StreamListKeys(string bucket)
        {
            var lkReq = new RpbListKeysReq {Bucket = bucket.ToRiakString()};
            RiakResult<IEnumerable<RpbListKeysResp>> result = _cluster.UseStreamConnection(_clientId,
                                                                                           (conn, onFinish) =>
                                                                                           conn.PbcWriteStreamRead
                                                                                               <RpbListKeysReq,
                                                                                               RpbListKeysResp>(lkReq,
                                                                                                                lkr =>
                                                                                                                !lkr.
                                                                                                                     Done,
                                                                                                                onFinish));

            if (result.IsSuccess)
            {
                IEnumerable<string> keys = result.Value.SelectMany(r => r.KeyNames);
                return RiakResult<IEnumerable<string>>.Success(keys);
            }
            return RiakResult<IEnumerable<string>>.Error(result.ResultCode, result.ErrorMessage);
        }

        public void StreamListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(StreamListKeys(bucket)));
        }

        public RiakResult<RiakBucketProperties> GetBucketProperties(string bucket, bool extended = false)
        {
            if (extended)
            {
                RiakRestRequest request = new RiakRestRequest(ToBucketUri(bucket), RiakConstants.Rest.HttpMethod.Get)
                    .AddQueryParam(RiakConstants.Rest.QueryParameters.Bucket.GetPropertiesKey,
                                   RiakConstants.Rest.QueryParameters.Bucket.GetPropertiesValue);

                RiakResult<RiakRestResponse> result = _cluster.UseConnection(_clientId,
                                                                             conn => conn.RestRequest(request));
                if (result.IsSuccess)
                {
                    if (result.Value.StatusCode == HttpStatusCode.OK)
                    {
                        var response = new RiakBucketProperties(result.Value);
                        return RiakResult<RiakBucketProperties>.Success(response);
                    }
                    return RiakResult<RiakBucketProperties>.Error(ResultCode.InvalidResponse,
                                                                  "Unexpected Status Code: {0} ({1})".Fmt(
                                                                      result.Value.StatusCode,
                                                                      (int) result.Value.StatusCode));
                }
                return RiakResult<RiakBucketProperties>.Error(result.ResultCode, result.ErrorMessage);
            }
            else
            {
                var bpReq = new RpbGetBucketReq {Bucket = bucket.ToRiakString()};
                RiakResult<RpbGetBucketResp> result = _cluster.UseConnection(_clientId,
                                                                             conn =>
                                                                             conn.PbcWriteRead
                                                                                 <RpbGetBucketReq, RpbGetBucketResp>(
                                                                                     bpReq));

                if (result.IsSuccess)
                {
                    var props = new RiakBucketProperties(result.Value.Props);
                    return RiakResult<RiakBucketProperties>.Success(props);
                }
                return RiakResult<RiakBucketProperties>.Error(result.ResultCode, result.ErrorMessage);
            }
        }

        public void GetBucketProperties(string bucket, Action<RiakResult<RiakBucketProperties>> callback,
                                        bool extended = false)
        {
            ExecAsync(() => callback(GetBucketProperties(bucket, extended)));
        }

        public RiakResult SetBucketProperties(string bucket, RiakBucketProperties properties)
        {
            if (properties.CanUsePbc)
            {
                var request = new RpbSetBucketReq {Bucket = bucket.ToRiakString(), Props = properties.ToMessage()};
                RiakResult<RpbSetBucketResp> result = _cluster.UseConnection(_clientId,
                                                                             conn =>
                                                                             conn.PbcWriteRead
                                                                                 <RpbSetBucketReq, RpbSetBucketResp>(
                                                                                     request));
                return result;
            }
            else
            {
                var request = new RiakRestRequest(ToBucketUri(bucket), RiakConstants.Rest.HttpMethod.Put)
                                  {
                                      Body = properties.ToJsonString().ToRiakString(),
                                      ContentType = RiakConstants.ContentTypes.ApplicationJson
                                  };

                RiakResult<RiakRestResponse> result = _cluster.UseConnection(_clientId,
                                                                             conn => conn.RestRequest(request));
                if (result.IsSuccess && result.Value.StatusCode != HttpStatusCode.NoContent)
                {
                    return RiakResult.Error(ResultCode.InvalidResponse,
                                            "Unexpected Status Code: {0} ({1})".Fmt(result.Value.StatusCode,
                                                                                    (int) result.Value.StatusCode));
                }
                return result;
            }
        }

        public void SetBucketProperties(string bucket, RiakBucketProperties properties, Action<RiakResult> callback)
        {
            ExecAsync(() => callback(SetBucketProperties(bucket, properties)));
        }

        public IList<RiakObject> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks)
        {
            RiakMapReduceQuery query =
                new RiakMapReduceQuery().Inputs(
                    new RiakPhaseInputs(new List<RiakBucketKeyInput>
                                            {new RiakBucketKeyInput(riakObject.Bucket, riakObject.Key)}));

            foreach (RiakLink riakLink in riakLinks)
            {
                RiakLink link = riakLink;
                bool keep = (link == riakLinks.Last());

                query.Link(l => l.FromRiakLink(link)
                                    .Keep(keep));
            }

            RiakMapReduceResultPhase linkResults = MapReduce(query).Value.PhaseResults.Last();
            string linkResultString = linkResults.Value.FromRiakString();
            IList<RiakLink> rawLinks = RiakLink.ParseArrayFromJsonString(linkResultString);
            List<RiakObjectId> oids =
                rawLinks.Select(riakLink => new RiakObjectId(riakLink.Bucket, riakLink.Key)).ToList();

            return Get(oids).Select(r => r.Value).ToList();
        }

        /// <summary>
        /// Get the server information from the connected cluster.
        /// </summary>
        /// <returns>Model containing information gathered from a node in the cluster.</returns>
        /// <remarks>This function will assume that all of the nodes in the cluster are running
        /// the same version of Riak. It will only get executed on a single node, and the content
        /// that is returned technically only relates to that node. All nodes in a cluster should
        /// run on the same version of Riak.</remarks>
        public RiakResult<RiakServerInfo> GetServerInfo()
        {
            RiakResult<RpbGetServerInfoResp> result = _cluster.UseConnection(_clientId,
                                                                             conn =>
                                                                             conn.PbcWriteRead
                                                                                 <RpbGetServerInfoReq,
                                                                                 RpbGetServerInfoResp>(
                                                                                     new RpbGetServerInfoReq()));
            if (result.IsSuccess)
            {
                return RiakResult<RiakServerInfo>.Success(new RiakServerInfo(result.Value));
            }
            return RiakResult<RiakServerInfo>.Error(result.ResultCode, result.ErrorMessage);
        }

        public void GetServerInfo(Action<RiakResult<RiakServerInfo>> callback)
        {
            ExecAsync(() => callback(GetServerInfo()));
        }

        #endregion

        public void Get(IEnumerable<RiakObjectId> bucketKeyPairs, Action<IEnumerable<RiakResult<RiakObject>>> callback,
                        uint rVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(() => callback(Get(bucketKeyPairs, rVal)));
        }

        private static byte[] GetClientId()
        {
            NetworkInterface nic =
                NetworkInterface.GetAllNetworkInterfaces().Where(i => (i.OperationalStatus == OperationalStatus.Up)
                                                                      && (i.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                                                                      && (i.NetworkInterfaceType != NetworkInterfaceType.Unknown)
                                                                      && (i.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                                                                      // eliminating virtual devices and MS supplied virtual devices masquerading as physical devices
                                                                      && !((i.Id.IndexOf("Root") == 0) || (i.Description.IndexOf("Root") == 0))
                                                                      && !(i.Description.Contains("Microsoft"))
                                                                )
                    .OrderBy(i => i.Id)
                    .First();

            byte[] mac = nic.GetPhysicalAddress().GetAddressBytes();
            Array.Resize(ref mac, 4);
            return mac;
        }

        private static void ExecAsync(Action action)
        {
            ThreadPool.QueueUserWorkItem(o => action());
        }

        private static string ToBucketUri(string bucket)
        {
            return "{0}/{1}".Fmt(RiakConstants.Rest.Uri.RiakRoot, bucket);
        }
    }
}