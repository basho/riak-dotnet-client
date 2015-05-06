// <copyright file="RiakAsyncClient.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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
    using Commands;
    using Models;
    using Models.Index;
    using Models.MapReduce;
    using Models.Search;

    /// <summary>
    /// An asyncronous version of <see cref="RiakClient"/>.
    /// </summary>
    internal class RiakAsyncClient : IRiakAsyncClient
    {
        private readonly IRiakClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakAsyncClient"/> class from the specified <see cref="IRiakClient"/>.
        /// </summary>
        /// <param name="client">The <see cref="RiakClient"/> to use for all operations.</param>
        public RiakAsyncClient(IRiakClient client)
        {
            this.client = client;
        }

        /// <inheritdoc/>
        public Task<RiakResult> Ping()
        {
            return Task.Factory.StartNew(() => client.Ping());
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakObject>> Get(string bucket, string key, RiakGetOptions options = null)
        {
            options = options ?? RiakGetOptions.Default;
            return Task.Factory.StartNew(() => client.Get(bucket, key, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakObject>> Get(RiakObjectId objectId, RiakGetOptions options = null)
        {
            options = options ?? RiakGetOptions.Default;
            return Task.Factory.StartNew(() => client.Get(objectId.Bucket, objectId.Key, options));
        }
        
        /// <inheritdoc/>
        public Task<IEnumerable<RiakResult<RiakObject>>> Get(IEnumerable<RiakObjectId> objectIds, RiakGetOptions options = null)
        {
            options = options ?? RiakGetOptions.Default;
            return Task.Factory.StartNew(() => client.Get(objectIds, options));
        }

        /// <inheritdoc/>
        public Task<RiakCounterResult> IncrementCounter(string bucket, string counter, long amount, RiakCounterUpdateOptions options = null)
        {
            return Task.Factory.StartNew(() => client.IncrementCounter(bucket, counter, amount, options));
        }

        /// <inheritdoc/>
        public Task<RiakCounterResult> GetCounter(string bucket, string counter, RiakCounterGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.GetCounter(bucket, counter, options));
        }

        /// <inheritdoc/>
        public Task<IEnumerable<RiakResult<RiakObject>>> Put(IEnumerable<RiakObject> values, RiakPutOptions options)
        {
            return Task.Factory.StartNew(() => client.Put(values, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakObject>> Put(RiakObject value, RiakPutOptions options)
        {
            return Task.Factory.StartNew(() => client.Put(value, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult> Delete(RiakObject riakObject, RiakDeleteOptions options = null)
        {
            return Task.Factory.StartNew(() => client.Delete(riakObject, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult> Delete(string bucket, string key, RiakDeleteOptions options = null)
        {
            return Task.Factory.StartNew(() => client.Delete(bucket, key, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult> Delete(RiakObjectId objectId, RiakDeleteOptions options = null)
        {
            return Task.Factory.StartNew(() => client.Delete(objectId.Bucket, objectId.Key, options));
        }

        /// <inheritdoc/>
        public Task<IEnumerable<RiakResult>> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null)
        {
            return Task.Factory.StartNew(() => client.Delete(objectIds, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakSearchResult>> Search(RiakSearchRequest search)
        {
            return Task.Factory.StartNew(() => client.Search(search));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakMapReduceResult>> MapReduce(RiakMapReduceQuery query)
        {
            return Task.Factory.StartNew(() => client.MapReduce(query));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakStreamedMapReduceResult>> StreamMapReduce(RiakMapReduceQuery query)
        {
            return Task.Factory.StartNew(() => client.StreamMapReduce(query));
        }

        /// <inheritdoc/>
        public Task<RiakResult<IEnumerable<string>>> StreamListBuckets()
        {
            return Task.Factory.StartNew(() => client.StreamListBuckets());
        }

        /// <inheritdoc/>
        public Task<RiakResult<IEnumerable<string>>> ListBuckets()
        {
            return Task.Factory.StartNew(() => client.ListBuckets());
        }

        /// <inheritdoc/>
        public Task<RiakResult<IEnumerable<string>>> ListKeys(string bucket)
        {
            return Task.Factory.StartNew(() => client.ListKeys(bucket));
        }

        /// <inheritdoc/>
        public Task<RiakResult<IEnumerable<string>>> StreamListKeys(string bucket)
        {
            return Task.Factory.StartNew(() => client.StreamListKeys(bucket));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakBucketProperties>> GetBucketProperties(string bucket)
        {
            return Task.Factory.StartNew(() => client.GetBucketProperties(bucket));
        }

        /// <inheritdoc/>
        public Task<RiakResult> SetBucketProperties(string bucket, RiakBucketProperties properties)
        {
            return Task.Factory.StartNew(() => client.SetBucketProperties(bucket, properties));
        }

        /// <inheritdoc/>
        public Task<RiakResult> ResetBucketProperties(string bucket)
        {
            return Task.Factory.StartNew(() => client.ResetBucketProperties(bucket));
        }

        /// <inheritdoc/>
        public Task<RiakResult<IList<string>>> ListKeysFromIndex(string bucket)
        {
            return Task.Factory.StartNew(() => client.ListKeysFromIndex(bucket));
        }

        /// <inheritdoc/>
        [Obsolete("Linkwalking has been deprecated as of Riak 2.0. This method will be removed in the next major version.")]
        public Task<RiakResult<IList<RiakObject>>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks)
        {
#pragma warning disable 618
            return Task.Factory.StartNew(() => client.WalkLinks(riakObject, riakLinks));
#pragma warning restore 618
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakServerInfo>> GetServerInfo()
        {
            return Task.Factory.StartNew(() => client.GetServerInfo());
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, BigInteger value, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.GetSecondaryIndex(index, value, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, string value, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.GetSecondaryIndex(index, value, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, BigInteger min, BigInteger max, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.GetSecondaryIndex(index, min, max, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, string min, string max, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.GetSecondaryIndex(index, min, max, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, BigInteger value, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.StreamGetSecondaryIndex(index, value, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, string value, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.StreamGetSecondaryIndex(index, value, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, BigInteger min, BigInteger max, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.StreamGetSecondaryIndex(index, min, max, options));
        }

        /// <inheritdoc/>
        public Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, string min, string max, RiakIndexGetOptions options = null)
        {
            return Task.Factory.StartNew(() => client.StreamGetSecondaryIndex(index, min, max, options));
        }

        /// <inheritdoc/>
        public Task Batch(Action<IRiakBatchClient> batchAction)
        {
            return Task.Factory.StartNew(() => client.Batch(batchAction));
        }

        // TODO: Everything under here isn't in the interface
        
        /// <inheritdoc/>
        public Task<T> Batch<T>(Func<IRiakBatchClient, T> batchFunc)
        {
            return Task.Factory.StartNew<T>(() => client.Batch(batchFunc));
        }

        /// <inheritdoc/>
        public Task<RiakResult> Ping(Action<RiakResult> callback)
        {
            return Task.Factory.StartNew(() => client.Ping());
        }

        /// <inheritdoc/>
        public void Get(string bucket, string key, Action<RiakResult<RiakObject>> callback, RiakGetOptions options = null)
        {
            options = options ?? RiakGetOptions.Default;
            ExecAsync(() => callback(client.Get(bucket, key, options)));
        }

        /// <inheritdoc/>
        public void Get(RiakObjectId objectId, Action<RiakResult<RiakObject>> callback, RiakGetOptions options = null)
        {
            options = options ?? RiakGetOptions.Default;
            ExecAsync(() => callback(client.Get(objectId.Bucket, objectId.Key, options)));
        }

        /// <inheritdoc/>
        public void Get(IEnumerable<RiakObjectId> objectIds, Action<IEnumerable<RiakResult<RiakObject>>> callback, RiakGetOptions options = null)
        {
            options = options ?? RiakGetOptions.Default;
            ExecAsync(() => callback(client.Get(objectIds, options)));
        }

        /// <inheritdoc/>
        public void Put(IEnumerable<RiakObject> values, Action<IEnumerable<RiakResult<RiakObject>>> callback, RiakPutOptions options)
        {
            ExecAsync(() => callback(client.Put(values, options)));
        }

        /// <inheritdoc/>
        public void Put(RiakObject value, Action<RiakResult<RiakObject>> callback, RiakPutOptions options)
        {
            ExecAsync(() => callback(client.Put(value, options)));
        }

        /// <inheritdoc/>
        public void Delete(string bucket, string key, Action<RiakResult> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(() => callback(client.Delete(bucket, key, options)));
        }

        /// <inheritdoc/>
        public void Delete(RiakObjectId objectId, Action<RiakResult> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(() => callback(client.Delete(objectId.Bucket, objectId.Key, options)));
        }

        /// <inheritdoc/>
        public void Delete(IEnumerable<RiakObjectId> objectIds, Action<IEnumerable<RiakResult>> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(() => callback(client.Delete(objectIds, options)));
        }

        /// <inheritdoc/>
        public void MapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakMapReduceResult>> callback)
        {
            ExecAsync(() => callback(client.MapReduce(query)));
        }

        /// <inheritdoc/>
        public void StreamMapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakStreamedMapReduceResult>> callback)
        {
            ExecAsync(() => callback(client.StreamMapReduce(query)));
        }

        /// <inheritdoc/>
        public void ListBuckets(Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(client.ListBuckets()));
        }

        /// <inheritdoc/>
        public void ListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(client.ListKeys(bucket)));
        }

        /// <inheritdoc/>
        public void StreamListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(client.StreamListKeys(bucket)));
        }

        /// <inheritdoc/>
        public void GetBucketProperties(string bucket, Action<RiakResult<RiakBucketProperties>> callback)
        {
            ExecAsync(() => callback(client.GetBucketProperties(bucket)));
        }

        /// <inheritdoc/>
        public void SetBucketProperties(string bucket, RiakBucketProperties properties, Action<RiakResult> callback)
        {
            ExecAsync(() => callback(client.SetBucketProperties(bucket, properties)));
        }

        /// <inheritdoc/>
        public void GetServerInfo(Action<RiakResult<RiakServerInfo>> callback)
        {
            ExecAsync(() => callback(client.GetServerInfo()));
        }

        /// <inheritdoc/>
        public Task<RiakResult> Execute(IRiakCommand command)
        {
            return Task.Factory.StartNew(() => client.Execute(command));
        }

        private static void ExecAsync(Action action)
        {
            Task.Factory.StartNew(action);
        }
    }
}