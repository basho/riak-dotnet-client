// <copyright file="RiakAsyncClient.cs" company="Basho Technologies, Inc.">
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

    internal class RiakAsyncClient : IRiakAsyncClient
    {
        private readonly IRiakClient client;

        public RiakAsyncClient(IRiakClient client)
        {
            this.client = client;
        }

        public Task<RiakResult> Ping()
        {
            return Task.Factory.StartNew(() => client.Ping());
        }

        public Task<RiakResult<RiakObject>> Get(string bucket, string key, RiakGetOptions options = null)
        {
            options = options ?? RiakClient.DefaultGetOptions();
            return Task.Factory.StartNew(() => client.Get(bucket, key, options));
        }

        public Task<RiakResult<RiakObject>> Get(RiakObjectId objectId, RiakGetOptions options = null)
        {
            options = options ?? RiakClient.DefaultGetOptions();
            return Task.Factory.StartNew(() => client.Get(objectId.Bucket, objectId.Key, options));
        }

        public Task<IEnumerable<RiakResult<RiakObject>>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, RiakGetOptions options = null)
        {
            options = options ?? RiakClient.DefaultGetOptions();
            return Task.Factory.StartNew(() => client.Get(bucketKeyPairs, options));
        }

        public Task<RiakCounterResult> IncrementCounter(string bucket, string counter, long amount, RiakCounterUpdateOptions options = null)
        {
            return Task.Factory.StartNew(() => client.IncrementCounter(bucket, counter, amount, options));
        }

        public Task<RiakCounterResult> GetCounter(string bucket, string counter, RiakCounterGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.GetCounter(bucket, counter, options));
        }

        public Task<IEnumerable<RiakResult<RiakObject>>> Put(IEnumerable<RiakObject> values, RiakPutOptions options)
        {
            return Task.Factory.StartNew(() => client.Put(values, options));
        }

        public Task<RiakResult<RiakObject>> Put(RiakObject value, RiakPutOptions options)
        {
            return Task.Factory.StartNew(() => client.Put(value, options));
        }

        public Task<RiakResult> Delete(RiakObject riakObject, RiakDeleteOptions options = null)
        {
            return Task.Factory.StartNew(() => client.Delete(riakObject, options));
        }

        public Task<RiakResult> Delete(string bucket, string key, RiakDeleteOptions options = null)
        {
            return Task.Factory.StartNew(() => client.Delete(bucket, key, options));
        }

        public Task<RiakResult> Delete(RiakObjectId objectId, RiakDeleteOptions options = null)
        {
            return Task.Factory.StartNew(() => client.Delete(objectId.Bucket, objectId.Key, options));
        }

        public Task<IEnumerable<RiakResult>> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null)
        {
            return Task.Factory.StartNew(() => client.Delete(objectIds, options));
        }

        public Task<RiakResult<RiakSearchResult>> Search(RiakSearchRequest search)
        {
            return Task.Factory.StartNew(() => client.Search(search));
        }

        public Task<RiakResult<RiakMapReduceResult>> MapReduce(RiakMapReduceQuery query)
        {
            return Task.Factory.StartNew(() => client.MapReduce(query));
        }

        public Task<RiakResult<RiakStreamedMapReduceResult>> StreamMapReduce(RiakMapReduceQuery query)
        {
            return Task.Factory.StartNew(() => client.StreamMapReduce(query));
        }

        public Task<RiakResult<IEnumerable<string>>> StreamListBuckets()
        {
            return Task.Factory.StartNew(() => client.StreamListBuckets());
        }

        public Task<RiakResult<IEnumerable<string>>> ListBuckets()
        {
            return Task.Factory.StartNew(() => client.ListBuckets());
        }

        public Task<RiakResult<IEnumerable<string>>> ListKeys(string bucket)
        {
            return Task.Factory.StartNew(() => client.ListKeys(bucket));
        }

        public Task<RiakResult<IEnumerable<string>>> StreamListKeys(string bucket)
        {
            return Task.Factory.StartNew(() => client.StreamListKeys(bucket));
        }

        public Task<RiakResult<RiakBucketProperties>> GetBucketProperties(string bucket)
        {
            return Task.Factory.StartNew(() => client.GetBucketProperties(bucket));
        }

        public Task<RiakResult> SetBucketProperties(string bucket, RiakBucketProperties properties, bool useHttp = false)
        {
            return Task.Factory.StartNew(() => client.SetBucketProperties(bucket, properties, useHttp));
        }

        public Task<RiakResult> ResetBucketProperties(string bucket, bool useHttp = false)
        {
            return Task.Factory.StartNew(() => client.ResetBucketProperties(bucket, useHttp));
        }

        public Task<RiakResult<IList<string>>> ListKeysFromIndex(string bucket)
        {
            return Task.Factory.StartNew(() => client.ListKeysFromIndex(bucket));
        }

        public Task<RiakResult<IList<RiakObject>>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks)
        {
            return Task.Factory.StartNew(() => client.WalkLinks(riakObject, riakLinks));
        }

        public Task<RiakResult<RiakServerInfo>> GetServerInfo()
        {
            return Task.Factory.StartNew(() => client.GetServerInfo());
        }

        public Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, BigInteger value, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.GetSecondaryIndex(index, value, options));
        }

        public Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, string value, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.GetSecondaryIndex(index, value, options));
        }

        public Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, BigInteger min, BigInteger max, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.GetSecondaryIndex(index, min, max, options));
        }

        public Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, string min, string max, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.GetSecondaryIndex(index, min, max, options));
        }

        public Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, BigInteger value, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.StreamGetSecondaryIndex(index, value, options));
        }

        public Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, string value, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.StreamGetSecondaryIndex(index, value, options));
        }

        public Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, BigInteger min, BigInteger max, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.StreamGetSecondaryIndex(index, min, max, options));
        }

        public Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, string min, string max, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.StreamGetSecondaryIndex(index, min, max, options));
        }

        public Task Batch(Action<IRiakBatchClient> batchAction)
        {
            return Task.Factory.StartNew(() => client.Batch(batchAction));
        }

        public Task<T> Batch<T>(Func<IRiakBatchClient, T> batchFunc)
        {
            return Task.Factory.StartNew<T>(() => client.Batch(batchFunc));
        }

        /*
         * -----------------------------------------------------------------------------------------------------------
         * All functions below are marked as obsolete
         * -----------------------------------------------------------------------------------------------------------
         */

        public void Ping(Action<RiakResult> callback)
        {
            ExecAsync(() => callback(client.Ping()));
        }

        public void Get(string bucket, string key, Action<RiakResult<RiakObject>> callback, RiakGetOptions options = null)
        {
            options = options ?? RiakClient.DefaultGetOptions();
            ExecAsync(() => callback(client.Get(bucket, key, options)));
        }

        public void Get(RiakObjectId objectId, Action<RiakResult<RiakObject>> callback, RiakGetOptions options = null)
        {
            options = options ?? RiakClient.DefaultGetOptions();
            ExecAsync(() => callback(client.Get(objectId.Bucket, objectId.Key, options)));
        }

        public void Get(IEnumerable<RiakObjectId> bucketKeyPairs, Action<IEnumerable<RiakResult<RiakObject>>> callback, RiakGetOptions options = null)
        {
            options = options ?? RiakClient.DefaultGetOptions();
            ExecAsync(() => callback(client.Get(bucketKeyPairs, options)));
        }

        public void Put(IEnumerable<RiakObject> values, Action<IEnumerable<RiakResult<RiakObject>>> callback, RiakPutOptions options)
        {
            ExecAsync(() => callback(client.Put(values, options)));
        }

        public void Put(RiakObject value, Action<RiakResult<RiakObject>> callback, RiakPutOptions options)
        {
            ExecAsync(() => callback(client.Put(value, options)));
        }

        public void Delete(string bucket, string key, Action<RiakResult> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(() => callback(client.Delete(bucket, key, options)));
        }

        public void Delete(RiakObjectId objectId, Action<RiakResult> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(() => callback(client.Delete(objectId.Bucket, objectId.Key, options)));
        }

        public void Delete(IEnumerable<RiakObjectId> objectIds, Action<IEnumerable<RiakResult>> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(() => callback(client.Delete(objectIds, options)));
        }

        public void MapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakMapReduceResult>> callback)
        {
            ExecAsync(() => callback(client.MapReduce(query)));
        }

        public void StreamMapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakStreamedMapReduceResult>> callback)
        {
            ExecAsync(() => callback(client.StreamMapReduce(query)));
        }

        public void ListBuckets(Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(client.ListBuckets()));
        }

        public void ListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(client.ListKeys(bucket)));
        }

        public void StreamListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(client.StreamListKeys(bucket)));
        }

        public void GetBucketProperties(string bucket, Action<RiakResult<RiakBucketProperties>> callback)
        {
            ExecAsync(() => callback(client.GetBucketProperties(bucket)));
        }

        public void SetBucketProperties(string bucket, RiakBucketProperties properties, Action<RiakResult> callback)
        {
            ExecAsync(() => callback(client.SetBucketProperties(bucket, properties)));
        }

        public void WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks, Action<RiakResult<IList<RiakObject>>> callback)
        {
            ExecAsync(() => callback(client.WalkLinks(riakObject, riakLinks)));
        }

        public void GetServerInfo(Action<RiakResult<RiakServerInfo>> callback)
        {
            ExecAsync(() => callback(client.GetServerInfo()));
        }

        private static void ExecAsync(Action action)
        {
            Task.Factory.StartNew(action);
        }
    }
}
