// <copyright file="IRiakAsyncClient.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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
// </copyright>

namespace RiakClient
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Threading.Tasks;
    using Models;
    using Models.Index;
    using Models.MapReduce;
    using Models.Search;

    public interface IRiakAsyncClient
    {
        Task<RiakResult> Ping();

        Task<RiakResult<RiakObject>> Get(string bucket, string key, RiakGetOptions options = null);

        Task<RiakResult<RiakObject>> Get(RiakObjectId objectId, RiakGetOptions options = null);

        Task<IEnumerable<RiakResult<RiakObject>>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, RiakGetOptions options = null);

        Task<RiakCounterResult> IncrementCounter(string bucket, string counter, long amount, RiakCounterUpdateOptions options = null);

        Task<RiakCounterResult> GetCounter(string bucket, string counter, RiakCounterGetOptions options = null);

        Task<RiakResult<RiakObject>> Put(RiakObject value, RiakPutOptions options = null);

        Task<IEnumerable<RiakResult<RiakObject>>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null);

        Task<RiakResult> Delete(RiakObject riakObject, RiakDeleteOptions options = null);

        Task<RiakResult> Delete(string bucket, string key, RiakDeleteOptions options = null);

        Task<RiakResult> Delete(RiakObjectId objectId, RiakDeleteOptions options = null);

        Task<IEnumerable<RiakResult>> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null);

        Task<RiakResult<RiakSearchResult>> Search(RiakSearchRequest search);

        Task<RiakResult<RiakMapReduceResult>> MapReduce(RiakMapReduceQuery query);

        Task<RiakResult<RiakStreamedMapReduceResult>> StreamMapReduce(RiakMapReduceQuery query);

        Task<RiakResult<IEnumerable<string>>> ListBuckets();

        Task<RiakResult<IEnumerable<string>>> StreamListBuckets();

        Task<RiakResult<IEnumerable<string>>> ListKeys(string bucket);

        Task<RiakResult<IEnumerable<string>>> StreamListKeys(string bucket);

        Task<RiakResult<RiakBucketProperties>> GetBucketProperties(string bucket);

        [Obsolete("This overload will be removed in the next version.")]
        Task<RiakResult> SetBucketProperties(string bucket, RiakBucketProperties properties, bool useHttp = false);

        Task<RiakResult> ResetBucketProperties(string bucket, bool useHttp = false);

        [Obsolete("Linkwalking has been depreciated as of Riak 2.0. This method will be removed in the next major version.")]
        Task<RiakResult<IList<RiakObject>>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks);

        Task<RiakResult<RiakServerInfo>> GetServerInfo();

        Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, BigInteger value, RiakIndexGetOptions options = null);

        Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, string value, RiakIndexGetOptions options = null);

        Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, BigInteger min, BigInteger max, RiakIndexGetOptions options = null);

        Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, string min, string max, RiakIndexGetOptions options = null);

        Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, BigInteger value, RiakIndexGetOptions options = null);

        Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, string value, RiakIndexGetOptions options = null);

        Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, BigInteger min, BigInteger max, RiakIndexGetOptions options = null);

        Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, string min, string max, RiakIndexGetOptions options = null);

        Task<RiakResult<IList<string>>> ListKeysFromIndex(string bucket);

        Task Batch(Action<IRiakBatchClient> batchAction);
    }
}
