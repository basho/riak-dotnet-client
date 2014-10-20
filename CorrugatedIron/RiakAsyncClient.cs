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

using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Web;
using CorrugatedIron.Comms;
using CorrugatedIron.Containers;
using CorrugatedIron.Exceptions;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Models.Index;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Models.Rest;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorrugatedIron
{
    internal class RiakAsyncClient : IRiakAsyncClient
    {
        private const string ListKeysWarning = "*** [CI] -> ListKeys is an expensive operation and should not be used in Production scenarios. ***";
        private const string InvalidBucketErrorMessage = "Bucket cannot be blank or contain forward-slashes";
        private const string InvalidKeyErrorMessage = "Key cannot be blank or contain forward-slashes";

        private readonly IRiakEndPoint _endPoint;
        private readonly IRiakConnection _connection;

        internal RiakAsyncClient(IRiakEndPoint endPoint, IRiakConnection connection)
        {
            _endPoint = endPoint;
            _connection = connection;
        }

        public Task Batch(Action<IRiakAsyncBatchClient> batchAction)
        {
            if (_endPoint is RiakBatch)
            {
                batchAction(this);
                return Task.FromResult(false);
            }

            using (var batchEndPoint = new RiakBatch(_endPoint, new RiakEndPointContext()))
            {
                using (var batchedAsyncClient = new RiakAsyncClient(batchEndPoint, _connection))
                {
                    batchAction(batchedAsyncClient);
                    return Task.FromResult(false);
                }
            }
        }

        public Task<T> Batch<T>(Func<IRiakAsyncBatchClient, T> batchFunction)
        {
            if (_endPoint is RiakBatch)
            {
                return Task.FromResult(batchFunction(this));
            }

            using (var batchEndPoint = new RiakBatch(_endPoint, new RiakEndPointContext()))
            {
                using (var batchedAsyncClient = new RiakAsyncClient(batchEndPoint, _connection))
                {
                    return Task.FromResult(batchFunction(batchedAsyncClient));
                }
            }
        }

        public IObservable<T> Batch<T>(Func<IRiakAsyncBatchClient, IObservable<T>> batchFunction)
        {
            if (_endPoint is RiakBatch)
            {
                return batchFunction(this);
            }

            using (var batchEndPoint = new RiakBatch(_endPoint, new RiakEndPointContext()))
            {
                using (var batchedAsyncClient = new RiakAsyncClient(batchEndPoint, _connection))
                {
                    return batchFunction(batchedAsyncClient);
                }
            }
        }

        #region Helper functions

        private static bool IsValidBucketOrKey(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && !value.Contains("/");
        }

        private static RiakGetOptions DefaultGetOptions()
        {
            return (new RiakGetOptions()).SetR(RiakConstants.Defaults.RVal);
        }

        #endregion

        public async Task<Pong> Ping()
        {
            var startTime = Stopwatch.StartNew();
            await _connection.PbcWriteRead(_endPoint, MessageCode.PingReq, MessageCode.PingResp).ConfigureAwait(false);
            startTime.Stop();

            var pong = new Pong
            {
                ResponseTime = startTime.Elapsed
            };

            return pong;
        }

        public async Task<Either<RiakException, RiakObject>> Get(string bucket, string key, RiakGetOptions options = null)
        {
            options = options ?? RiakClient.DefaultGetOptions();

            if (!IsValidBucketOrKey(bucket))
            {
                return new Either<RiakException, RiakObject>(new RiakException((uint)ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
            }

            if (!IsValidBucketOrKey(key))
            {
                return new Either<RiakException, RiakObject>(new RiakException((uint)ResultCode.InvalidRequest, InvalidKeyErrorMessage, false));
            }

            var request = new RpbGetReq { bucket = bucket.ToRiakString(), key = key.ToRiakString() };

            options = options ?? new RiakGetOptions();
            options.Populate(request);

            try
            {
                var result = await _connection.PbcWriteRead<RpbGetReq, RpbGetResp>(_endPoint, request).ConfigureAwait(false);

                if (result.vclock == null)
                {
                    return new Either<RiakException, RiakObject>(new RiakException((uint)ResultCode.NotFound, "Unable to find value in Riak", false));
                }

                var riakObject = new RiakObject(bucket, key, result.content, result.vclock);

                return new Either<RiakException, RiakObject>(riakObject);
            }
            catch (RiakException riakException)
            {
                return new Either<RiakException, RiakObject>(riakException);
            }
        }

        public Task<Either<RiakException, RiakObject>> Get(RiakObjectId objectId, RiakGetOptions options = null)
        {
            options = options ?? DefaultGetOptions();
            return Get(objectId.Bucket, objectId.Key, options);
        }

        public IObservable<Either<RiakException, RiakObject>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, RiakGetOptions options = null)
        {
            var observable = Observable.Create<Either<RiakException, RiakObject>>(async obs =>
            {
                try
                {
                    bucketKeyPairs = bucketKeyPairs.ToList();
                    options = options ?? new RiakGetOptions();

                    foreach (var bucketKeyPair in bucketKeyPairs)
                    {

                        // modified closure FTW
                        var bk = bucketKeyPair;
                        if (!IsValidBucketOrKey(bk.Bucket))
                        {
                            obs.OnNext(new Either<RiakException, RiakObject>(new RiakException((uint)ResultCode.InvalidRequest, InvalidBucketErrorMessage, false)));
                            continue;
                        }

                        if (!IsValidBucketOrKey(bk.Key))
                        {
                            obs.OnNext(new Either<RiakException, RiakObject>(new RiakException((uint)ResultCode.InvalidRequest, InvalidKeyErrorMessage, false)));
                            continue;
                        }

                        var req = new RpbGetReq
                        {
                            bucket = bk.Bucket.ToRiakString(),
                            key = bk.Key.ToRiakString()
                        };
                        options.Populate(req);

                        try
                        {
                            var result = await _connection.PbcWriteRead<RpbGetReq, RpbGetResp>(_endPoint, req)
                                        .ConfigureAwait(false);

                            if (result.vclock == null)
                            {
                                obs.OnNext(new Either<RiakException, RiakObject>(new RiakException((uint)ResultCode.NotFound, "Unable to find value in Riak", false)));
                                continue;
                            }

                            var riakObject = new RiakObject(bk.Bucket, bk.Key, result.content.First(), result.vclock);

                            if (result.content.Count > 1)
                            {
                                riakObject.Siblings = result.content
                                    .Select(c => new RiakObject(bk.Bucket, bk.Key, c, result.vclock))
                                    .ToList();
                            }

                            obs.OnNext(new Either<RiakException, RiakObject>(riakObject));
                        }
                        catch (RiakException riakException)
                        {
                            obs.OnNext(new Either<RiakException, RiakObject>(riakException));
                        }
                    }

                    obs.OnCompleted();

                }
                catch (Exception exception)
                {
                    obs.OnError(exception);
                }
                return Disposable.Empty;
            });

            return observable;
        }

        public async Task<Either<RiakException, RiakCounterResult>> IncrementCounter(string bucket, string counter, long amount, RiakCounterUpdateOptions options = null)
        {
            if (!IsValidBucketOrKey(bucket))
            {
                return new Either<RiakException, RiakCounterResult>(new RiakException((uint)ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
            }

            if (!IsValidBucketOrKey(counter))
            {
                return new Either<RiakException, RiakCounterResult>(new RiakException((uint)ResultCode.InvalidRequest, InvalidKeyErrorMessage, false));
            }

            var request = new RpbCounterUpdateReq { bucket = bucket.ToRiakString(), key = counter.ToRiakString(), amount = amount };
            options = options ?? new RiakCounterUpdateOptions();
            options.Populate(request);

            try
            {
                var result = await _connection.PbcWriteRead<RpbCounterUpdateReq, RpbCounterUpdateResp>(_endPoint, request)
                            .ConfigureAwait(false);

                var riakObject = new RiakObject(bucket, counter, result.value);
                var cVal = 0L;
                var parseResult = false;

                if (options.ReturnValue != null && options.ReturnValue.Value)
                {
                    parseResult = long.TryParse(riakObject.Value.FromRiakString(), out cVal);
                }

                return new Either<RiakException, RiakCounterResult>(new RiakCounterResult(riakObject, parseResult ? (long?)cVal : null));
            }
            catch (RiakException riakException)
            {
                return new Either<RiakException, RiakCounterResult>(riakException);
            }
        }

        public async Task<Either<RiakException, RiakCounterResult>> GetCounter(string bucket, string counter, RiakCounterGetOptions options = null)
        {
            if (!IsValidBucketOrKey(bucket))
            {
                return new Either<RiakException, RiakCounterResult>(new RiakException((uint)ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
            }

            if (!IsValidBucketOrKey(counter))
            {
                return new Either<RiakException, RiakCounterResult>(new RiakException((uint)ResultCode.InvalidRequest, InvalidKeyErrorMessage, false));
            }

            var request = new RpbCounterGetReq { bucket = bucket.ToRiakString(), key = counter.ToRiakString() };
            options = options ?? new RiakCounterGetOptions();
            options.Populate(request);

            try
            {
                var result = await _connection.PbcWriteRead<RpbCounterGetReq, RpbCounterGetResp>(_endPoint, request).ConfigureAwait(false);

                var riakObject = new RiakObject(bucket, counter, result.value);
                long cVal;
                var parseResult = long.TryParse(riakObject.Value.FromRiakString(), out cVal);

                return new Either<RiakException, RiakCounterResult>(new RiakCounterResult(riakObject, parseResult ? (long?)cVal : null));
            }
            catch (RiakException riakException)
            {
                return new Either<RiakException, RiakCounterResult>(riakException);
            }
        }

        public IObservable<Either<RiakException, RiakObject>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null)
        {
            var observables = Observable.Create<Either<RiakException, RiakObject>>(async obs =>
            {
                try
                {
                    options = options ?? new RiakPutOptions();

                    foreach (var v in values)
                    {
                        if (!IsValidBucketOrKey(v.Bucket))
                        {
                            obs.OnNext(new Either<RiakException, RiakObject>(new RiakException((uint)ResultCode.InvalidRequest, InvalidBucketErrorMessage, false)));
                            continue;
                        }

                        if (!IsValidBucketOrKey(v.Key))
                        {
                            obs.OnNext(new Either<RiakException, RiakObject>(new RiakException((uint)ResultCode.InvalidRequest, InvalidKeyErrorMessage, false)));
                            continue;
                        }

                        var msg = v.ToMessage();
                        options.Populate(msg);

                        try
                        {
                            var result = await
                                    _connection.PbcWriteRead<RpbPutReq, RpbPutResp>(_endPoint, msg)
                                        .ConfigureAwait(false);

                            var finalResult = options.ReturnBody
                                ? new RiakObject(v.Bucket, v.Key, result.content.First(), result.vclock)
                                : v;

                            if (options.ReturnBody && result.content.Count > 1)
                            {
                                finalResult.Siblings = result.content
                                    .Select(c => new RiakObject(v.Bucket, v.Key, c, result.vclock))
                                    .ToList();
                            }

                            obs.OnNext(new Either<RiakException, RiakObject>(finalResult));
                        }
                        catch (RiakException riakException)
                        {
                            obs.OnNext(new Either<RiakException, RiakObject>(riakException));
                            continue;
                        }
                    }
                    obs.OnCompleted();
                }
                catch (Exception exception)
                {
                    obs.OnError(exception);
                }
                return Disposable.Empty;
            });

            return observables;
        }

        public async Task<Either<RiakException, RiakObject>> Put(RiakObject value, RiakPutOptions options = null)
        {
            if (!IsValidBucketOrKey(value.Bucket))
            {
                return new Either<RiakException, RiakObject>(new RiakException((uint)ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
            }

            if (!IsValidBucketOrKey(value.Key))
            {
                return new Either<RiakException, RiakObject>(new RiakException((uint)ResultCode.InvalidRequest, InvalidKeyErrorMessage, false));
            }

            options = options ?? new RiakPutOptions();

            var request = value.ToMessage();
            options.Populate(request);

            try
            {
                var result =
                    await _connection.PbcWriteRead<RpbPutReq, RpbPutResp>(_endPoint, request).ConfigureAwait(false);

                var finalResult = options.ReturnBody
                    ? new RiakObject(value.Bucket, value.Key, result.content.First(), result.vclock)
                    : value;

                if (options.ReturnBody && result.content.Count > 1)
                {
                    finalResult.Siblings = result.content.Select(c =>
                        new RiakObject(value.Bucket, value.Key, c, result.vclock)).ToList();
                }

                return new Either<RiakException, RiakObject>(finalResult);
            }
            catch (RiakException riakException)
            {
                return new Either<RiakException, RiakObject>(riakException);

            }
        }

        public Task<Either<RiakException, RiakObjectId>> Delete(RiakObject riakObject, RiakDeleteOptions options = null)
        {
            return Delete(riakObject.Bucket, riakObject.Key, options);
        }

        public async Task<Either<RiakException, RiakObjectId>> Delete(string bucket, string key, RiakDeleteOptions options = null)
        {
            if (!IsValidBucketOrKey(bucket))
            {
                return new Either<RiakException, RiakObjectId>(new RiakException((uint)ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
            }

            if (!IsValidBucketOrKey(key))
            {
                return new Either<RiakException, RiakObjectId>(new RiakException((uint)ResultCode.InvalidRequest, InvalidKeyErrorMessage, false));
            }

            options = options ?? new RiakDeleteOptions();

            var request = new RpbDelReq { bucket = bucket.ToRiakString(), key = key.ToRiakString() };
            options.Populate(request);

            try
            {
                await _connection.PbcWriteRead(_endPoint, request, MessageCode.DelResp).ConfigureAwait(false);

                return new Either<RiakException, RiakObjectId>(new RiakObjectId(bucket, key));
            }
            catch (RiakException riakException)
            {
                return new Either<RiakException, RiakObjectId>(riakException);
            }
        }

        public Task<Either<RiakException, RiakObjectId>> Delete(RiakObjectId objectId, RiakDeleteOptions options = null)
        {
            return Delete(objectId.Bucket, objectId.Key, options);
        }

        public IObservable<Either<RiakException, RiakObjectId>> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null)
        {
            var observables = Observable.Create<Either<RiakException, RiakObjectId>>(async obs =>
            {
                try
                {
                    options = options ?? new RiakDeleteOptions();

                    foreach (var id in objectIds)
                    {
                        if (!IsValidBucketOrKey(id.Bucket))
                        {
                            obs.OnNext(new Either<RiakException, RiakObjectId>(new RiakException((uint)ResultCode.InvalidRequest, InvalidBucketErrorMessage, false)));
                            continue;
                        }

                        if (!IsValidBucketOrKey(id.Key))
                        {

                            obs.OnNext(new Either<RiakException, RiakObjectId>(new RiakException((uint)ResultCode.InvalidRequest, InvalidKeyErrorMessage, false)));
                            continue;
                        }

                        var req = new RpbDelReq { bucket = id.Bucket.ToRiakString(), key = id.Key.ToRiakString() };
                        options.Populate(req);

                        try
                        {
                            await _connection.PbcWriteRead(_endPoint, req, MessageCode.DelResp).ConfigureAwait(false);
                        }
                        catch (RiakException riakException)
                        {
                            obs.OnNext(new Either<RiakException, RiakObjectId>(riakException));
                            continue;
                        }

                        obs.OnNext(new Either<RiakException, RiakObjectId>(id));
                    }
                    obs.OnCompleted();
                }
                catch (Exception exception)
                {
                    obs.OnError(exception);
                }
                return Disposable.Empty;
            });

            return observables;
        }

        public IObservable<Either<RiakException, RiakObjectId>> DeleteBucket(string bucket, RiakDeleteOptions deleteOptions = null)
        {
            var objectIds = ListKeys(bucket)
                .Where(x => !x.IsLeft)
                .Select(key => new RiakObjectId(bucket, key.Right))
                .ToEnumerable()
                .ToList();

            return Delete(objectIds, deleteOptions);
        }

        public async Task<Either<RiakException, RiakSearchResult>> Search(RiakSearchRequest search)
        {
            var request = search.ToMessage();
            try
            {
                var response = await _connection.PbcWriteRead<RpbSearchQueryReq, RpbSearchQueryResp>(_endPoint, request)
                    .ConfigureAwait(false);

                return new Either<RiakException, RiakSearchResult>(new RiakSearchResult(response));
            }
            catch (RiakException riakException)
            {
                return new Either<RiakException, RiakSearchResult>(riakException);
            }
        }

        public async Task<RiakMapReduceResult> MapReduce(RiakMapReduceQuery query)
        {
            var request = query.ToMessage();
            var response = _connection
                .PbcWriteRead<RpbMapRedReq, RpbMapRedResp>(_endPoint, request, r => !r.done);

            return new RiakMapReduceResult(response);
        }

        public async Task<RiakStreamedMapReduceResult> StreamMapReduce(RiakMapReduceQuery query)
        {
            var request = query.ToMessage();
            var response = _connection
                .PbcWriteStreamRead<RpbMapRedReq, RpbMapRedResp>(_endPoint, request, r => !r.done);

            return new RiakStreamedMapReduceResult(response);
        }

        public IObservable<Either<RiakException, string>> StreamListBuckets()
        {
            var lbReq = new RpbListBucketsReq { stream = true };

            var buckets = _connection
                .PbcWriteStreamRead<RpbListBucketsReq, RpbListBucketsResp>(_endPoint, lbReq, lbr => !lbr.done)
                .SelectMany(r => r.buckets)
                .Select(k => new Either<RiakException, string>(k.FromRiakString()))
                .Catch<Either<RiakException, string>, RiakException>(exception => Observable.Return(new Either<RiakException, string>(exception)));

            return buckets;
        }

        public IObservable<Either<RiakException, string>> ListBuckets()
        {
            var observable = Observable.Create<Either<RiakException, string>>(async obs =>
            {
                try
                {
                    try
                    {
                        var result = await _connection.PbcWriteRead<RpbListBucketsResp>(_endPoint, MessageCode.ListBucketsReq)
                                    .ConfigureAwait(false);

                        var buckets = result.buckets
                            .Select(b => b.FromRiakString());

                        foreach (var bucket in buckets)
                        {
                            obs.OnNext(new Either<RiakException, string>(bucket));
                        }
                    }
                    catch (RiakException riakException)
                    {
                        obs.OnNext(new Either<RiakException, string>(riakException));
                    }
                    obs.OnCompleted();
                }
                catch (Exception exception)
                {
                    obs.OnError(exception);
                }
                return Disposable.Empty;
            });

            return observable;
        }

        public IObservable<Either<RiakException, string>> ListKeys(string bucket)
        {
            System.Diagnostics.Debug.Write(ListKeysWarning);
            System.Diagnostics.Trace.TraceWarning(ListKeysWarning);
            Console.WriteLine(ListKeysWarning);

            var lkReq = new RpbListKeysReq { bucket = bucket.ToRiakString() };

            var keys = _connection
                .PbcWriteRead<RpbListKeysReq, RpbListKeysResp>(_endPoint, lkReq, lkr => !lkr.done)
                .SelectMany(r => r.keys)
                .Select(k => new Either<RiakException, string>(k.FromRiakString()))
                .Catch<Either<RiakException, string>, RiakException>(exception => Observable.Return(new Either<RiakException, string>(exception)));


            return keys;
        }

        public IObservable<Either<RiakException, string>> StreamListKeys(string bucket)
        {
            System.Diagnostics.Debug.Write(ListKeysWarning);
            System.Diagnostics.Trace.TraceWarning(ListKeysWarning);
            Console.WriteLine(ListKeysWarning);

            var lkReq = new RpbListKeysReq { bucket = bucket.ToRiakString() };

            var keys = _connection
                .PbcWriteStreamRead<RpbListKeysReq, RpbListKeysResp>(_endPoint, lkReq, lkr => !lkr.done)
                .SelectMany(r => r.keys)
                .Select(k => new Either<RiakException, string>(k.FromRiakString()))
                .Catch<Either<RiakException, string>, RiakException>(exception => Observable.Return(new Either<RiakException, string>(exception)));


            return keys;
        }

        public async Task<RiakBucketProperties> GetBucketProperties(string bucket)
        {
            // bucket names cannot have slashes in the names, the REST interface doesn't like it at all
            if (!IsValidBucketOrKey(bucket))
            {
                throw new RiakException((uint)ResultCode.InvalidRequest, InvalidBucketErrorMessage, false);
            }

            var bpReq = new RpbGetBucketReq { bucket = bucket.ToRiakString() };
            var result = await _connection.PbcWriteRead<RpbGetBucketReq, RpbGetBucketResp>(_endPoint, bpReq).ConfigureAwait(false);

            var bucketProperties = new RiakBucketProperties(result.props);
            return bucketProperties;
        }

        public async Task<bool> SetBucketProperties(string bucket, RiakBucketProperties properties, bool useHttp = false)
        {
            var task = useHttp ? SetHttpBucketProperties(bucket, properties) : SetPbcBucketProperties(bucket, properties);
            await task;
            return true;
        }

        private static string ToBucketUri(string bucket)
        {
            return "{0}/{1}".Fmt(RiakConstants.Rest.Uri.RiakRoot, HttpUtility.UrlEncode(bucket));
        }

        private async Task SetHttpBucketProperties(string bucket, RiakBucketProperties properties)
        {
            var request = new RiakRestRequest(ToBucketUri(bucket), RiakConstants.Rest.HttpMethod.Put)
            {
                Body = properties.ToJsonString().ToRiakString(),
                ContentType = RiakConstants.ContentTypes.ApplicationJson
            };

            await _connection.RestRequest(_endPoint, request).ConfigureAwait(false);
        }

        private async Task SetPbcBucketProperties(string bucket, RiakBucketProperties properties)
        {
            if (!IsValidBucketOrKey(bucket))
            {
                throw new RiakException((uint)ResultCode.InvalidRequest, InvalidBucketErrorMessage, false);
            }

            var request = new RpbSetBucketReq { bucket = bucket.ToRiakString(), props = properties.ToMessage() };
            await _connection.PbcWriteRead(_endPoint, request, MessageCode.SetBucketResp).ConfigureAwait(false);
        }

        public async Task<bool> ResetBucketProperties(string bucket, bool useHttp = false)
        {
            if (!IsValidBucketOrKey(bucket))
            {
                throw new RiakException((uint)ResultCode.InvalidRequest, InvalidBucketErrorMessage, false);
            }

            var task = useHttp ? ResetHttpBucketProperties(bucket) : ResetPbcBucketProperties(bucket);

            await task.ConfigureAwait(false);
            return true;
        }

        private static string ToBucketPropsUri(string bucket)
        {
            return RiakConstants.Rest.Uri.BucketPropsFmt.Fmt(HttpUtility.UrlEncode(bucket));
        }

        private async Task ResetPbcBucketProperties(string bucket)
        {
            var request = new RpbResetBucketReq { bucket = bucket.ToRiakString() };
            await _connection.PbcWriteRead(_endPoint, request, MessageCode.ResetBucketResp);
        }

        private async Task ResetHttpBucketProperties(string bucket)
        {
            var request = new RiakRestRequest(ToBucketPropsUri(bucket), RiakConstants.Rest.HttpMethod.Delete);

            var result = await _connection.RestRequest(_endPoint, request).ConfigureAwait(false);

            switch (result.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    break;
                case HttpStatusCode.NotFound:
                    throw new RiakException((uint)ResultCode.NotFound, "Bucket {0} not found.".Fmt(bucket), false);
                default:
                    throw new RiakException((uint)ResultCode.InvalidResponse, "Unexpected Status Code: {0} ({1})".Fmt(result.StatusCode, (int)result.StatusCode), false);
            }
        }

        public Task<Either<RiakException, RiakIndexResult>> IndexGet(string bucket, string indexName, BigInteger value, RiakIndexGetOptions options = null)
        {
            return IndexGetEquals(bucket, indexName.ToIntegerKey(), value.ToString(), options);
        }

        public Task<Either<RiakException, RiakIndexResult>> IndexGet(string bucket, string indexName, string value, RiakIndexGetOptions options = null)
        {
            return IndexGetEquals(bucket, indexName.ToBinaryKey(), value, options);
        }

        public Task<Either<RiakException, RiakIndexResult>> IndexGet(string bucket, string indexName, BigInteger minValue, BigInteger maxValue, RiakIndexGetOptions options = null)
        {
            return IndexGetRange(bucket, indexName.ToIntegerKey(), minValue.ToString(), maxValue.ToString(), options);
        }

        public Task<Either<RiakException, RiakIndexResult>> IndexGet(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null)
        {
            return IndexGetRange(bucket, indexName.ToBinaryKey(), minValue, maxValue, options);
        }

        private static bool ReturnTerms(RiakIndexGetOptions options)
        {
            return options.ReturnTerms != null && options.ReturnTerms.Value;
        }

        private async Task<Either<RiakException, RiakIndexResult>> IndexGetRange(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null)
        {
            var message = new RpbIndexReq
            {
                bucket = bucket.ToRiakString(),
                index = indexName.ToRiakString(),
                qtype = RpbIndexReq.IndexQueryType.range,
                range_min = minValue.ToRiakString(),
                range_max = maxValue.ToRiakString()
            };

            options = options ?? new RiakIndexGetOptions();
            options.Populate(message);

            try
            {
                var result = await _connection.PbcWriteRead<RpbIndexReq, RpbIndexResp>(_endPoint, message).ConfigureAwait(false);
                var includeTerms = ReturnTerms(options);
                var riakIndexResult = new RiakIndexResult(includeTerms, result);
                return new Either<RiakException, RiakIndexResult>(riakIndexResult);
            }
            catch (RiakException riakException)
            {
                return new Either<RiakException, RiakIndexResult>(riakException);
            }
        }

        private async Task<Either<RiakException, RiakIndexResult>> IndexGetEquals(string bucket, string indexName, string value, RiakIndexGetOptions options = null)
        {
            var message = new RpbIndexReq
            {
                bucket = bucket.ToRiakString(),
                index = indexName.ToRiakString(),
                key = value.ToRiakString(),
                qtype = RpbIndexReq.IndexQueryType.eq
            };

            options = options ?? new RiakIndexGetOptions();
            options.Populate(message);

            try
            {
                var result = await _connection.PbcWriteRead<RpbIndexReq, RpbIndexResp>(_endPoint, message).ConfigureAwait(false);
                var includeTerms = ReturnTerms(options);
                var riakIndexResult = new RiakIndexResult(includeTerms, result);

                return new Either<RiakException, RiakIndexResult>(riakIndexResult);
            }
            catch (RiakException riakException)
            {
                return new Either<RiakException, RiakIndexResult>(riakException);
            }
        }

        public Task<Either<RiakException, RiakStreamedIndexResult>> StreamIndexGet(string bucket, string indexName, BigInteger value, RiakIndexGetOptions options = null)
        {
            return StreamIndexGetEquals(bucket, indexName.ToIntegerKey(), value.ToString(), options);
        }

        public Task<Either<RiakException, RiakStreamedIndexResult>> StreamIndexGet(string bucket, string indexName, string value, RiakIndexGetOptions options = null)
        {
            return StreamIndexGetEquals(bucket, indexName.ToBinaryKey(), value, options);
        }

        public Task<Either<RiakException, RiakStreamedIndexResult>> StreamIndexGet(string bucket, string indexName, BigInteger minValue, BigInteger maxValue, RiakIndexGetOptions options = null)
        {
            return StreamIndexGetRange(bucket, indexName.ToIntegerKey(), minValue.ToString(), maxValue.ToString(), options);
        }

        public Task<Either<RiakException, RiakStreamedIndexResult>> StreamIndexGet(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null)
        {
            return StreamIndexGetRange(bucket, indexName.ToBinaryKey(), minValue, maxValue, options);
        }

        private async Task<Either<RiakException, RiakStreamedIndexResult>> StreamIndexGetEquals(string bucket, string indexName, string value, RiakIndexGetOptions options = null)
        {
            var message = new RpbIndexReq
            {
                bucket = bucket.ToRiakString(),
                index = indexName.ToRiakString(),
                key = value.ToRiakString(),
                qtype = RpbIndexReq.IndexQueryType.eq,
                stream = true
            };

            options = options ?? new RiakIndexGetOptions();
            options.Populate(message);

            try
            {
                var result = _connection.PbcWriteStreamRead<RpbIndexReq, RpbIndexResp>(_endPoint, message,
                    lbr => !lbr.done);

                var includeTerms = ReturnTerms(options);
                var riakStreamedIndexResult = new RiakStreamedIndexResult(includeTerms, result);


                return new Either<RiakException, RiakStreamedIndexResult>(riakStreamedIndexResult);
            }
            catch (RiakException riakException)
            {
                return new Either<RiakException, RiakStreamedIndexResult>(riakException);
            }
        }

        private async Task<Either<RiakException, RiakStreamedIndexResult>> StreamIndexGetRange(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null)
        {
            var message = new RpbIndexReq
            {
                bucket = bucket.ToRiakString(),
                index = indexName.ToRiakString(),
                qtype = RpbIndexReq.IndexQueryType.range,
                range_min = minValue.ToRiakString(),
                range_max = maxValue.ToRiakString(),
                stream = true
            };

            options = options ?? new RiakIndexGetOptions();
            options.Populate(message);

            try
            {
                var result = _connection.PbcWriteStreamRead<RpbIndexReq, RpbIndexResp>(_endPoint, message, lbr => !lbr.done);
                var includeTerms = ReturnTerms(options);
                var riakStreamedIndexResult = new RiakStreamedIndexResult(includeTerms, result);

                return new Either<RiakException, RiakStreamedIndexResult>(riakStreamedIndexResult);
            }
            catch (RiakException riakException)
            {
                return new Either<RiakException, RiakStreamedIndexResult>(riakException);
            }
        }

        public IObservable<Either<RiakException, string>> ListKeysFromIndex(string bucket)
        {
            var observables = Observable.Create<Either<RiakException, string>>(async obs =>
            {
                try
                {
                    var result = await IndexGet(bucket, RiakConstants.SystemIndexKeys.RiakBucketIndex, bucket).ConfigureAwait(false);

                    if (result.IsLeft)
                    {
                        obs.OnNext(new Either<RiakException, string>(result.Left));
                    }
                    else
                    {
                        var keys = result.Right.IndexKeyTerms.Select(ikt => ikt.Key);

                        foreach (var key in keys)
                        {
                            obs.OnNext(new Either<RiakException, string>(key));
                        }
                    }
                    obs.OnCompleted();
                }
                catch (Exception exception)
                {
                    obs.OnError(exception);
                }
                return Disposable.Empty;
            });

            return observables;
        }

        public IObservable<Either<RiakException, RiakObject>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks)
        {
            var observables = Observable.Create<Either<RiakException, RiakObject>>(async obs =>
            {
                try
                {
                    System.Diagnostics.Debug.Assert(riakLinks.Count > 0, "Link walking requires at least one link");

                    var input = new RiakBucketKeyInput()
                        .Add(riakObject.Bucket, riakObject.Key);

                    var query = new RiakMapReduceQuery()
                        .Inputs(input);

                    var lastLink = riakLinks.Last();

                    foreach (var riakLink in riakLinks)
                    {
                        var link = riakLink;
                        var keep = ReferenceEquals(link, lastLink);

                        query.Link(l => l.FromRiakLink(link).Keep(keep));
                    }

                    var result = await MapReduce(query).ConfigureAwait(false);

                    var linkResults = result.PhaseResults
                        .GroupBy(r => r.Phase)
                        .Where(g => g.Key == riakLinks.Count - 1);

                    var linkResultStrings = linkResults
                        .SelectMany(lr => lr.ToList(), (lr, r) => new { lr, r })
                        .SelectMany(@t => @t.r.Values, (@t, s) => s.FromRiakString());

                    //var linkResultStrings = linkResults.SelectMany(g => g.Select(r => r.Values.Value.FromRiakString()));
                    var rawLinks = linkResultStrings
                        .SelectMany(RiakLink.ParseArrayFromJsonString)
                        .Distinct();

                    var oids = rawLinks
                        .Select(l => new RiakObjectId(l.Bucket, l.Key))
                        .ToList();

                    var source = Get(oids, new RiakGetOptions());

                    return source.Subscribe(obs.OnNext,
                            obs.OnError,
                            obs.OnCompleted);
                }
                catch (Exception exception)
                {
                    obs.OnError(exception);
                }
                return Disposable.Empty;
            });

            return observables;
        }

        public async Task<Either<RiakException, RiakServerInfo>> GetServerInfo()
        {
            try
            {
                var result = await _connection.PbcWriteRead<RpbGetServerInfoResp>(_endPoint, MessageCode.GetServerInfoReq)
                            .ConfigureAwait(false);

                return new Either<RiakException, RiakServerInfo>(new RiakServerInfo(result));
            }
            catch (RiakException riakException)
            {
                return new Either<RiakException, RiakServerInfo>(riakException);
            }
        }

        public void Dispose()
        {
        }
    }
}