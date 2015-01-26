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
using CorrugatedIron.Models.RiakDt;
using CorrugatedIron.Messages;

namespace CorrugatedIron
{
    public interface IRiakBatchClient
    {
        int RetryCount { get; set; }
        RiakResult Ping();

        RiakResult<RiakObject> Get(RiakObjectId objectId, RiakGetOptions options = null);
        RiakResult<RiakObject> Get(string bucket, string key, RiakGetOptions options = null);
        RiakResult<RiakObject> Get(string bucketType, string bucket, string key, RiakGetOptions options = null);
        IEnumerable<RiakResult<RiakObject>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, RiakGetOptions options = null);
        
        RiakCounterResult IncrementCounter(string bucket, string counter, long amount, RiakCounterUpdateOptions options = null);
        RiakCounterResult GetCounter(string bucket, string counter, RiakCounterGetOptions options = null);

        RiakResult<RiakObject> Put(RiakObject value, RiakPutOptions options = null);
        IEnumerable<RiakResult<RiakObject>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null);

        RiakResult Delete(RiakObject riakObject, RiakDeleteOptions options = null);
        RiakResult Delete(string bucket, string key, RiakDeleteOptions options = null);
        RiakResult Delete(string bucketType, string bucket, string key, RiakDeleteOptions options = null);
        RiakResult Delete(RiakObjectId objectId, RiakDeleteOptions options = null);
        IEnumerable<RiakResult> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null);

        RiakResult<RiakSearchResult> Search(RiakSearchRequest search);

        RiakResult<RiakMapReduceResult> MapReduce(RiakMapReduceQuery query);
        RiakResult<RiakStreamedMapReduceResult> StreamMapReduce(RiakMapReduceQuery query);

        RiakResult<IEnumerable<string>> ListBuckets();
        RiakResult<IEnumerable<string>> StreamListBuckets();
        RiakResult<IEnumerable<string>> ListKeys(string bucket);
        RiakResult<IEnumerable<string>> ListKeys(string bucketType, string bucket);
        RiakResult<IEnumerable<string>> StreamListKeys(string bucket);
        RiakResult<IEnumerable<string>> StreamListKeys(string bucketType, string bucket);

        RiakResult<RiakBucketProperties> GetBucketProperties(string bucket);
        RiakResult<RiakBucketProperties> GetBucketProperties(string bucketType, string bucket);
        RiakResult SetBucketProperties(string bucket, RiakBucketProperties properties, bool useHttp = false);
        RiakResult SetBucketProperties(string bucketType, string bucket, RiakBucketProperties properties);
        RiakResult ResetBucketProperties(string bucket, bool useHttp = false);
        RiakResult ResetBucketProperties(string bucketType, string bucket);

        RiakResult<IList<RiakObject>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks);

        RiakResult<RiakServerInfo> GetServerInfo();

        RiakResult<RiakIndexResult> GetSecondaryIndex(RiakIndexId index, BigInteger value, RiakIndexGetOptions options = null);
        RiakResult<RiakIndexResult> GetSecondaryIndex(RiakIndexId index, string value, RiakIndexGetOptions options = null);
        RiakResult<RiakIndexResult> GetSecondaryIndex(RiakIndexId index, BigInteger min, BigInteger max, RiakIndexGetOptions options = null);
        RiakResult<RiakIndexResult> GetSecondaryIndex(RiakIndexId index, string min, string max, RiakIndexGetOptions options = null);

        RiakResult<RiakStreamedIndexResult> StreamGetSecondaryIndex(RiakIndexId index, BigInteger value, RiakIndexGetOptions options = null);
        RiakResult<RiakStreamedIndexResult> StreamGetSecondaryIndex(RiakIndexId index, string value, RiakIndexGetOptions options = null);
        RiakResult<RiakStreamedIndexResult> StreamGetSecondaryIndex(RiakIndexId index, BigInteger min, BigInteger max, RiakIndexGetOptions options = null);
        RiakResult<RiakStreamedIndexResult> StreamGetSecondaryIndex(RiakIndexId index, string min, string max, RiakIndexGetOptions options = null);

        RiakResult<IList<string>> ListKeysFromIndex(string bucket);
        RiakResult<IList<string>> ListKeysFromIndex(string bucketType, string bucket);

        RiakResult<RiakObject> DtFetch(string bucketType, string bucket, string key, RiakDtFetchOptions options = null);
        RiakResult<RiakObject> DtFetch(RiakObjectId riakObject, RiakDtFetchOptions options = null);
        
        RiakCounterResult DtFetchCounter(string bucketType, string bucket, string key, RiakDtFetchOptions options = null);
        RiakCounterResult DtFetchCounter(RiakObjectId objectId, RiakDtFetchOptions options = null);
        RiakCounterResult DtUpdateCounter(string bucketType, string bucket, string key, long amount, RiakDtUpdateOptions options = null);
        RiakCounterResult DtUpdateCounter(RiakObjectId objectId, long amount, RiakDtUpdateOptions options = null);
        
        RiakDtSetResult DtFetchSet(string bucketType, string bucket, string key, RiakDtFetchOptions options = null);
        RiakDtSetResult DtFetchSet(RiakObjectId objectId, RiakDtFetchOptions options = null);
        RiakDtSetResult DtUpdateSet<T>(string bucketType, string bucket, string key, SerializeObjectToByteArray<T> serialize, byte[] context, List<T> adds = null, List<T> removes = null, RiakDtUpdateOptions options = null);
        RiakDtSetResult DtUpdateSet<T>(RiakObjectId objectId, SerializeObjectToByteArray<T> serialize, byte[] context, List<T> adds = null, List<T> removes = null, RiakDtUpdateOptions options = null);

        RiakDtMapResult DtFetchMap(string bucketType, string bucket, string key, RiakDtFetchOptions options = null);
        RiakDtMapResult DtFetchMap(RiakObjectId objectId, RiakDtFetchOptions options = null);
        RiakDtMapResult DtUpdateMap<T>(string bucketType, string bucket, string key, SerializeObjectToByteArray<T> serialize, byte[] context, List<RiakDtMapField> removes = null, List<MapUpdate> updates = null, RiakDtUpdateOptions options = null);
        RiakDtMapResult DtUpdateMap<T>(RiakObjectId objectId, SerializeObjectToByteArray<T> serialize, byte[] context, List<RiakDtMapField> removes = null, List<MapUpdate> updates = null, RiakDtUpdateOptions options = null);

        RiakResult<SearchIndexResult> GetSearchIndex(string indexName);
        RiakResult PutSearchIndex(SearchIndex index);
        RiakResult DeleteSearchIndex(string indexName);

        RiakResult<SearchSchema> GetSearchSchema(string schemaName);
        RiakResult PutSearchSchema(SearchSchema schema);
        RiakResult<string> GetServerStatus();

    }
}