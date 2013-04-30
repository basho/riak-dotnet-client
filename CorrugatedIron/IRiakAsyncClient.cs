using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Util;

namespace CorrugatedIron
{
    public interface IRiakAsyncClient : IRiakBatchAsyncClient
    {
     
        Task Batch(Action<IRiakBatchAsyncClient> batchAction);
        
        Task Batch(Action<IRiakBatchClient> batchAction);
        
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
}

