using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using CorrugatedIron.Containers;
using CorrugatedIron.Exceptions;
using CorrugatedIron.Models;
using CorrugatedIron.Models.Index;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.Search;

namespace CorrugatedIron
{
    public interface IRiakAsyncBatchClient : IDisposable
    {
        Task<Pong> Ping();

        Task<Either<RiakException, RiakObject>> Get(string bucket, string key, RiakGetOptions options = null);
        Task<Either<RiakException, RiakObject>> Get(RiakObjectId objectId, RiakGetOptions options = null);

        IObservable<Either<RiakException, RiakObject>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, RiakGetOptions options = null);

        Task<Either<RiakException,RiakCounterResult>> IncrementCounter(string bucket, string counter, long amount, RiakCounterUpdateOptions options = null);
        Task<Either<RiakException,RiakCounterResult>> GetCounter(string bucket, string counter, RiakCounterGetOptions options = null);

        Task<Either<RiakException, RiakObject>> Put(RiakObject value, RiakPutOptions options = null);
        IObservable<Either<RiakException, RiakObject>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null);

        Task<Either<RiakException, RiakObjectId>> Delete(RiakObject riakObject, RiakDeleteOptions options = null);
        Task<Either<RiakException, RiakObjectId>> Delete(string bucket, string key, RiakDeleteOptions options = null);
        Task<Either<RiakException, RiakObjectId>> Delete(RiakObjectId objectId, RiakDeleteOptions options = null);
        IObservable<Either<RiakException, RiakObjectId>> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null);
        IObservable<Either<RiakException, RiakObjectId>> DeleteBucket(string bucket, RiakDeleteOptions deleteOptions = null);

        Task<Either<RiakException, RiakSearchResult>> Search(RiakSearchRequest search);

        Task<RiakMapReduceResult> MapReduce(RiakMapReduceQuery query);
        Task<RiakStreamedMapReduceResult> StreamMapReduce(RiakMapReduceQuery query);

        IObservable<Either<RiakException,string>> ListBuckets();
        IObservable<Either<RiakException,string>> StreamListBuckets();
        IObservable<Either<RiakException,string>> ListKeys(string bucket);
        IObservable<Either<RiakException,string>> StreamListKeys(string bucket);

        Task<RiakBucketProperties> GetBucketProperties(string bucket);
        Task<bool> SetBucketProperties(string bucket, RiakBucketProperties properties, bool useHttp = false);
        Task<bool> ResetBucketProperties(string bucket, bool useHttp = false);

        IObservable<Either<RiakException, RiakObject>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks);

        Task<Either<RiakException, RiakServerInfo>> GetServerInfo();

        Task<Either<RiakException, RiakIndexResult>> IndexGet(string bucket, string indexName, BigInteger value, RiakIndexGetOptions options = null);
        Task<Either<RiakException, RiakIndexResult>> IndexGet(string bucket, string indexName, string value, RiakIndexGetOptions options = null);
        Task<Either<RiakException, RiakIndexResult>> IndexGet(string bucket, string indexName, BigInteger minValue, BigInteger maxValue, RiakIndexGetOptions options = null);
        Task<Either<RiakException, RiakIndexResult>> IndexGet(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null);

        Task<Either<RiakException, RiakStreamedIndexResult>> StreamIndexGet(string bucket, string indexName, BigInteger value, RiakIndexGetOptions options = null);
        Task<Either<RiakException, RiakStreamedIndexResult>> StreamIndexGet(string bucket, string indexName, string value, RiakIndexGetOptions options = null);
        Task<Either<RiakException, RiakStreamedIndexResult>> StreamIndexGet(string bucket, string indexName, BigInteger minValue, BigInteger maxValue, RiakIndexGetOptions options = null);
        Task<Either<RiakException, RiakStreamedIndexResult>> StreamIndexGet(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null);

        IObservable<Either<RiakException, string>> ListKeysFromIndex(string bucket);
    }
}