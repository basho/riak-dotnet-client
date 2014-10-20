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
using System.Linq;
using System.Reactive.Linq;
using CorrugatedIron.Comms;
using CorrugatedIron.Models;
using CorrugatedIron.Models.Index;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Util;
using System.Collections.Generic;
using System.Numerics;

namespace CorrugatedIron
{
    public class RiakClient : IRiakClient
    {
        private readonly IRiakEndPoint _endPoint;
        private readonly IRiakConnection _riakConnection;

        public IRiakAsyncClient Async { get; private set; }

        public void Batch(Action<IRiakBatchClient> batchAction)
        {
            if (_endPoint is RiakBatch)
            {
                batchAction(this);
                return;
            }

            using (var batchEndPoint = new RiakBatch(_endPoint, new RiakEndPointContext()))
            {
                using (var batchedClient = new RiakClient(batchEndPoint, _riakConnection))
                {
                    batchAction(batchedClient);
                }
            }
        }

        public T Batch<T>(Func<IRiakBatchClient, T> batchFunction)
        {
            if (_endPoint is RiakBatch)
            {
                return batchFunction(this);
            }

            using (var batchEndPoint = new RiakBatch(_endPoint, new RiakEndPointContext()))
            {
                using (var batchedClient = new RiakClient(batchEndPoint, _riakConnection))
                {
                    return batchFunction(batchedClient);
                }
            }
        }

        public IEnumerable<T> Batch<T>(Func<IRiakBatchClient, IEnumerable<T>> batchFunction)
        {
            if (_endPoint is RiakBatch)
            {
                return batchFunction(this);
            }

            using (var batchEndPoint = new RiakBatch(_endPoint, new RiakEndPointContext()))
            {
                using (var batchedClient = new RiakClient(batchEndPoint, _riakConnection))
                {
                    return batchFunction(batchedClient);
                }
            }
        }

        internal RiakClient(IRiakEndPoint endPoint, IRiakConnection riakConnection)
        {
            _endPoint = endPoint;
            _riakConnection = riakConnection;
            Async = new RiakAsyncClient(endPoint, riakConnection);
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
        public Pong Ping()
        {
            return Async.Ping().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Increments a Riak counter. 
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <param name="counter">The name of the counter</param>
        /// <param name="amount">The amount to increment/decrement the counter</param>
        /// <param name="options">The <see cref="RiakCounterUpdateOptions"/></param>
        /// <returns><see cref="RiakCounterResult"/></returns>
        /// <remarks>Only available in Riak 1.4+. If the counter is not initialized, then the counter will be initialized to 0 and then incremented.</remarks>
        public RiakCounterResult IncrementCounter(string bucket, string counter, long amount, RiakCounterUpdateOptions options = null)
        {
            var result = Async.IncrementCounter(bucket, counter, amount, options).ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.IsLeft)
            {
                throw result.Left;
            }

            return result.Right;
        }

        /// <summary>
        /// Returns the value of a counter
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <param name="counter">The counter</param>
        /// <param name="options"><see cref="RiakCounterGetOptions"/> describing how to read the counter.</param>
        /// <returns><see cref="RiakCounterResult"/></returns>
        /// <remarks>Only available in Riak 1.4+.</remarks>
        public RiakCounterResult GetCounter(string bucket, string counter, RiakCounterGetOptions options = null)
        {
            var result = Async.GetCounter(bucket, counter, options).ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.IsLeft)
            {
                throw result.Left;
            }

            return result.Right;
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
        public RiakObject Get(string bucket, string key, RiakGetOptions options = null)
        {
            var result = Async.Get(bucket, key, options).ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            return result.IsLeft ? null : result.Right;
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
        /// <remarks>If a node does not respond, that does not necessarily mean that the 
        /// <paramref name="bucket"/>/<paramref name="key"/> combination is not available. It simply means
        /// that fewer than the default number nodes responded to the read request. Unfortunatley, 
        /// the Riak API does not allow us to distinguish between a 404 resulting from less than <paramref name="rVal"/>
        /// nodes successfully responding and a <paramref name="bucket"/>/<paramref name="key"/> combination
        /// not being found in Riak.
        /// </remarks>
        public RiakObject Get(string bucket, string key)
        {
            var result = Async.Get(bucket, key).ConfigureAwait(false).GetAwaiter().GetResult();
            return result.IsLeft ? null : result.Right;
        }

        /// <summary>
        /// Retrieve the specified object from Riak.
        /// </summary>
        /// <param name='objectId'>
        /// Object identifier made up of a key and bucket. <see cref="CorrugatedIron.Models.RiakObjectId"/>
        /// </param>
        /// <param name='options'>The <see cref="CorrugatedIron.Models.RiakGetOptions" /> responsible for 
        /// configuring the semantics of this single get request. These options will override any previously 
        /// defined bucket configuration properties.</param>
        /// <remarks>If a node does not respond, that does not necessarily mean that the 
        /// <paramref name="objectId"/> is not available. It simply means
        /// that fewer than <paramref name="rVal" /> nodes responded to the read request. Unfortunatley, 
        /// the Riak API does not allow us to distinguish between a 404 resulting from less than <paramref name="rVal"/>
        /// nodes successfully responding and an <paramref name="objectId"/> not being found in Riak.
        /// </remarks>
        public RiakObject Get(RiakObjectId objectId, RiakGetOptions options = null)
        {
            var result = Async.Get(objectId, options).ConfigureAwait(false).GetAwaiter().GetResult();
            return result.IsLeft ? null : result.Right;
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
        public IEnumerable<RiakObject> Get(IEnumerable<RiakObjectId> bucketKeyPairs, RiakGetOptions options = null)
        {
            return Async.Get(bucketKeyPairs, options)
                .ToEnumerable()
                .Where(x => !x.IsLeft)
                .Select(x => x.Right)
                .ToList();
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
        public RiakObject Put(RiakObject value, RiakPutOptions options = null)
        {
            var result = Async.Put(value, options).ConfigureAwait(false).GetAwaiter().GetResult();
            return result.IsLeft ? null : result.Right;
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
        public IEnumerable<RiakObject> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null)
        {
            return Async.Put(values, options)
                .ToEnumerable()
                .Where(x => !x.IsLeft)
                .Select(x => x.Right)
                .ToList();
        }

        /// <summary>
        /// Delete the data identified by the <paramref name="riakObject"/>
        /// </summary>
        /// <param name='riakObject'>
        /// The object to delete
        /// </param>
        public RiakObjectId Delete(RiakObject riakObject, RiakDeleteOptions options = null)
        {
            var result = Async.Delete(riakObject, options).ConfigureAwait(false).GetAwaiter().GetResult();
            return result.IsLeft ? null : result.Right;
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
        public RiakObjectId Delete(string bucket, string key, RiakDeleteOptions options = null)
        {
            var result = Async.Delete(bucket, key, options).ConfigureAwait(false).GetAwaiter().GetResult();
            return result.IsLeft ? null : result.Right;
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
        public RiakObjectId Delete(RiakObjectId objectId, RiakDeleteOptions options = null)
        {
            var result = Async.Delete(objectId, options).ConfigureAwait(false).GetAwaiter().GetResult();
            return result.IsLeft ? null : result.Right;
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
        public IEnumerable<RiakObjectId> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null)
        {
            return Async.Delete(objectIds, options)
                .ToEnumerable()
                .Where(x => !x.IsLeft)
                .Select(x => x.Right)
                .ToList();
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
        public IEnumerable<RiakObjectId> DeleteBucket(string bucket, RiakDeleteOptions deleteOptions)
        {
            return Async.DeleteBucket(bucket, deleteOptions)
                .ToEnumerable()
                .Where(x => !x.IsLeft)
                .Select(x => x.Right)
                .ToList();
        }

        /// <summary>
        /// Perform a Riak Search query
        /// </summary>
        /// <param name="search">The <see cref="RiakSearchRequest"/></param>
        /// <returns>A <see cref="RiakResult"/> of <see cref="RiakSearchResult"/></returns>
        public RiakSearchResult Search(RiakSearchRequest search)
        {
            var result = Async.Search(search).ConfigureAwait(false).GetAwaiter().GetResult();
            return result.IsLeft ? null : result.Right;
        }

        /// <summary>
        /// Execute a map reduce query.
        /// </summary>
        /// <param name="query">A <see cref="RiakMapReduceQuery"/></param>
        /// <returns>A <see cref="RiakResult"/> of <see cref="RiakMapReduceResult"/></returns>
        public RiakMapReduceResult MapReduce(RiakMapReduceQuery query)
        {
            return Async.MapReduce(query).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Perform a map reduce query and stream the results.
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>A <see cref="RiakResult"/> of <see cref="RiakStreamedMapReduceResult"/></returns>
        /// <remarks>Make sure to fully enumerate the <see cref="RiakStreamedMapReduceResult"/> or connections may be left open.</remarks>
        public RiakStreamedMapReduceResult StreamMapReduce(RiakMapReduceQuery query)
        {
            return Async.StreamMapReduce(query).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public IEnumerable<string> ListBuckets()
        {
            return Async.ListBuckets()
                .ToEnumerable()
                .Where(x => !x.IsLeft)
                .Select(x => x.Right)
                .ToList();
        }

        /// <summary>
        /// Lists all buckets available on the Riak cluster. This uses an <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> 
        /// of <see cref="string"/> to lazy initialize the collection of bucket names. 
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> of <see cref="string"/> bucket names.
        /// </returns>
        /// <remarks>Buckets provide a logical namespace for keys. Listing buckets requires folding over all keys in a cluster and 
        /// reading a list of buckets from disk. This operation, while non-blocking in Riak 1.0 and newer, still produces considerable
        /// physical I/O and can take a long time. Callers should fully enumerate the collection or else close the connection when finished.</remarks>
        public IEnumerable<string> StreamListBuckets()
        {
            return Async.StreamListBuckets()
                .ToEnumerable()
                .Where(x => !x.IsLeft)
                .Select(x => x.Right)
                .ToList();
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
        public IEnumerable<string> ListKeys(string bucket)
        {
            return Async.ListKeys(bucket)
                .ToEnumerable()
                .Where(x => !x.IsLeft)
                .Select(x => x.Right)
                .ToList();
        }

        /// <summary>
        /// Performs a streaming list keys operation.
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/></returns>
        /// <remarks>While this streams results back to the client, alleviating pressure on Riak, this still relies on
        /// folding over all keys present in the Riak cluster. Use at your own risk. A better approach would be to
        /// use <see cref="ListKeysFromIndex"/></remarks>
        public IEnumerable<string> StreamListKeys(string bucket)
        {
            return Async.StreamListKeys(bucket)
                .ToEnumerable()
                .Where(x => !x.IsLeft)
                .Select(x => x.Right)
                .ToList();
        }

        /// <summary>
        /// Return a list of keys from the given bucket.
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns></returns>
        /// <remarks>This uses the $key special index instead of the list keys API to 
        /// quickly return an unsorted list of keys from Riak.</remarks>
        public IEnumerable<string> ListKeysFromIndex(string bucket)
        {
            return Async.ListKeysFromIndex(bucket)
                .ToEnumerable()
                .Where(x => !x.IsLeft)
                .Select(x => x.Right)
                .ToList();
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
        public RiakBucketProperties GetBucketProperties(string bucket)
        {
            return Async.GetBucketProperties(bucket).ConfigureAwait(false).GetAwaiter().GetResult();
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
        /// <param name='useHttp'>When true, CorrugatedIron will use the HTTP interface</param>
        public bool SetBucketProperties(string bucket, RiakBucketProperties properties, bool useHttp = false)
        {
            return Async.SetBucketProperties(bucket, properties, useHttp).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Reset the properties on a bucket back to their defaults.
        /// </summary>
        /// <param name="bucket">The name of the bucket to reset the properties on.</param>
        /// <param name="useHttp">Whether or not to use the HTTP interface to Riak. Set to true for Riak 1.3 and earlier</param> 
        /// <returns>An indication of success or failure.</returns>
        public bool ResetBucketProperties(string bucket, bool useHttp = false)
        {
            return Async.ResetBucketProperties(bucket, useHttp).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public IEnumerable<RiakObject> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks)
        {
            var links = Async.WalkLinks(riakObject, riakLinks)
                .Where(x => !x.IsLeft)
                .Select(x => x.Right)
                .ToEnumerable()
                .ToList();

            return links;
        }

        /// <summary>
        /// Get the server information from the connected cluster.
        /// </summary>
        /// <returns>Model containing information gathered from a node in the cluster.</returns>
        /// <remarks>This function will assume that all of the nodes in the cluster are running
        /// the same version of Riak. It will only get executed on a single node, and the content
        /// that is returned technically only relates to that node. All nodes in a cluster should
        /// run on the same version of Riak.</remarks>
        public RiakServerInfo GetServerInfo()
        {
            var result = Async.GetServerInfo().ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.IsLeft)
            {
                throw result.Left;
            }
            return result.Right;
        }

        /// <summary>
        /// Retrieve index results using the streaming interface.
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <param name="indexName">The index</param>
        /// <param name="value">The indexed value to search for</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/></param>
        /// <returns>A <see cref="RiakResult{T}"/> of <see cref="RiakStreamedIndexResult"/> containing an <see cref="IEnumerable{T}"/>
        /// of <see cref="RiakIndexKeyTerm"/></returns>
        /// <remarks>Make sure to fully enumerate the <see cref="IEnumerable{T}"/> of <see cref="RiakIndexKeyTerm"/>.</remarks>
        public RiakStreamedIndexResult StreamIndexGet(string bucket, string indexName, BigInteger value, RiakIndexGetOptions options = null)
        {
            var result = Async.StreamIndexGet(bucket, indexName, value, options).ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.IsLeft)
            {
                throw result.Left;
            }
            return result.Right;
        }

        /// <summary>
        /// Retrieve index results using the streaming interface.
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <param name="indexName">The index</param>
        /// <param name="value">The indexed value to search for</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/></param>
        /// <returns>A <see cref="RiakResult{T}"/> of <see cref="RiakStreamedIndexResult"/> containing an <see cref="IEnumerable{T}"/>
        /// of <see cref="RiakIndexKeyTerm"/></returns>
        /// <remarks>Make sure to fully enumerate the <see cref="IEnumerable{T}"/> of <see cref="RiakIndexKeyTerm"/>.</remarks>
        public RiakStreamedIndexResult StreamIndexGet(string bucket, string indexName, string value, RiakIndexGetOptions options = null)
        {
            var result = Async.StreamIndexGet(bucket, indexName, value, options).ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.IsLeft)
            {
                throw result.Left;
            }
            return result.Right;
        }

        /// <summary>
        /// Retrieve index results using the streaming interface.
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <param name="indexName">The index</param>
        /// <param name="minValue">The start of the indexed range to search for</param>
        /// <param name="maxValue">The end of the indexed range to search for</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/></param>
        /// <returns>A <see cref="RiakResult{T}"/> of <see cref="RiakStreamedIndexResult"/> containing an <see cref="IEnumerable{T}"/>
        /// of <see cref="RiakIndexKeyTerm"/></returns>
        /// <remarks>Make sure to fully enumerate the <see cref="IEnumerable{T}"/> of <see cref="RiakIndexKeyTerm"/>.</remarks>
        public RiakStreamedIndexResult StreamIndexGet(string bucket, string indexName, BigInteger minValue, BigInteger maxValue, RiakIndexGetOptions options = null)
        {
            var result = Async.StreamIndexGet(bucket, indexName, minValue, maxValue, options).ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.IsLeft)
            {
                throw result.Left;
            }
            return result.Right;
        }

        /// <summary>
        /// Retrieve index results using the streaming interface.
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <param name="indexName">The index</param>
        /// <param name="minValue">The start of the indexed range to search for</param>
        /// <param name="maxValue">The end of the indexed range to search for</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/></param>
        /// <returns>A <see cref="RiakResult{T}"/> of <see cref="RiakStreamedIndexResult"/> containing an <see cref="IEnumerable{T}"/>
        /// of <see cref="RiakIndexKeyTerm"/></returns>
        /// <remarks>Make sure to fully enumerate the <see cref="IEnumerable{T}"/> of <see cref="RiakIndexKeyTerm"/>.</remarks>
        public RiakStreamedIndexResult StreamIndexGet(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null)
        {
            var result = Async.StreamIndexGet(bucket, indexName, minValue, maxValue, options).ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.IsLeft)
            {
                throw result.Left;
            }
            return result.Right;
        }

        /// <summary>
        /// Retrieve a range of indexed values.
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <param name="indexName">The index</param>
        /// <param name="minValue">The start of the indexed range to search for</param>
        /// <param name="maxValue">The end of the indexed range to search for</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/></param>
        /// <returns>A <see cref="RiakResult{T}"/> of <see cref="RiakIndexResult"/></returns>
        public RiakIndexResult IndexGet(string bucket, string indexName, string minValue, string maxValue, RiakIndexGetOptions options = null)
        {
            var result = Async.IndexGet(bucket, indexName, minValue, maxValue, options).ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.IsLeft)
            {
                throw result.Left;
            }
            return result.Right;
        }

        /// <summary>
        /// Retrieve a range of indexed values.
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <param name="indexName">The index</param>
        /// <param name="minValue">The start of the indexed range to search for</param>
        /// <param name="maxValue">The end of the indexed range to search for</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/></param>
        /// <returns>A <see cref="RiakResult{T}"/> of <see cref="RiakIndexResult"/></returns>
        public RiakIndexResult IndexGet(string bucket, string indexName, BigInteger minValue, BigInteger maxValue, RiakIndexGetOptions options = null)
        {
            var result = Async.IndexGet(bucket, indexName, minValue, maxValue, options).ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.IsLeft)
            {
                throw result.Left;
            }
            return result.Right;
        }

        /// <summary>
        /// Retrieve a indexed values
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <param name="indexName">The index</param>
        /// <param name="value">The indexed value to search for</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/></param>
        /// <returns>A <see cref="RiakResult{T}"/> of <see cref="RiakIndexResult"/></returns>
        public RiakIndexResult IndexGet(string bucket, string indexName, string value, RiakIndexGetOptions options = null)
        {
            var result = Async.IndexGet(bucket, indexName, value, options).ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.IsLeft)
            {
                throw result.Left;
            }
            return result.Right;
        }

        /// <summary>
        /// Retrieve a indexed values
        /// </summary>
        /// <param name="bucket">The bucket</param>
        /// <param name="indexName">The index</param>
        /// <param name="value">The indexed value to search for</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/></param>
        /// <returns>A <see cref="RiakResult{T}"/> of <see cref="RiakIndexResult"/></returns>
        public RiakIndexResult IndexGet(string bucket, string indexName, BigInteger value, RiakIndexGetOptions options = null)
        {
            var result = Async.IndexGet(bucket, indexName, value, options).ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.IsLeft)
            {
                throw result.Left;
            }
            return result.Right;
        }

        internal static RiakGetOptions DefaultGetOptions()
        {
            return (new RiakGetOptions()).SetR(RiakConstants.Defaults.RVal);
        }

        public void Dispose()
        {
        }
    }
}