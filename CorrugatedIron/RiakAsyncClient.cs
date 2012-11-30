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
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorrugatedIron
{
    public interface IRiakAsyncClient
    {
        Task<RiakResult> Ping();

        Task<RiakResult<RiakObject>> Get(string bucket, string key, uint rVal = RiakConstants.Defaults.RVal);
        Task<RiakResult<RiakObject>> Get(RiakObjectId objectId, uint rVal = RiakConstants.Defaults.RVal);
        Task<IEnumerable<RiakResult<RiakObject>>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, uint rVal = RiakConstants.Defaults.RVal);

        Task<RiakResult<RiakObject>> Put(RiakObject value, RiakPutOptions options = null);
        Task<IEnumerable<RiakResult<RiakObject>>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null);

        Task<RiakResult> Delete(string bucket, string key, RiakDeleteOptions options = null);
        Task<RiakResult> Delete(RiakObjectId objectId, RiakDeleteOptions options = null);
        Task<IEnumerable<RiakResult>> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null);
        Task<IEnumerable<RiakResult>> DeleteBucket(string bucket, uint rwVal = RiakConstants.Defaults.RVal);

        Task<RiakResult<RiakMapReduceResult>> MapReduce(RiakMapReduceQuery query);
        Task<RiakResult<RiakStreamedMapReduceResult>> StreamMapReduce(RiakMapReduceQuery query);

        Task<RiakResult<IEnumerable<string>>> ListBuckets();
        Task<RiakResult<IEnumerable<string>>> ListKeys(string bucket);
        Task<RiakResult<IEnumerable<string>>> StreamListKeys(string bucket);

        Task<RiakResult<RiakBucketProperties>> GetBucketProperties(string bucket, bool extended = false);
        Task<RiakResult> SetBucketProperties(string bucket, RiakBucketProperties properties);

        Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, int value);
        Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, string value);
        Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, int minValue, int maxValue);
        Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, string minValue, string maxValue);

        Task<RiakResult<IList<RiakObject>>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks);

        Task<RiakResult<RiakServerInfo>> GetServerInfo();

        void Batch(Action<IRiakBatchClient> batchAction);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void Ping(Action<RiakResult> callback);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void Get(string bucket, string key, Action<RiakResult<RiakObject>> callback, uint rVal = RiakConstants.Defaults.RVal);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void Get(RiakObjectId objectId, Action<RiakResult<RiakObject>> callback, uint rVal = RiakConstants.Defaults.RVal);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void Get(IEnumerable<RiakObjectId> bucketKeyPairs, Action<IEnumerable<RiakResult<RiakObject>>> callback, uint rVal = RiakConstants.Defaults.RVal);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void Put(RiakObject value, Action<RiakResult<RiakObject>> callback, RiakPutOptions options = null);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void Put(IEnumerable<RiakObject> values, Action<IEnumerable<RiakResult<RiakObject>>> callback, RiakPutOptions options = null);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void Delete(string bucket, string key, Action<RiakResult> callback, RiakDeleteOptions options = null);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void Delete(RiakObjectId objectId, Action<RiakResult> callback, RiakDeleteOptions options = null);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void Delete(IEnumerable<RiakObjectId> objectIds, Action<IEnumerable<RiakResult>> callback, RiakDeleteOptions options = null);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void DeleteBucket(string bucket, Action<IEnumerable<RiakResult>> callback, uint rwVal = RiakConstants.Defaults.RVal);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void MapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakMapReduceResult>> callback);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void StreamMapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakStreamedMapReduceResult>> callback);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void ListBuckets(Action<RiakResult<IEnumerable<string>>> callback);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void ListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void StreamListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void GetBucketProperties(string bucket, Action<RiakResult<RiakBucketProperties>> callback, bool extended = false);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void SetBucketProperties(string bucket, RiakBucketProperties properties, Action<RiakResult> callback);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks, Action<RiakResult<IList<RiakObject>>> callback);

        [Obsolete("All async operations should use the functions that return Task<T>.")]
        void GetServerInfo(Action<RiakResult<RiakServerInfo>> callback);
    }

    internal class RiakAsyncClient : IRiakAsyncClient
    {
        private readonly IRiakClient _client;

        public RiakAsyncClient(IRiakClient client)
        {
            _client = client;
        }

        public Task<RiakResult> Ping()
        {
            return Task.Factory.StartNew(() => _client.Ping());
        }

        public Task<RiakResult<RiakObject>> Get(string bucket, string key, uint rVal = RiakConstants.Defaults.RVal)
        {
            return Task.Factory.StartNew(() => _client.Get(bucket, key, rVal));
        }

        public Task<RiakResult<RiakObject>> Get(RiakObjectId objectId, uint rVal = RiakConstants.Defaults.RVal)
        {
            return Task.Factory.StartNew(() => _client.Get(objectId.Bucket, objectId.Key, rVal));
        }

        public Task<IEnumerable<RiakResult<RiakObject>>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, uint rVal = RiakConstants.Defaults.RVal)
        {
            return Task.Factory.StartNew(() => _client.Get(bucketKeyPairs, rVal));
        }

        public Task<IEnumerable<RiakResult<RiakObject>>> Put(IEnumerable<RiakObject> values, RiakPutOptions options)
        {
            return Task.Factory.StartNew(() => _client.Put(values, options));
        }

        public Task<RiakResult<RiakObject>> Put(RiakObject value, RiakPutOptions options)
        {
            return Task.Factory.StartNew(() => _client.Put(value, options));
        }

        public Task<RiakResult> Delete(string bucket, string key, RiakDeleteOptions options = null)
        {
            return Task.Factory.StartNew(() => _client.Delete(bucket, key, options));
        }

        public Task<RiakResult> Delete(RiakObjectId objectId, RiakDeleteOptions options = null)
        {
            return Task.Factory.StartNew(() => _client.Delete(objectId.Bucket, objectId.Key, options));
        }

        public Task<IEnumerable<RiakResult>> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null)
        {
            return Task.Factory.StartNew(() => _client.Delete(objectIds, options));
        }

        public Task<IEnumerable<RiakResult>> DeleteBucket(string bucket, uint rwVal = RiakConstants.Defaults.RVal)
        {
            return Task.Factory.StartNew(() => _client.DeleteBucket(bucket, rwVal));
        }

        public Task<RiakResult<RiakMapReduceResult>> MapReduce(RiakMapReduceQuery query)
        {
            return Task.Factory.StartNew(() => _client.MapReduce(query));
        }

        public Task<RiakResult<RiakStreamedMapReduceResult>> StreamMapReduce(RiakMapReduceQuery query)
        {
            return Task.Factory.StartNew(() => _client.StreamMapReduce(query));
        }

        public Task<RiakResult<IEnumerable<string>>>  ListBuckets()
        {
            return Task.Factory.StartNew(() => _client.ListBuckets());
        }

        public Task<RiakResult<IEnumerable<string>>> ListKeys(string bucket)
        {
            return Task.Factory.StartNew(() => _client.ListKeys(bucket));
        }

        public Task<RiakResult<IEnumerable<string>>> StreamListKeys(string bucket)
        {
            return Task.Factory.StartNew(() => _client.StreamListKeys(bucket));
        }

        public Task<RiakResult<RiakBucketProperties>> GetBucketProperties(string bucket, bool extended = false)
        {
            return Task.Factory.StartNew(() => _client.GetBucketProperties(bucket, extended));
        }

        public Task<RiakResult> SetBucketProperties(string bucket, RiakBucketProperties properties)
        {
            return Task.Factory.StartNew(() => _client.SetBucketProperties(bucket, properties));
        }

        public Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, int value)
        {
            return Task.Factory.StartNew(() => _client.IndexGet(bucket, indexName, value));
        }

        public Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, string value)
        {
            return Task.Factory.StartNew(() => _client.IndexGet(bucket, indexName, value));
        }

        public Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, int minValue, int maxValue)
        {
            return Task.Factory.StartNew(() => _client.IndexGet(bucket, indexName, minValue, maxValue));
        }

        public Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, string minValue, string maxValue)
        {
            return Task.Factory.StartNew(() => _client.IndexGet(bucket, indexName, minValue, maxValue));
        }

        public Task<RiakResult<IList<RiakObject>>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks)
        {
            return Task.Factory.StartNew(() => _client.WalkLinks(riakObject, riakLinks));
        }

        public Task<RiakResult<RiakServerInfo>>  GetServerInfo()
        {
            return Task.Factory.StartNew(() => _client.GetServerInfo());
        }

        public void Batch(Action<IRiakBatchClient> batchAction)
        {
            Task.Factory.StartNew(() => _client.Batch(batchAction));
        }


        // -----------------------------------------------------------------------------------------------------------
        // All functions below are marked as obsolete
        // -----------------------------------------------------------------------------------------------------------

        public void Ping(Action<RiakResult> callback)
        {
            ExecAsync(() => callback(_client.Ping()));
        }

        public void Get(string bucket, string key, Action<RiakResult<RiakObject>> callback, uint rVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(() => callback(_client.Get(bucket, key, rVal)));
        }

        public void Get(RiakObjectId objectId, Action<RiakResult<RiakObject>> callback, uint rVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(() => callback(_client.Get(objectId.Bucket, objectId.Key, rVal)));
        }

        public void Get(IEnumerable<RiakObjectId> bucketKeyPairs, Action<IEnumerable<RiakResult<RiakObject>>> callback, uint rVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(() => callback(_client.Get(bucketKeyPairs, rVal)));
        }

        public void Put(IEnumerable<RiakObject> values, Action<IEnumerable<RiakResult<RiakObject>>> callback, RiakPutOptions options)
        {
            ExecAsync(() => callback(_client.Put(values, options)));
        }

        public void Put(RiakObject value, Action<RiakResult<RiakObject>> callback, RiakPutOptions options)
        {
            ExecAsync(() => callback(_client.Put(value, options)));
        }

        public void Delete(string bucket, string key, Action<RiakResult> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(() => callback(_client.Delete(bucket, key, options)));
        }

        public void Delete(RiakObjectId objectId, Action<RiakResult> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(() => callback(_client.Delete(objectId.Bucket, objectId.Key, options)));
        }

        public void Delete(IEnumerable<RiakObjectId> objectIds, Action<IEnumerable<RiakResult>> callback, RiakDeleteOptions options = null)
        {
            ExecAsync(() => callback(_client.Delete(objectIds, options)));
        }

        public void DeleteBucket(string bucket, Action<IEnumerable<RiakResult>> callback, uint rwVal = RiakConstants.Defaults.RVal)
        {
            ExecAsync(() => callback(_client.DeleteBucket(bucket, rwVal)));
        }

        public void MapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakMapReduceResult>> callback)
        {
            ExecAsync(() => callback(_client.MapReduce(query)));
        }

        public void StreamMapReduce(RiakMapReduceQuery query, Action<RiakResult<RiakStreamedMapReduceResult>> callback)
        {
            ExecAsync(() => callback(_client.StreamMapReduce(query)));
        }

        public void ListBuckets(Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(_client.ListBuckets()));
        }

        public void ListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(_client.ListKeys(bucket)));
        }

        public void StreamListKeys(string bucket, Action<RiakResult<IEnumerable<string>>> callback)
        {
            ExecAsync(() => callback(_client.StreamListKeys(bucket)));
        }

        public void GetBucketProperties(string bucket, Action<RiakResult<RiakBucketProperties>> callback, bool extended = false)
        {
            ExecAsync(() => callback(_client.GetBucketProperties(bucket, extended)));
        }

        public void SetBucketProperties(string bucket, RiakBucketProperties properties, Action<RiakResult> callback)
        {
            ExecAsync(() => callback(_client.SetBucketProperties(bucket, properties)));
        }

        public void WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks, Action<RiakResult<IList<RiakObject>>> callback)
        {
            ExecAsync(() => callback(_client.WalkLinks(riakObject, riakLinks)));
        }

        public void GetServerInfo(Action<RiakResult<RiakServerInfo>> callback)
        {
            ExecAsync(() => callback(_client.GetServerInfo()));
        }

        private static void ExecAsync(Action action)
        {
            Task.Factory.StartNew(action);
        }
    }
}