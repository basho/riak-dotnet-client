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

using CorrugatedIron.Comms;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Models.Rest;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

using System.Threading.Tasks;

namespace CorrugatedIron
{

    internal class RiakAsyncClient : IRiakAsyncClient
    {
        private const string ListKeysWarning = "*** [CI] -> ListKeys is an expensive operation and should not be used in Production scenarios. ***";
        private const string InvalidBucketErrorMessage = "Bucket cannot be blank or contain forward-slashes";
        private const string InvalidKeyErrorMessage = "Key cannot be blank or contain forward-slashes";
        
        private readonly IRiakEndPoint _endPoint;
        private readonly IRiakConnection _batchConnection;
        
        public int RetryCount { get; set; }

        internal RiakAsyncClient(IRiakEndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        public Task<RiakResult> Ping()
        {
            return UseConnection(conn => conn.PbcWriteRead(MessageCode.PingReq, MessageCode.PingResp));
        }

        public Task<RiakResult<RiakObject>> Get(string bucket, string key, RiakGetOptions options = null)
        {
            if (!IsValidBucketOrKey(bucket))
            {
                return TaskResult(RiakResult<RiakObject>.Error(ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
            }
            
            if (!IsValidBucketOrKey(key))
            {
                return TaskResult(RiakResult<RiakObject>.Error(ResultCode.InvalidRequest, InvalidKeyErrorMessage, false));
            }
            
            var request = new RpbGetReq { bucket = bucket.ToRiakString(), key = key.ToRiakString() };
            options = options ?? new RiakGetOptions();
            options.Populate(request);
            
            return UseConnection(conn => conn.PbcWriteRead<RpbGetReq, RpbGetResp>(request))
                .ContinueWith((Task<RiakResult<RpbGetResp>> finishedTask) => {
                    var result = finishedTask.Result;
                    if(!result.IsSuccess)
                    {
                        return RiakResult<RiakObject>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
                    }
                    
                    if(result.Value.vclock == null)
                    {
                        return RiakResult<RiakObject>.Error(ResultCode.NotFound, "Unable to find value in Riak", false);
                    }
                    
                    var o = new RiakObject(bucket, key, result.Value.content, result.Value.vclock);
                    return RiakResult<RiakObject>.Success(o);
                });
        }

        public Task<RiakResult<RiakObject>> Get(string bucket, string key, uint rVal = RiakConstants.Defaults.RVal)
        {
            var options = new RiakGetOptions().SetR(rVal);
            return Get(bucket, key, options);
        }

        public Task<RiakResult<RiakObject>> Get(RiakObjectId objectId, uint rVal = RiakConstants.Defaults.RVal)
        {
            return Get(objectId.Bucket, objectId.Key, rVal);
        }

        public Task<IEnumerable<RiakResult<RiakObject>>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, RiakGetOptions options = null)
        {
            bucketKeyPairs = bucketKeyPairs.ToList();
            options = options ?? new RiakGetOptions();
            
            return UseConnection(conn => {
                return AfterAll(bucketKeyPairs.Select(bkp => {

                    // modified closure FTW
                    var bk = bkp;
                    if (!IsValidBucketOrKey(bk.Bucket))
                    {
                        return TaskResult(RiakResult<RpbGetResp>.Error(ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
                    }
                    if (!IsValidBucketOrKey(bk.Key))
                    {
                        return TaskResult(RiakResult<RpbGetResp>.Error(ResultCode.InvalidRequest, InvalidKeyErrorMessage, false));
                    }
                    
                    var req = new RpbGetReq { bucket = bk.Bucket.ToRiakString(), key = bk.Key.ToRiakString() };
                    options.Populate(req);
                    
                    return conn.PbcWriteRead<RpbGetReq, RpbGetResp>(req);
                })).ContinueWith((Task<IEnumerable<RiakResult<RpbGetResp>>> finishedTask) => {
                    return RiakResult<IEnumerable<RiakResult<RpbGetResp>>>.Success(finishedTask.Result); 
                });
            }).ContinueWith((Task<RiakResult<IEnumerable<RiakResult<RpbGetResp>>>> finishedTask) => {

                // zip up results
                var results = RiakResult<IEnumerable<RiakResult<RpbGetResp>>>.Success(finishedTask.Result.Value);
                return results.Value.Zip(bucketKeyPairs, Tuple.Create).Select(result => {
                    if(!result.Item1.IsSuccess)
                    {
                        return RiakResult<RiakObject>.Error(result.Item1.ResultCode, result.Item1.ErrorMessage, result.Item1.NodeOffline);
                    }
                    if(result.Item1.Value.vclock == null)
                    {
                        return RiakResult<RiakObject>.Error(ResultCode.NotFound, "Unable to find value in Riak", false);
                    }
                    
                    var o = new RiakObject(result.Item2.Bucket, result.Item2.Key, result.Item1.Value.content.First(), result.Item1.Value.vclock);
                    if(result.Item1.Value.content.Count > 1)
                    {
                        o.Siblings = result.Item1.Value.content.Select(
                            c => new RiakObject(result.Item2.Bucket, result.Item2.Key, c, result.Item1.Value.vclock)).ToList();
                    }
                    
                    return RiakResult<RiakObject>.Success(o);
                });
            });
        }

        public Task<IEnumerable<RiakResult<RiakObject>>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, uint rVal = RiakConstants.Defaults.RVal)
        {
            var options = new RiakGetOptions().SetR(rVal);
            return Get(bucketKeyPairs, options);
        }

        public Task<RiakResult<RiakObject>> Put(RiakObject value, RiakPutOptions options = null)
        {
            if (!IsValidBucketOrKey(value.Bucket))
            {
                return TaskResult(RiakResult<RiakObject>.Error(ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
            }
            
            if (!IsValidBucketOrKey(value.Key))
            {
                return TaskResult(RiakResult<RiakObject>.Error(ResultCode.InvalidRequest, InvalidKeyErrorMessage, false));
            }

            var request = value.ToMessage();
            options = options ?? new RiakPutOptions();
            options.Populate(request);
            
            return UseConnection(conn => conn.PbcWriteRead<RpbPutReq, RpbPutResp>(request))
                .ContinueWith((Task<RiakResult<RpbPutResp>> finishedTask) => {
                    var result = finishedTask.Result;

                    if(!result.IsSuccess)
                    {
                        return RiakResult<RiakObject>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
                    }
                    
                    var finalResult = options.ReturnBody
                        ? new RiakObject(value.Bucket, value.Key, result.Value.content.First(), result.Value.vclock)
                            : value;
                    
                    if(options.ReturnBody && result.Value.content.Count > 1)
                    {
                        finalResult.Siblings = result.Value.content.Select(
                            c => new RiakObject(value.Bucket, value.Key, c, result.Value.vclock)).ToList();
                    }
                    
                    return RiakResult<RiakObject>.Success(finalResult);
                });
        }

        public Task<IEnumerable<RiakResult<RiakObject>>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null)
        {
            options = options ?? new RiakPutOptions();
            
            return UseConnection(conn => {
                return AfterAll(values.Select(v => {
                    if (!IsValidBucketOrKey(v.Bucket))
                    {
                        return TaskResult(RiakResult<RpbPutResp>.Error(ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
                    }
                    
                    if (!IsValidBucketOrKey(v.Key))
                    {
                        return TaskResult(RiakResult<RpbPutResp>.Error(ResultCode.InvalidRequest, InvalidKeyErrorMessage, false));
                    }
                    
                    var msg = v.ToMessage();
                    options.Populate(msg);
                    
                    return conn.PbcWriteRead<RpbPutReq, RpbPutResp>(msg);
                })).ContinueWith((Task<IEnumerable<RiakResult<RpbPutResp>>> finishedTask) => {
                    return RiakResult<IEnumerable<RiakResult<RpbPutResp>>>.Success(finishedTask.Result);
                });
            }).ContinueWith((Task<RiakResult<IEnumerable<RiakResult<RpbPutResp>>>> finishedTask) => {
                var results = finishedTask.Result;
                return results.Value.Zip(values, Tuple.Create).Select(t => {
                    if(t.Item1.IsSuccess)
                    {
                        var finalResult = options.ReturnBody
                            ? new RiakObject(t.Item2.Bucket, t.Item2.Key, t.Item1.Value.content.First(), t.Item1.Value.vclock)
                                : t.Item2;
                        
                        if(options.ReturnBody && t.Item1.Value.content.Count > 1)
                        {
                            finalResult.Siblings = t.Item1.Value.content.Select(
                                c => new RiakObject(t.Item2.Bucket, t.Item2.Key, c, t.Item1.Value.vclock)).ToList();
                        }
                        
                        return RiakResult<RiakObject>.Success(finalResult);
                    }

                    return RiakResult<RiakObject>.Error(t.Item1.ResultCode, t.Item1.ErrorMessage, t.Item1.NodeOffline);
                });
            });
        }

        public Task<RiakResult> Delete(string bucket, string key, RiakDeleteOptions options = null)
        {
            if (!IsValidBucketOrKey(bucket))
            {
                return TaskResult(RiakResult.Error(ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
            }
            
            if (!IsValidBucketOrKey(key))
            {
                return TaskResult(RiakResult.Error(ResultCode.InvalidRequest, InvalidKeyErrorMessage, false));
            }

            var request = new RpbDelReq { bucket = bucket.ToRiakString(), key = key.ToRiakString() };
            options = options ?? new RiakDeleteOptions();
            options.Populate(request);
            return UseConnection(conn => conn.PbcWriteRead(request, MessageCode.DelResp));
        }

        private Task<RiakResult<IEnumerable<RiakResult>>> Delete(IRiakConnection conn,
                                                                 IEnumerable<RiakObjectId> objectIds,
                                                                 RiakDeleteOptions options = null)
        {
            options = options ?? new RiakDeleteOptions();
            return AfterAll(objectIds.Select(id => {
                if (!IsValidBucketOrKey(id.Bucket))
                {
                    return TaskResult(RiakResult.Error(ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
                }
                
                if (!IsValidBucketOrKey(id.Key))
                {
                    return TaskResult(RiakResult.Error(ResultCode.InvalidRequest, InvalidKeyErrorMessage, false));
                }
                
                var req = new RpbDelReq { bucket = id.Bucket.ToRiakString(), key = id.Key.ToRiakString() };
                options.Populate(req);
                return conn.PbcWriteRead(req, MessageCode.DelResp);
            })).ContinueWith((Task<IEnumerable<RiakResult>> finishedTask) => {
                return RiakResult<IEnumerable<RiakResult>>.Success(finishedTask.Result);
            });
        }

        public Task<RiakResult> Delete(RiakObjectId objectId, RiakDeleteOptions options = null)
        {
            return Delete(objectId.Bucket, objectId.Key);
        }

        public Task<IEnumerable<RiakResult>> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null)
        {
            return UseConnection(conn => Delete(conn, objectIds, options))
                .ContinueWith((Task<RiakResult<IEnumerable<RiakResult>>> finishedTask) => {
                    return finishedTask.Result.Value;
                });
        }

        public Task<IEnumerable<RiakResult>> DeleteBucket(string bucket, RiakDeleteOptions deleteOptions)
        {
            return UseConnection(conn => {
                return ListKeys(conn, bucket)
                    .ContinueWith((Task<RiakResult<IEnumerable<string>>> finishedListingKeys) => {
                        var keyResults = finishedListingKeys.Result;
                        if (keyResults.IsSuccess)
                        {
                            var objectIds = keyResults.Value.Select(key => new RiakObjectId(bucket, key)).ToList();
                            return Delete(conn, objectIds, deleteOptions);
                        }
                        return TaskResult(RiakResult<IEnumerable<RiakResult>>.Error(keyResults.ResultCode, keyResults.ErrorMessage, keyResults.NodeOffline));
                    }).Unwrap();
            }).ContinueWith((Task<RiakResult<IEnumerable<RiakResult>>> finishedTask) => {
                return finishedTask.Result.Value;
            });
        }

        public Task<IEnumerable<RiakResult>> DeleteBucket(string bucket, uint rwVal = RiakConstants.Defaults.RVal)
        {
            var options = new RiakDeleteOptions();
            options.SetRw(rwVal);
            return DeleteBucket(bucket, options);
        }

        public Task<RiakResult<RiakMapReduceResult>> MapReduce(RiakMapReduceQuery query)
        {
            var request = query.ToMessage();
            return UseConnection(conn => conn.PbcWriteRead<RpbMapRedReq, RpbMapRedResp>(request, r => r.IsSuccess && !r.Value.done))
                .ContinueWith((Task<RiakResult<IEnumerable<RiakResult<RpbMapRedResp>>>> finishedTask) => {
                    var result = finishedTask.Result;
                    if(result.IsSuccess)
                    {
                        return RiakResult<RiakMapReduceResult>.Success(new RiakMapReduceResult(result.Value));
                    }
                    return RiakResult<RiakMapReduceResult>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
                });
        }

        public Task<RiakResult<RiakStreamedMapReduceResult>> StreamMapReduce(RiakMapReduceQuery query)
        {
            var request = query.ToMessage();
            return UseDelayedConnection((conn, onFinish) => {
                return conn.PbcWriteStreamRead<RpbMapRedReq, RpbMapRedResp>(request, r => r.IsSuccess && !r.Value.done, onFinish);
            }).ContinueWith((Task<RiakResult<IEnumerable<RiakResult<RpbMapRedResp>>>> finishedTask) => {
                var result = finishedTask.Result;
                if(result.IsSuccess)
                {
                    return RiakResult<RiakStreamedMapReduceResult>.Success(new RiakStreamedMapReduceResult(result.Value));
                }
                return RiakResult<RiakStreamedMapReduceResult>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
            });
        }

        public Task<RiakResult<IEnumerable<string>>>  ListBuckets()
        {
            return UseConnection(conn => conn.PbcWriteRead<RpbListBucketsResp>(MessageCode.ListBucketsReq))
                .ContinueWith((Task<RiakResult<RpbListBucketsResp>> finishedTask) => {
                    var result = finishedTask.Result;
                    if(result.IsSuccess)
                    {
                        var buckets = result.Value.buckets.Select(b => b.FromRiakString());
                        return RiakResult<IEnumerable<string>>.Success(buckets.ToList());
                    }
                    return RiakResult<IEnumerable<string>>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
                });
        }

        public Task<RiakResult<IEnumerable<string>>> ListKeys(string bucket)
        {
            return UseConnection(conn => ListKeys(conn, bucket));
        }

        public Task<RiakResult<IEnumerable<string>>> StreamListKeys(string bucket)
        {
            System.Diagnostics.Debug.Write(ListKeysWarning);
            System.Diagnostics.Trace.TraceWarning(ListKeysWarning);
            Console.WriteLine(ListKeysWarning);
            
            var lkReq = new RpbListKeysReq { bucket = bucket.ToRiakString() };
            return UseDelayedConnection((conn, onFinish) => {
                return conn.PbcWriteStreamRead<RpbListKeysReq, RpbListKeysResp>(lkReq, lkr => lkr.IsSuccess && !lkr.Value.done, onFinish);
            }).ContinueWith((Task<RiakResult<IEnumerable<RiakResult<RpbListKeysResp>>>> finishedTask) => {
                var result = finishedTask.Result;
                if(result.IsSuccess)
                {
                    var keys = result.Value.Where(r => r.IsSuccess).SelectMany(r => r.Value.keys).Select(k => k.FromRiakString());
                    return RiakResult<IEnumerable<string>>.Success(keys);
                }
                return RiakResult<IEnumerable<string>>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
            });
        }

        public Task<RiakResult<RiakBucketProperties>> GetBucketProperties(string bucket, bool extended = false)
        {
            // bucket names cannot have slashes in the names, the REST interface doesn't like it at all
            if (!IsValidBucketOrKey(bucket))
            {
                return TaskResult(RiakResult<RiakBucketProperties>.Error(ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
            }
            
            if(extended)
            {
                var request = new RiakRestRequest(ToBucketUri(bucket), RiakConstants.Rest.HttpMethod.Get)
                    .AddQueryParam(RiakConstants.Rest.QueryParameters.Bucket.GetPropertiesKey,
                                   RiakConstants.Rest.QueryParameters.Bucket.GetPropertiesValue);
                
                return UseConnection(conn => conn.RestRequest(request))
                    .ContinueWith((Task<RiakResult<RiakRestResponse>> finishedTask) => {
                        var result = finishedTask.Result;
                        if(result.IsSuccess)
                        {
                            if(result.Value.StatusCode == HttpStatusCode.OK)
                            {
                                var response = new RiakBucketProperties(result.Value);
                                return RiakResult<RiakBucketProperties>.Success(response);
                            }
                            return RiakResult<RiakBucketProperties>.Error(ResultCode.InvalidResponse,
                                                                          "Unexpected Status Code: {0} ({1})".Fmt(result.Value.StatusCode, (int)result.Value.StatusCode), false);
                        }
                        return RiakResult<RiakBucketProperties>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
                    });
            }
            else
            {
                var bpReq = new RpbGetBucketReq { bucket = bucket.ToRiakString() };
                return UseConnection(conn => conn.PbcWriteRead<RpbGetBucketReq, RpbGetBucketResp>(bpReq))
                    .ContinueWith((Task<RiakResult<RpbGetBucketResp>> finishedTask) => {
                        var result = finishedTask.Result;
                        if(result.IsSuccess)
                        {
                            var props = new RiakBucketProperties(result.Value.props);
                            return RiakResult<RiakBucketProperties>.Success(props);
                        }
                        return RiakResult<RiakBucketProperties>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
                    });
            }
        }

        public Task<RiakResult> SetBucketProperties(string bucket, RiakBucketProperties properties)
        {
            if (!IsValidBucketOrKey(bucket))
            {
                return TaskResult(RiakResult.Error(ResultCode.InvalidRequest, InvalidBucketErrorMessage, false));
            }
            
            if(properties.CanUsePbc)
            {
                var request = new RpbSetBucketReq { bucket = bucket.ToRiakString(), props = properties.ToMessage() };
                return UseConnection(conn => conn.PbcWriteRead(request, MessageCode.SetBucketResp));
            }
            else
            {
                var request = new RiakRestRequest(ToBucketUri(bucket), RiakConstants.Rest.HttpMethod.Put)
                {
                    Body = properties.ToJsonString().ToRiakString(),
                    ContentType = RiakConstants.ContentTypes.ApplicationJson
                };
                
                return UseConnection(conn => conn.RestRequest(request))
                    .ContinueWith((Task<RiakResult<RiakRestResponse>> finishedTask) => {
                        var result = finishedTask.Result;
                        if(result.IsSuccess && result.Value.StatusCode != HttpStatusCode.NoContent)
                        {
                            return RiakResult.Error(ResultCode.InvalidResponse, "Unexpected Status Code: {0} ({1})".Fmt(result.Value.StatusCode, (int)result.Value.StatusCode), result.NodeOffline);
                        }
                        return result;
                    });
            }
        }

        public Task<RiakResult> ResetBucketProperties(string bucket)
        {
            var request = new RiakRestRequest(ToBucketPropsUri(bucket), RiakConstants.Rest.HttpMethod.Delete);
            return UseConnection(conn => conn.RestRequest(request))
                .ContinueWith((Task<RiakResult<RiakRestResponse>> finishedTask) => {
                    var result = finishedTask.Result;
                    if(result.IsSuccess)
                    {
                        switch (result.Value.StatusCode)
                        {
                            case HttpStatusCode.NoContent:
                                return result;
                            case HttpStatusCode.NotFound:
                                return RiakResult.Error(ResultCode.NotFound, "Bucket {0} not found.".Fmt(bucket), false);
                            default:
                                return RiakResult.Error(ResultCode.InvalidResponse, "Unexpected Status Code: {0} ({1})".Fmt(result.Value.StatusCode, (int)result.Value.StatusCode), result.NodeOffline);
                        }
                    }
                    return result;
                });
        }

        public Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, int value)
        {
            return IndexGetEquals(bucket, indexName.ToIntegerKey(), value.ToString());
        }

        public Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, string value)
        {
            return IndexGetEquals(bucket, indexName.ToBinaryKey(), value);
        }

        public Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, int minValue, int maxValue)
        {
            return IndexGetRange(bucket, indexName.ToIntegerKey(), minValue.ToString(), maxValue.ToString());
        }

        public Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, string minValue, string maxValue)
        {
            return IndexGetRange(bucket, indexName.ToBinaryKey(), minValue, maxValue);
        }

        private Task<RiakResult<IList<string>>> IndexGetRange(string bucket, string indexName, string minValue, string maxValue)
        {
            var message = new RpbIndexReq
            {
                bucket = bucket.ToRiakString(),
                index = indexName.ToRiakString(),
                qtype = RpbIndexReq.IndexQueryType.range,
                range_min = minValue.ToRiakString(),
                range_max = maxValue.ToRiakString()
            };
            
            return UseConnection(conn => conn.PbcWriteRead<RpbIndexReq, RpbIndexResp>(message))
                .ContinueWith((Task<RiakResult<RpbIndexResp>> finishedTask) => {
                    var result = finishedTask.Result;
                    if (result.IsSuccess)
                    {
                        return RiakResult<IList<string>>.Success(result.Value.keys.Select(k => k.FromRiakString()).ToList());
                    }
                    return RiakResult<IList<string>>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
                });
        }

        private Task<RiakResult<IList<string>>> IndexGetEquals(string bucket, string indexName, string value)
        {
            var message = new RpbIndexReq
            {
                bucket = bucket.ToRiakString(),
                index = indexName.ToRiakString(),
                key = value.ToRiakString(),
                qtype = RpbIndexReq.IndexQueryType.eq
            };
            
            return UseConnection(conn => conn.PbcWriteRead<RpbIndexReq, RpbIndexResp>(message))
                .ContinueWith((Task<RiakResult<RpbIndexResp>> finishedTask) => {
                    var result = finishedTask.Result;
                    if (result.IsSuccess)
                    {
                        return RiakResult<IList<string>>.Success(result.Value.keys.Select(k => k.FromRiakString()).ToList());
                    }
                    return RiakResult<IList<string>>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
                });
        }

        public Task<RiakResult<IList<RiakObject>>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks)
        {
            System.Diagnostics.Debug.Assert(riakLinks.Count > 0, "Link walking requires at least one link");
            
            var input = new RiakBucketKeyInput();
            input.AddBucketKey(riakObject.Bucket, riakObject.Key);
            
            var query = new RiakMapReduceQuery()
                .Inputs(input);
            
            var lastLink = riakLinks.Last();
            
            foreach(var riakLink in riakLinks)
            {
                var link = riakLink;
                var keep = ReferenceEquals(link, lastLink);
                query.Link(l => l.FromRiakLink(link).Keep(keep));
            }
            
            return MapReduce(query)
                .ContinueWith((Task<RiakResult<RiakMapReduceResult>> finishedTask) => {
                    var result = finishedTask.Result;
                    if(result.IsSuccess)
                    {
                        var linkResults = result.Value.PhaseResults.GroupBy(r => r.Phase).Where(g => g.Key == riakLinks.Count - 1);
                        var linkResultStrings = linkResults.SelectMany(lr => lr.ToList(), (lr, r) => new { lr, r })
                            .SelectMany(@t => @t.r.Values, (@t, s) => s.FromRiakString());
                        
                        //var linkResultStrings = linkResults.SelectMany(g => g.Select(r => r.Values.Value.FromRiakString()));
                        var rawLinks = linkResultStrings.SelectMany(RiakLink.ParseArrayFromJsonString).Distinct();
                        var oids = rawLinks.Select(l => new RiakObjectId(l.Bucket, l.Key)).ToList();
                        
                        return Get(oids, new RiakGetOptions())
                            .ContinueWith((Task<IEnumerable<RiakResult<RiakObject>>> getTask) => {
                                var objects = getTask.Result;

                                // FIXME
                                // we could be discarding results here. Not good?
                                // This really should be a multi-phase map/reduce
                                return RiakResult<IList<RiakObject>>.Success(objects.Where(r => r.IsSuccess).Select(r => r.Value).ToList());
                            });
                    }
                    return TaskResult(RiakResult<IList<RiakObject>>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline));
                }).Unwrap();
        }

        public Task<RiakResult<RiakServerInfo>>  GetServerInfo()
        {
            return UseConnection(conn => conn.PbcWriteRead<RpbGetServerInfoResp>(MessageCode.GetServerInfoReq))
                .ContinueWith((Task<RiakResult<RpbGetServerInfoResp>> finishedTask) => {
                    var result = finishedTask.Result;
                    if(result.IsSuccess)
                    {
                        return RiakResult<RiakServerInfo>.Success(new RiakServerInfo(result.Value));
                    }
                    return RiakResult<RiakServerInfo>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
                });
        }

        public Task<RiakResult<IList<string>>> ListKeysFromIndex(string bucket)
        {
            return IndexGet(bucket, RiakConstants.SystemIndexKeys.RiakBucketIndex, bucket);
        }

        public Task<RiakResult<RiakSearchResult>> Search(RiakSearchRequest search)
        {
            var request = search.ToMessage();
            return UseConnection(conn => conn.PbcWriteRead<RpbSearchQueryReq, RpbSearchQueryResp>(request))
                .ContinueWith((Task<RiakResult<RpbSearchQueryResp>> finishedTask) => {
                    var result = finishedTask.Result;
                    if (result.IsSuccess)
                    {
                        return RiakResult<RiakSearchResult>.Success(new RiakSearchResult(result.Value));
                    }
                    
                    return RiakResult<RiakSearchResult>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
                });
        }

        public Task Batch(Action<IRiakBatchClient> batchAction)
        {
            throw new NotImplementedException();
        }

        public Task Batch(Action<IRiakBatchAsyncClient> batchFunc)
        {
            throw new NotImplementedException();
        }

        public Task<T> Batch<T>(Func<IRiakBatchClient, T> batchFunc)
        {
            throw new NotImplementedException();
            /*
            var funResult = default(T);

            // no idea what this is
            Func<IRiakConnection, Action, RiakResult<IEnumerable<RiakResult<object>>>> helperBatchFun = ((conn, onFinish) =>
            {
                try
                {
                    funResult = batchFun(new RiakClient(conn));
                    return RiakResult<IEnumerable<RiakResult<object>>>.Success(null);
                }
                catch(Exception ex)
                {
                    return RiakResult<IEnumerable<RiakResult<object>>>.Error(ResultCode.BatchException, "{0}\n{1}".Fmt(ex.Message, ex.StackTrace), true);
                }
                finally
                {
                    onFinish();
                }
            });
            
            var result = _endPoint.UseDelayedConnection(helperBatchFun, RetryCount);
            
            if(!result.IsSuccess && result.ResultCode == ResultCode.BatchException)
            {
                throw new Exception(result.ErrorMessage);
            }
            
            return funResult;
            */
        }

        private Task<RiakResult<IEnumerable<string>>> ListKeys(IRiakConnection conn, string bucket)
        {
            System.Diagnostics.Debug.Write(ListKeysWarning);
            System.Diagnostics.Trace.TraceWarning(ListKeysWarning);
            Console.WriteLine(ListKeysWarning);

            // start listing keys
            var lkReq = new RpbListKeysReq { bucket = bucket.ToRiakString() };
            return conn.PbcWriteRead<RpbListKeysReq, RpbListKeysResp>(lkReq, lkr => lkr.IsSuccess && !lkr.Value.done)
                .ContinueWith((Task<RiakResult<IEnumerable<RiakResult<RpbListKeysResp>>>> finishedTask) => {
                    var result = finishedTask.Result;
                    if(result.IsSuccess)
                    {
                        var keys = result.Value.Where(r => r.IsSuccess).SelectMany(r => r.Value.keys).Select(k => k.FromRiakString()).Distinct().ToList();
                        return RiakResult<IEnumerable<string>>.Success(keys);
                    }
                    return RiakResult<IEnumerable<string>>.Error(result.ResultCode, result.ErrorMessage, result.NodeOffline);
                });
        }


        // using connections etc...

        private Task<RiakResult> UseConnection(Func<IRiakConnection, Task<RiakResult>> op)
        {
            return _batchConnection != null ? op(_batchConnection) : _endPoint.UseConnection(op, RetryCount);
        }
        
        private Task<RiakResult<TResult>> UseConnection<TResult>(Func<IRiakConnection, Task<RiakResult<TResult>>> op)
        {
            return _batchConnection != null ? op(_batchConnection) : _endPoint.UseConnection(op, RetryCount);
        }

        private Task<RiakResult<IEnumerable<RiakResult<TResult>>>> UseDelayedConnection<TResult>(
            Func<IRiakConnection, Action, Task<RiakResult<IEnumerable<RiakResult<TResult>>>>> op)
        {
            return _batchConnection != null
                ? op(_batchConnection, () => { })
                    : _endPoint.UseDelayedConnection(op, RetryCount);
        }

        
        private string ToBucketUri(string bucket)
        {
            return "{0}/{1}".Fmt(RiakConstants.Rest.Uri.RiakRoot, HttpUtility.UrlEncode(bucket));
        }
        
        private string ToBucketPropsUri(string bucket)
        {
            return RiakConstants.Rest.Uri.BucketPropsFmt.Fmt(HttpUtility.UrlEncode(bucket));
        }
        
        private bool IsValidBucketOrKey(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && !value.Contains('/');
        }

        // wrap a task result
        private Task<T> TaskResult<T>(T result)
        {
            var source = new TaskCompletionSource<T>();
            source.SetResult(result);
            return source.Task;
        }

        // wait for tasks to complete
        private Task<IEnumerable<T>> AfterAll<T>(IEnumerable<Task<T>> tasks)
        {
            var source = new TaskCompletionSource<IEnumerable<T>>();
            var leftToComplete = tasks.Count();

            // complete all tasks
            foreach (var task in tasks)
            {
                task.ContinueWith((Task<T> t) => {
                    leftToComplete--;
                    if (leftToComplete == 0)
                    {
                        source.SetResult(tasks.Select(tt => tt.Result));
                    }
                });
            }

            // completed task
            return source.Task;
        }




        // -----------------------------------------------------------------------------------------------------------
        // All functions below are marked as obsolete
        // -----------------------------------------------------------------------------------------------------------

        public void Ping(Action<RiakResult> callback)
        {
            ExecAsync(Ping(), callback);
        }

        public void Get(string bucket, string key, Action<RiakResult<RiakObject>> callback, uint rVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(Get(bucket, key, rVal), callback);
        }

        public void Get(RiakObjectId objectId, Action<RiakResult<RiakObject>> callback, uint rVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(Get(objectId, rVal), callback);
        }

        public void Get(IEnumerable<RiakObjectId> bucketKeyPairs, Action<IEnumerable<RiakResult<RiakObject>>> callback, uint rVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(Get(bucketKeyPairs, rVal), callback);
        }

        public void Put(IEnumerable<RiakObject> values, Action<IEnumerable<RiakResult<RiakObject>>> callback, RiakPutOptions options)
        {
            ExecAsync(Put(values, options), callback);
        }

        public void Put(RiakObject value, Action<RiakResult<RiakObject>> callback, RiakPutOptions options)
        {
            ExecAsync(Put(value, options), callback);
        }

        public void Delete(string bucket, string key, Action<RiakResult> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(Delete(bucket, key, options), callback);
        }

        public void Delete(RiakObjectId objectId, Action<RiakResult> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(Delete(objectId, options), callback);
        }

        public void Delete(IEnumerable<RiakObjectId> objectIds, Action<IEnumerable<RiakResult>> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(Delete(objectIds, options), callback);
        }

        public void DeleteBucket(string bucket, Action<IEnumerable<RiakResult>> callback, uint rwVal = RiakConstants.Defaults.RVal)
        {         
            ExecAsync(DeleteBucket(bucket, rwVal), callback);
        }

        public void MapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakMapReduceResult>> callback)
        {
            ExecAsync(MapReduce(query), callback);
        }

        public void StreamMapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakStreamedMapReduceResult>> callback)
        {
            ExecAsync(StreamMapReduce(query), callback);
        }

        public void ListBuckets(Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(ListBuckets(), callback);
        }

        public void ListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(ListKeys(bucket), callback);
        }

        public void StreamListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(StreamListKeys(bucket), callback);
        }

        public void GetBucketProperties(string bucket, Action<RiakResult<RiakBucketProperties>> callback, bool extended = false)
        {
            ExecAsync(GetBucketProperties(bucket, extended), callback);
        }

        public void SetBucketProperties(string bucket, RiakBucketProperties properties, Action<RiakResult> callback)
        {
            ExecAsync(SetBucketProperties(bucket, properties), callback);
        }

        public void WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks, Action<RiakResult<IList<RiakObject>>> callback)
        {
            ExecAsync(WalkLinks(riakObject, riakLinks), callback);
        }

        public void GetServerInfo(Action<RiakResult<RiakServerInfo>> callback)
        {
            ExecAsync(GetServerInfo(), callback);
        }

        private void ExecAsync<T>(Task<T> getResult, Action<T> callback)
        {
            getResult.ContinueWith((Task<T> task) => {
                callback(task.Result);
            });
        }
    }
}