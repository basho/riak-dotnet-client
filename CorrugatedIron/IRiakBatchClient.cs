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

using CorrugatedIron.Models;
using CorrugatedIron.Models.Index;
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
        
        RiakCounterResult IncrementCounter(string bucket, string counter, long amount, RiakCounterUpdateOptions options = null);
        RiakCounterResult GetCounter(string bucket, string counter, RiakCounterGetOptions options = null);

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
        RiakResult<IEnumerable<string>> StreamListBuckets();
        RiakResult<IEnumerable<string>> ListKeys(string bucket);
        RiakResult<IEnumerable<string>> StreamListKeys(string bucket);

        RiakResult<RiakBucketProperties> GetBucketProperties(string bucket);
        RiakResult SetBucketProperties(string bucket, RiakBucketProperties properties, bool useHttp = false);
        RiakResult ResetBucketProperties(string bucket, bool useHttp = false);

        RiakResult<IList<RiakObject>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks);

        RiakResult<RiakServerInfo> GetServerInfo();

        RiakResult<RiakIndexResult> IndexGet(string bucket, string indexName, BigInteger value, RiakIndexGetOptions options = null);
        RiakResult<RiakIndexResult> IndexGet(string bucket, string indexName, string value, RiakIndexGetOptions options = null);
        RiakResult<RiakIndexResult> IndexGet(string bucket, string indexName, BigInteger minValue, BigInteger maxValue, RiakIndexGetOptions options = null);
        RiakResult<RiakIndexResult> IndexGet(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null);

        RiakResult<RiakStreamedIndexResult> StreamIndexGet(string bucket, string indexName, BigInteger value, RiakIndexGetOptions options = null);
        RiakResult<RiakStreamedIndexResult> StreamIndexGet(string bucket, string indexName, string value, RiakIndexGetOptions options = null);
        RiakResult<RiakStreamedIndexResult> StreamIndexGet(string bucket, string indexName, BigInteger minValue, BigInteger maxValue, RiakIndexGetOptions options = null);
        RiakResult<RiakStreamedIndexResult> StreamIndexGet(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null);

        RiakResult<IList<string>> ListKeysFromIndex(string bucket);

        //RiakResult<RiakSearchResult> Search(Action<RiakSearchRequest> prepareRequest)
    }

}