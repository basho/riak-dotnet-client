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

using System;
using CorrugatedIron.Containers;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Util;
using System.Collections.Generic;
using System.Numerics;

namespace CorrugatedIron
{
    public interface IRiakBatchClient
    {
        int RetryCount { get; set; }
        RiakResult Ping();

        RiakResult<RiakObject> Get(RiakObjectId objectId, RiakGetOptions options = null);
        RiakResult<RiakObject> Get(string bucket, string key, RiakGetOptions options = null);
        IEnumerable<RiakResult<RiakObject>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, RiakGetOptions options = null);
        
        Tuple<RiakResult<RiakObject>, long?> IncrementCounter(string bucket, string counter, long amount, RiakCounterUpdateOptions options = null);
        Tuple<RiakResult<RiakObject>, long?> GetCounter(string bucket, string counter, RiakCounterGetOptions options = null);

        RiakResult<RiakObject> Put(RiakObject value, RiakPutOptions options = null);
        IEnumerable<RiakResult<RiakObject>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null);

        RiakResult Delete(string bucket, string key, RiakDeleteOptions options = null);
        RiakResult Delete(RiakObjectId objectId, RiakDeleteOptions options = null);
        IEnumerable<RiakResult> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null);

        IEnumerable<RiakResult> DeleteBucket(string bucket, uint rwVal = RiakConstants.Defaults.RVal);

        RiakResult<RiakSearchResult> Search(RiakSearchRequest search);

        RiakResult<RiakMapReduceResult> MapReduce(RiakMapReduceQuery query);

        RiakResult<RiakStreamedMapReduceResult> StreamMapReduce(RiakMapReduceQuery query);

        RiakResult<IEnumerable<string>> ListBuckets();
        RiakResult<IEnumerable<string>> ListKeys(string bucket);
        RiakResult<IEnumerable<string>> StreamListKeys(string bucket);

        RiakResult<RiakBucketProperties> GetBucketProperties(string bucket);

        RiakResult SetBucketProperties(string bucket, RiakBucketProperties properties);

        RiakResult ResetBucketProperties(string bucket);

        RiakResult<IList<RiakObject>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks);

        RiakResult<RiakServerInfo> GetServerInfo();

        RiakResult<IList<string>> IndexGet(string bucket, string indexName, BigInteger value, uint? maxResults = null, BigInteger? continuation = null);
        RiakResult<IList<string>> IndexGet(string bucket, string indexName, string value, uint? maxResults = null, string continuation = null);
        RiakResult<IList<string>> IndexGet(string bucket, string indexName, BigInteger minValue, BigInteger maxValue, uint? maxResults = null, BigInteger? continuation = null);
        RiakResult<IList<string>> IndexGet(string bucket, string indexName, string minValue, string maxValue, uint? maxResults = null, string continuation = null);

        RiakResult<IList<string>> ListKeysFromIndex(string bucket);

        //RiakResult<RiakSearchResult> Search(Action<RiakSearchRequest> prepareRequest)
    }

}