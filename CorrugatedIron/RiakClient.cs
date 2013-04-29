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

using CorrugatedIron.Comms;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Models.Rest;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;

namespace CorrugatedIron
{
    public interface IRiakClient : IRiakBatchClient
    {
    /*
        void Batch(Action<IRiakBatchClient> batchAction);

        T Batch<T>(Func<IRiakBatchClient, T> batchFunction);
        */

        IRiakAsyncClient Async { get; }
    }

    public class RiakClient : IRiakClient
    {
        private IRiakAsyncClient _client;

        public int RetryCount
        {
            get { return _client.RetryCount; }
            set { _client.RetryCount = value; }
        }

        public IRiakAsyncClient Async
        {
            get { return _client; }
        }

        internal RiakClient(IRiakAsyncClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Ping this instance of Riak
        /// </summary>
        /// <description>Ping can be used to ensure that there is an operational Riak node
        /// present at the other end of the client. It's important to note that this will ping
        /// any Riak node in the cluster and a specific node cannot be specified by the user.
        /// Do not use this method to determine individual node health.</description>
        /// <returns>Returns true if the Riak instance has returned a 'pong' response. 
        /// Returns false if Riak is unavailable or returns a 'pang' response. </returns>
        public RiakResult Ping()
        {
            return WaitFor(_client.Ping());
        }

        /// <summary>
        /// Get the specified <paramref name="key"/> from the <paramref name="bucket"/>.
        /// Optionally can be read from rVal instances. By default, the server's
        /// r-value will be used, but can be overridden by rVal.
        /// </summary>
        /// <param name='bucket'>
        /// The name of the bucket containing the <paramref name="key"/>
        /// </param>
        /// <param name='key'>
        /// The key.
        /// </param>
        /// <param name='options'>The <see cref="CorrugatedIron.Models.RiakGetOptions" /> responsible for 
        /// configuring the semantics of this single get request. These options will override any previously 
        /// defined bucket configuration properties.</param>
        /// <remarks>If a node does not respond, that does not necessarily mean that the 
        /// <paramref name="bucket"/>/<paramref name="key"/> combination is not available. It simply means
        /// that fewer than R/PR nodes responded to the read request. See <see cref="CorrugatedIron.Models.RiakGetOptions" />
        /// for information on how different options change Riak's default behavior.
        /// </remarks>
        public RiakResult<RiakObject> Get(string bucket, string key, RiakGetOptions options = null)
        {
            return WaitFor(_client.Get(bucket, key, options));
        }

        /// <summary>
        /// Get the specified <paramref name="key"/> from the <paramref name="bucket"/>.
        /// Optionally can be read from <paramref name="rVal"/> instances. By default, the server's
        /// r-value will be used, but can be overridden by <paramref name="rVal"/>.
        /// </summary>
        /// <param name='bucket'>
        /// The name of the bucket containing the <paramref name="key"/>
        /// </param>
        /// <param name='key'>
        /// The key.
        /// </param>
        /// <param name='rVal'>
        /// The number of nodes required to successfully respond to the read before the read is considered a success.
        /// </param>
        /// <remarks>If a node does not respond, that does not necessarily mean that the 
        /// <paramref name="bucket"/>/<paramref name="key"/> combination is not available. It simply means
        /// that fewer than <paramref name="rVal" /> nodes responded to the read request. Unfortunatley, 
        /// the Riak API does not allow us to distinguish between a 404 resulting from less than <paramref name="rVal"/>
        /// nodes successfully responding and a <paramref name="bucket"/>/<paramref name="key"/> combination
        /// not being found in Riak.
        /// </remarks>
        [Obsolete("Use Get(string, string, RiakGetOptions) instead")]
        public RiakResult<RiakObject> Get(string bucket, string key, uint rVal = RiakConstants.Defaults.RVal)
        {
            var options = new RiakGetOptions().SetR(rVal);
            return Get(bucket, key, options);
        }

        /// <summary>
        /// Retrieve the specified object from Riak.
        /// </summary>
        /// <param name='objectId'>
        /// Object identifier made up of a key and bucket. <see cref="CorrugatedIron.Models.RiakObjectId"/>
        /// </param>
        /// <param name='rVal'>
        /// The number of nodes required to successfully respond to the read before the read is considered a success.
        /// </param>
        /// <remarks>If a node does not respond, that does not necessarily mean that the 
        /// <paramref name="objectId"/> is not available. It simply means
        /// that fewer than <paramref name="rVal" /> nodes responded to the read request. Unfortunatley, 
        /// the Riak API does not allow us to distinguish between a 404 resulting from less than <paramref name="rVal"/>
        /// nodes successfully responding and an <paramref name="objectId"/> not being found in Riak.
        /// </remarks>
        [Obsolete("Use Get(string, string, RiakGetOptions) instead")]
        public RiakResult<RiakObject> Get(RiakObjectId objectId, uint rVal = RiakConstants.Defaults.RVal)
        {
            return Get(objectId.Bucket, objectId.Key, rVal);
        }

        /// <summary>
        /// Retrieve multiple objects from Riak.
        /// </summary>
        /// <param name='bucketKeyPairs'>
        /// An <see href="System.Collections.Generic.IEnumerable&lt;T&gt;"/> of <see cref="CorrugatedIron.Models.RiakObjectId"/> to be retrieved
        /// </param>
        /// <param name='options'>The <see cref="CorrugatedIron.Models.RiakGetOptions" /> responsible for 
        /// configuring the semantics of this single get request. These options will override any previously 
        /// defined bucket configuration properties.</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> of <see cref="RiakResult{T}"/>
        /// is returned. You should verify the success or failure of each result separately.</returns>
        /// <remarks>Riak does not support multi get behavior. CorrugatedIron's multi get functionality wraps multiple
        /// get requests and returns results as an IEnumerable{RiakResult{RiakObject}}. Callers should be aware that
        /// this may result in partial success - all results should be evaluated individually in the calling application.
        /// In addition, applications should plan for multiple failures or multiple cases of siblings being present.</remarks>
        public IEnumerable<RiakResult<RiakObject>> Get(IEnumerable<RiakObjectId> bucketKeyPairs,
                                                       RiakGetOptions options = null)
        {
            return WaitFor(_client.Get(bucketKeyPairs));
        }

        /// <summary>
        /// Retrieve multiple objects from Riak.
        /// </summary>
        /// <param name='bucketKeyPairs'>
        /// An <see href="System.Collections.Generic.IEnumerable&lt;T&gt;"/> of <see cref="CorrugatedIron.Models.RiakObjectId"/> to be retrieved
        /// </param>
        /// <param name='rVal'>
        /// The number of nodes required to successfully respond to the read before the read is considered a success.
        /// </param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> of <see cref="RiakResult{TResult}"/>
        /// is returned. You should verify the success or failure of each result separately.</returns>
        /// <remarks>Riak does not support multi get behavior. CorrugatedIron's multi get functionality wraps multiple
        /// get requests and returns results as an IEnumerable{RiakResult{RiakObject}}. Callers should be aware that
        /// this may result in partial success - all results should be evaluated individually in the calling application.
        /// In addition, applications should plan for multiple failures or multiple cases of siblings being present.</remarks>
        [Obsolete("Use Get(IEnumerable<RiakObjectId>, RiakGetOptions) instead.")]
        public IEnumerable<RiakResult<RiakObject>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, uint rVal = RiakConstants.Defaults.RVal)
        {
            var options = new RiakGetOptions().SetR(rVal);
            return Get(bucketKeyPairs, options);
        }


        /// <summary>
        /// Persist a <see cref="CorrugatedIron.Models.RiakObject"/> to Riak using the specific <see cref="CorrugatedIron.Models.RiakPutOptions" />.
        /// </summary>
        /// <param name='value'>
        /// The <see cref="CorrugatedIron.Models.RiakObject"/> to save.
        /// </param>
        /// <param name='options'>
        /// Put options
        /// </param>
        public RiakResult<RiakObject> Put(RiakObject value, RiakPutOptions options = null)
        {
            return WaitFor(_client.Put(value, options));
        }

        /// <summary>
        /// Persist an <see href="System.Collections.Generic.IEnumerable{T}"/> of <see cref="CorrugatedIron.Models.RiakObjectId"/> to Riak.
        /// </summary>
        /// <param name='values'>
        /// The <see href="System.Collections.Generic.IEnumerable{T}"/> of <see cref="CorrugatedIron.Models.RiakObjectId"/> to save.
        /// </param>
        /// <param name='options'>
        /// Put options.
        /// </param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> of <see cref="RiakResult{T}"/>
        /// is returned. You should verify the success or failure of each result separately.</returns>
        /// <remarks>Riak does not support multi put behavior. CorrugatedIron's multi put functionality wraps multiple
        /// put requests and returns results as an IEnumerable{RiakResult{RiakObject}}. Callers should be aware that
        /// this may result in partial success - all results should be evaluated individually in the calling application.
        /// In addition, applications should plan for multiple failures or multiple cases of siblings being present.</remarks>
        public IEnumerable<RiakResult<RiakObject>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null)
        {
            return WaitFor(_client.Put(values, options));
        }

        /// <summary>
        /// Delete the record identified by <paramref name="key"/> from a <paramref name="bucket"/>.
        /// </summary>
        /// <param name='bucket'>
        /// The name of the bucket that contains the record to be deleted.
        /// </param>
        /// <param name='key'>
        /// The key identifying the record to be deleted.
        /// </param>
        /// <param name='options'>
        /// Delete options
        /// </param>
        public RiakResult Delete(string bucket, string key, RiakDeleteOptions options = null)
        {
            return WaitFor(_client.Delete(bucket, key, options));
        }

        /// <summary>
        /// Delete the record identified by the <paramref name="objectId"/>.
        /// </summary>
        /// <param name='objectId'>
        /// A <see cref="CorrugatedIron.Models.RiakObjectId"/> identifying the bucket/key combination for the record to be deleted.
        /// </param>
        /// <param name='options'>
        /// Delete options
        /// </param>
        public RiakResult Delete(RiakObjectId objectId, RiakDeleteOptions options = null)
        {
            return Delete(objectId.Bucket, objectId.Key, options);
        }

        /// <summary>
        /// Delete multiple objects identified by a <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> of <see cref="CorrugatedIron.Models.RiakObjectId"/>.
        /// </summary>
        /// <param name='objectIds'>
        /// A <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> of <see cref="CorrugatedIron.Models.RiakObjectId"/>.
        /// </param>
        /// <param name='options'>
        /// Delete options.
        /// </param>    
        public IEnumerable<RiakResult> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null)
        {
            return WaitFor(_client.Delete(objectIds, options));
        }

        /// <summary>
        /// Deletes the contents of the specified <paramref name="bucket"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> of <see cref="CorrugatedIron.RiakResult"/> listing the success of all deletes
        /// </returns>
        /// <param name='bucket'>
        /// The bucket to be deleted.
        /// </param>
        /// <param name='rwVal'>
        /// The number of nodes that must respond successfully to a delete request.
        /// </param>
        /// <remarks>
        /// /// <para>
        /// Because of the <see cref="CorrugatedIron.RiakClient.ListKeys"/> operation, this may be a time consuming operation on
        /// production systems and may cause memory problems for the client. This should be used either in testing or on small buckets with 
        /// known amounts of data.
        /// </para>
        /// <para>
        /// A delete bucket operation actually deletes all keys in the bucket individually. 
        /// A <see cref="CorrugatedIron.RiakClient.ListKeys"/> operation is performed to retrieve a list of keys
        /// The keys retrieved from the <see cref="CorrugatedIron.RiakClient.ListKeys"/> are then deleted through
        /// <see cref="CorrugatedIron.RiakClient.Delete"/>. 
        /// </para>
        /// </remarks>
        public IEnumerable<RiakResult> DeleteBucket(string bucket, uint rwVal)
        {
            return WaitFor(_client.DeleteBucket(bucket, rwVal));
        }

        /// <summary>
        /// Deletes the contents of the specified <paramref name="bucket"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> of <see cref="CorrugatedIron.RiakResult"/> listing the success of all deletes
        /// </returns>
        /// <param name='bucket'>
        /// The bucket to be deleted.
        /// </param>
        /// <param name='deleteOptions'>
        /// Options for Riak delete operation <see cref="CorrugatedIron.Models.RiakDeleteOptions"/>
        /// </param>
        /// <remarks>
        /// <para>
        /// A delete bucket operation actually deletes all keys in the bucket individually. 
        /// A <see cref="CorrugatedIron.RiakClient.ListKeys"/> operation is performed to retrieve a list of keys
        /// The keys retrieved from the <see cref="CorrugatedIron.RiakClient.ListKeys"/> are then deleted through
        /// <see cref="CorrugatedIron.RiakClient.Delete"/>. 
        /// </para>
        /// <para>
        /// Because of the <see cref="CorrugatedIron.RiakClient.ListKeys"/> operation, this may be a time consuming operation on
        /// production systems and may cause memory problems for the client. This should be used either in testing or on small buckets with 
        /// known amounts of data.
        /// </para>
        /// </remarks>
        public IEnumerable<RiakResult> DeleteBucket(string bucket, RiakDeleteOptions deleteOptions)
        {
            return WaitFor(_client.DeleteBucket(bucket, deleteOptions));
        }

        public RiakResult<RiakMapReduceResult> MapReduce(RiakMapReduceQuery query)
        {
            return WaitFor(_client.MapReduce(query));
        }

        public RiakResult<RiakSearchResult> Search(RiakSearchRequest search)
        {
            return WaitFor(_client.Search(search));
        }

        public RiakResult<RiakStreamedMapReduceResult> StreamMapReduce(RiakMapReduceQuery query)
        {
            return WaitFor(_client.StreamMapReduce(query));
        }

        /// <summary>
        /// Lists all buckets available on the Riak cluster.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> of <see cref="string"/> bucket names.
        /// </returns>
        /// <remarks>Buckets provide a logical namespace for keys. Listing buckets requires folding over all keys in a cluster and 
        /// reading a list of buckets from disk. This operation, while non-blocking in Riak 1.0 and newer, still produces considerable
        /// physical I/O and can take a long time.</remarks>
        public RiakResult<IEnumerable<string>> ListBuckets()
        {
            return WaitFor(_client.ListBuckets());
        }

        /// <summary>
        /// Lists all keys in the specified <paramref name="bucket"/>.
        /// </summary>
        /// <returns>
        /// The keys.
        /// </returns>
        /// <param name='bucket'>
        /// The bucket.
        /// </param>
        /// <remarks>ListKeys is an expensive operation that requires folding over all data in the Riak cluster to produce
        /// a list of keys. This operation, while cheaper in Riak 1.0 than in earlier versions of Riak, should be avoided.</remarks>
        public RiakResult<IEnumerable<string>> ListKeys(string bucket)
        {
            return WaitFor(_client.ListKeys(bucket));
        }

        public RiakResult<IEnumerable<string>> StreamListKeys(string bucket)
        {
            return WaitFor(_client.StreamListKeys(bucket));
        }

        /// <summary>
        /// Return a list of keys from the given bucket.
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns></returns>
        /// <remarks>This uses the $key special index instead of the list keys API to 
        /// quickly return an unsorted list of keys from Riak.</remarks>
        public RiakResult<IList<string>> ListKeysFromIndex(string bucket)
        {
            return WaitFor(_client.ListKeysFromIndex(bucket));
        }

        /// <summary>
        /// Returns all properties for a <paramref name="bucket"/>.
        /// </summary>
        /// <returns>
        /// The bucket properties.
        /// </returns>
        /// <param name='bucket'>
        /// The Riak bucket.
        /// </param>
        /// <param name='extended'>
        /// Extended parameters are retrieved by HTTP requests.
        /// </param>
        public RiakResult<RiakBucketProperties> GetBucketProperties(string bucket, bool extended = false)
        {
            return WaitFor(_client.GetBucketProperties(bucket, extended));
        }

        /// <summary>
        /// Sets the <see cref="CorrugatedIron.Models.RiakBucketProperties"/> properties of a <paramref name="bucket"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="CorrugatedIron.RiakResult"/> detailing the success or failure of the operation.
        /// </returns>
        /// <param name='bucket'>
        /// The Bucket.
        /// </param>
        /// <param name='properties'>
        /// The Properties.
        /// </param>
        public RiakResult SetBucketProperties(string bucket, RiakBucketProperties properties)
        {
            return WaitFor(_client.SetBucketProperties(bucket, properties));
        }

        /// <summary>
        /// Reset the properties on a bucket back to their defaults.
        /// </summary>
        /// <param name="bucket">The name of the bucket to reset the properties on.</param>
        /// <returns>An indication of success or failure.</returns>
        /// <remarks>This function requires Riak v1.3+.</remarks>
        public RiakResult ResetBucketProperties(string bucket)
        {
            return WaitFor(_client.ResetBucketProperties(bucket));
        }
        
        /// <summary>
        /// Get the results of an index query prepared for use in a <see cref="CorrugatedIron.Models.MapReduce.RiakMapReduceQuery"/>
        /// </summary>
        /// <returns>
        /// A <see cref="RiakBucketKeyInput"/> of the index query results
        /// </returns>
        /// <param name='indexQuery'>
        /// Index query.
        /// </param>
        [Obsolete("This has been replaced by the IndexGet methods as of v1.1.1. This method will be removed by v1.3")]
        public RiakBucketKeyInput GetIndex(RiakIndexInput indexQuery)
        {
            var query = new RiakMapReduceQuery()
                .Inputs(indexQuery).ReduceErlang(r => r.ModFun("riak_kv_mapreduce", "reduce_identity").Keep(true));
            var result = MapReduce(query);
            
            var keys = result.Value.PhaseResults.OrderBy(pr => pr.Phase).ElementAt(0).GetObjects<RiakObjectId>();
            
            return RiakBucketKeyInput.FromRiakObjectIds(keys);
        }

        /// <summary>
        /// Retrieve arbitrarily deep list of links for a <see cref="RiakObject"/>
        /// </summary>
        /// <returns>
        /// A list of <see cref="RiakObject"/> identified by the list of links.
        /// </returns>
        /// <param name='riakObject'>
        /// The initial object to use for the beginning of the link walking.
        /// </param>
        /// <param name='riakLinks'>
        /// A list of link definitions
        /// </param>
        /// <remarks>Refer to http://wiki.basho.com/Links-and-Link-Walking.html for more information.</remarks>
        public RiakResult<IList<RiakObject>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks)
        {
            return WaitFor(_client.WalkLinks(riakObject, riakLinks));
        }

        /// <summary>
        /// Get the server information from the connected cluster.
        /// </summary>
        /// <returns>Model containing information gathered from a node in the cluster.</returns>
        /// <remarks>This function will assume that all of the nodes in the cluster are running
        /// the same version of Riak. It will only get executed on a single node, and the content
        /// that is returned technically only relates to that node. All nodes in a cluster should
        /// run on the same version of Riak.</remarks>
        public RiakResult<RiakServerInfo> GetServerInfo()
        {
            return WaitFor(_client.GetServerInfo());
        }

        public RiakResult<IList<string>> IndexGet(string bucket, string indexName, string minValue, string maxValue)
        {
            return WaitFor(_client.IndexGet(bucket, indexName, minValue, maxValue));
        }

        public RiakResult<IList<string>> IndexGet(string bucket, string indexName, int minValue, int maxValue)
        {
            return WaitFor(_client.IndexGet(bucket, indexName, minValue, maxValue));
        }

        public RiakResult<IList<string>> IndexGet(string bucket, string indexName, string value)
        {
            return WaitFor(_client.IndexGet(bucket, indexName, value));
        }

        public RiakResult<IList<string>> IndexGet(string bucket, string indexName, int value)
        {
            return WaitFor(_client.IndexGet(bucket, indexName, value));
        }

        /// <summary>
        /// Used to create a batched set of actions to be sent to a Riak cluster. This guarantees some level of serialized activity.
        /// </summary>
        /// <param name='batchAction'>
        /// Batch action.
        /// </param>
        /// <exception cref='Exception'>
        /// Represents errors that occur during application execution.
        /// </exception>
        public void Batch(Action<IRiakBatchClient> batchAction)
        {
            throw new NotImplementedException();
            // WaitFor(_client.Batch(batchAction));
        }

        /*
        public T Batch<T>(Func<IRiakBatchClient, T> batchFun)
        {
            var funResult = default(T);

            Func<IRiakConnection, Action, RiakResult<IEnumerable<RiakResult<object>>>> helperBatchFun = (conn, onFinish) =>
            {
                try
                {
                    funResult = batchFun(new RiakClient(conn));
                    return RiakResult<IEnumerable<RiakResult<object>>>.Success(null);
                }
                catch(Exception ex)
                {
                    return RiakResult<IEnumerable<RiakResult<object>>>.Error(ResultCode.BatchException, "{0}\n{1}".Fmt(ex.Message, ex.StackTrace), true);
                }
                finally
                {
                    onFinish();
                }
            };

            var result = _endPoint.UseDelayedConnection(helperBatchFun, RetryCount);

            if(!result.IsSuccess && result.ResultCode == ResultCode.BatchException)
            {
                throw new Exception(result.ErrorMessage);
            }

            return funResult;
        }
        */

        // simple wrapper
        private T WaitFor<T>(Task<T> task)
        {
            task.Wait();
            return task.Result;
        }

        // just keep consistent
        private void WaitFor(Task task)
        {
            task.Wait();
        }
    }
}