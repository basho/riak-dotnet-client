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
using CorrugatedIron.Models;
using CorrugatedIron.Models.Index;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.Search;
using System.Collections.Generic;
using System.Numerics;

namespace CorrugatedIron
{
    public interface IRiakBatchClient : IDisposable
    {
        Pong Ping();

        RiakObject Get(RiakObjectId objectId, RiakGetOptions options = null);
        RiakObject Get(string bucket, string key, RiakGetOptions options = null);
        IEnumerable<RiakObject> Get(IEnumerable<RiakObjectId> bucketKeyPairs, RiakGetOptions options = null);
        
        RiakCounterResult IncrementCounter(string bucket, string counter, long amount, RiakCounterUpdateOptions options = null);
        RiakCounterResult GetCounter(string bucket, string counter, RiakCounterGetOptions options = null);

        RiakObject Put(RiakObject value, RiakPutOptions options = null);
        IEnumerable<RiakObject> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null);

        RiakObjectId Delete(RiakObject riakObject, RiakDeleteOptions options = null);
        RiakObjectId Delete(string bucket, string key, RiakDeleteOptions options = null);
        RiakObjectId Delete(RiakObjectId objectId, RiakDeleteOptions options = null);
        IEnumerable<RiakObjectId> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null);
        IEnumerable<RiakObjectId> DeleteBucket(string bucket, RiakDeleteOptions options = null);

        RiakSearchResult Search(RiakSearchRequest search);

        RiakMapReduceResult MapReduce(RiakMapReduceQuery query);
        RiakStreamedMapReduceResult StreamMapReduce(RiakMapReduceQuery query);

        IEnumerable<string> ListBuckets();
        IEnumerable<string> StreamListBuckets();
        IEnumerable<string> ListKeys(string bucket);
        IEnumerable<string> StreamListKeys(string bucket);

        RiakBucketProperties GetBucketProperties(string bucket);
        bool SetBucketProperties(string bucket, RiakBucketProperties properties, bool useHttp = false);
        bool ResetBucketProperties(string bucket, bool useHttp = false);

        IEnumerable<RiakObject> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks);

        RiakServerInfo GetServerInfo();

        RiakIndexResult IndexGet(string bucket, string indexName, BigInteger value, RiakIndexGetOptions options = null);
        RiakIndexResult IndexGet(string bucket, string indexName, string value, RiakIndexGetOptions options = null);
        RiakIndexResult IndexGet(string bucket, string indexName, BigInteger minValue, BigInteger maxValue, RiakIndexGetOptions options = null);
        RiakIndexResult IndexGet(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null);

        RiakStreamedIndexResult StreamIndexGet(string bucket, string indexName, BigInteger value, RiakIndexGetOptions options = null);
        RiakStreamedIndexResult StreamIndexGet(string bucket, string indexName, string value, RiakIndexGetOptions options = null);
        RiakStreamedIndexResult StreamIndexGet(string bucket, string indexName, BigInteger minValue, BigInteger maxValue, RiakIndexGetOptions options = null);
        RiakStreamedIndexResult StreamIndexGet(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null);

        IEnumerable<string> ListKeysFromIndex(string bucket);
    }

}