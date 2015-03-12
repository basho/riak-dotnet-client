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

    /// <summary>
    /// An asyncronous interface of <see cref="IRiakClient"/>. 
    /// Wraps all methods with a version of <see cref="TaskFactory.StartNew{TResult}(Func{TResult})"/>.
    /// </summary>
    public interface IRiakAsyncClient
    {
        /// <summary>
        /// Ping this instance of Riak.
        /// </summary>
        /// <description>
        /// Ping can be used to ensure that there is an operational Riak node
        /// present at the other end of the client. It's important to note that this will ping
        /// any Riak node in the cluster and a specific node cannot be specified by the user.
        /// Do not use this method to determine individual node health.
        /// </description>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that will contain a <see cref="RiakResult"/>. 
        /// Check <see cref="RiakResult.IsSuccess"/> to see if the operation was a success.
        /// </returns>
        Task<RiakResult> Ping();

        /// <summary>
        /// Get the specified <paramref name="key"/> from the <paramref name="bucket"/>.
        /// Optionally can be read from rVal instances. By default, the server's
        /// r-value will be used, but can be overridden by rVal.
        /// </summary>
        /// <param name="bucket">The name of the bucket containing the <paramref name="key"/>.</param>
        /// <param name="key">The key of the object.</param>
        /// <param name="options">
        /// The <see cref="RiakGetOptions" /> responsible for configuring the semantics of this single get request. 
        /// These options will override any previously defined bucket configuration properties.
        /// </param>
        /// <remarks>If a node does not respond, that does not necessarily mean that the 
        /// <paramref name="bucket"/>/<paramref name="key"/> combination is not available. 
        /// It simply means that fewer than R/PR nodes responded to the read request. 
        /// See <see cref="RiakGetOptions" /> for information on how different options change Riak's default behavior.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{RiakObject}"/>,  
        /// which will contain the found <see cref="RiakObject"/>.
        /// </returns>
        Task<RiakResult<RiakObject>> Get(string bucket, string key, RiakGetOptions options = null);

        /// <summary>
        /// Retrieve the specified object from Riak.
        /// </summary>
        /// <param name="objectId">
        /// Object identifier made up of a key, bucket, and bucket type. <see cref="RiakObjectId"/>
        /// </param>
        /// <param name="options">
        /// The <see cref="RiakGetOptions" /> responsible for configuring the semantics of this single get request.
        /// These options will override any previously defined bucket configuration properties.
        /// </param>
        /// <remarks>
        /// If a node does not respond, that does not necessarily mean that the 
        /// <paramref name="objectId"/> is not available. It simply means
        /// that fewer than <paramref name="rVal" /> nodes responded to the read request. Unfortunately, 
        /// the Riak API does not allow us to distinguish between a 404 resulting from less than <paramref name="rVal"/>
        /// nodes successfully responding and an <paramref name="objectId"/> not being found in Riak.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{RiakObject}"/>, 
        /// which will contain the found <see cref="RiakObject"/>.
        /// </returns>
        Task<RiakResult<RiakObject>> Get(RiakObjectId objectId, RiakGetOptions options = null);

        /// <summary>
        /// Retrieve multiple objects from Riak.
        /// </summary>
        /// <param name="objectIds">
        /// An <see href="System.Collections.Generic.IEnumerable{T}"/> of <see cref="RiakObjectId"/> to be retrieved.
        /// </param>
        /// <param name="options">
        /// The <see cref="RiakGetOptions" /> responsible for configuring the semantics of this single get request. 
        /// These options will override any previously defined bucket configuration properties.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain an <see cref="System.Collections.Generic.IEnumerable{T}"/> of <see cref="RiakResult{T}"/>.
        /// You should verify the success or failure of each result separately.
        /// </returns>
        /// <remarks>
        /// Riak does not support multi get behavior. RiakClient's multi get functionality wraps multiple
        /// get requests and returns results as an IEnumerable{RiakResult{RiakObject}}. Callers should be aware that
        /// this may result in partial success - all results should be evaluated individually in the calling application.
        /// In addition, applications should plan for multiple failures or multiple cases of siblings being present.
        /// </remarks>
        Task<IEnumerable<RiakResult<RiakObject>>> Get(IEnumerable<RiakObjectId> objectIds, RiakGetOptions options = null);

        /// <summary>
        /// Increments a Riak counter. 
        /// </summary>
        /// <param name="bucket">The bucket.</param>
        /// <param name="counter">The name of the counter.</param>
        /// <param name="amount">The amount to increment/decrement the counter.</param>
        /// <param name="options">The <see cref="RiakCounterUpdateOptions"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakCounterResult"/>.</returns>
        /// <remarks>Only available in Riak 1.4+. If the counter is not initialized, then the counter will be initialized to 0 and then incremented.</remarks>
        Task<RiakCounterResult> IncrementCounter(string bucket, string counter, long amount, RiakCounterUpdateOptions options = null);

        /// <summary>
        /// Returns the value of a counter.
        /// </summary>
        /// <param name="bucket">The bucket.</param>
        /// <param name="counter">The counter.</param>
        /// <param name="options"><see cref="RiakCounterGetOptions"/> describing how to read the counter.</param>
        /// <returns>A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakCounterResult"/>.</returns>
        /// <remarks>Only available in Riak 1.4+.</remarks>
        Task<RiakCounterResult> GetCounter(string bucket, string counter, RiakCounterGetOptions options = null);

        /// <summary>
        /// Persist a <see cref="RiakObject"/> to Riak using the specific <see cref="RiakPutOptions"/>.
        /// </summary>
        /// <param name="value">The <see cref="RiakObject"/> to save.</param>
        /// <param name="options">
        /// The <see cref="RiakPutOptions" /> responsible for configuring the semantics of this single put request. 
        /// These options will override any previously defined bucket configuration properties.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult"/> 
        /// detailing the success or failure of the operation.
        /// </returns>
        Task<RiakResult<RiakObject>> Put(RiakObject value, RiakPutOptions options = null);

        /// <summary>
        /// Persist an <see href="System.Collections.Generic.IEnumerable{T}"/> of <see cref="RiakObject"/>s to Riak.
        /// </summary>
        /// <param name="values">
        /// The <see href="System.Collections.Generic.IEnumerable{T}"/> of <see cref="RiakObject"/>s to save.
        /// </param>
        /// <param name="options">
        /// The <see cref="RiakPutOptions" /> responsible for configuring the semantics of this single put request. 
        /// These options will override any previously defined bucket configuration properties.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain an 
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> of <see cref="RiakResult{T}"/>. 
        /// You should verify the success or failure of each result separately.
        /// </returns>
        /// <remarks>
        /// Riak does not support multi put behavior. RiakClient's multi put functionality wraps multiple
        /// put requests and returns results as an IEnumerable{RiakResult{RiakObject}}. Callers should be aware that
        /// this may result in partial success - all results should be evaluated individually in the calling application.
        /// In addition, applications should plan for multiple failures or multiple cases of siblings being present.
        /// </remarks>
        Task<IEnumerable<RiakResult<RiakObject>>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null);

        /// <summary>
        /// Delete the data identified by the <paramref name="riakObject"/>
        /// </summary>
        /// <param name="riakObject">The object to delete.</param>
        /// <param name="options">
        /// The <see cref="RiakDeleteOptions" /> responsible for configuring the semantics of this single delete request. 
        /// These options will override any previously defined bucket configuration properties.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult"/> 
        /// detailing the success or failure of the operation.
        /// </returns>
        Task<RiakResult> Delete(RiakObject riakObject, RiakDeleteOptions options = null);

        /// <summary>
        /// Delete the object identified by <paramref name="key"/> from a <paramref name="bucket"/>.
        /// </summary>
        /// <param name="bucket">The name of the bucket containing the <paramref name="key"/>.</param>
        /// <param name="key">The key of the object to be deleted.</param>
        /// <param name="options">
        /// The <see cref="RiakDeleteOptions" /> responsible for configuring the semantics of this single delete request. 
        /// These options will override any previously defined bucket configuration properties.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult"/> 
        /// detailing the success or failure of the operation.
        /// </returns>
        Task<RiakResult> Delete(string bucket, string key, RiakDeleteOptions options = null);

        /// <summary>
        /// Delete the object identified by the <paramref name="objectId"/>.
        /// </summary>
        /// <param name="objectId">The <see cref="RiakObjectId"/> of the object to be deleted.</param>
        /// <param name="options">
        /// The <see cref="RiakDeleteOptions" /> responsible for configuring the semantics of this single delete request. 
        /// These options will override any previously defined bucket configuration properties.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult"/> 
        /// detailing the success or failure of the operation.
        /// </returns>
        Task<RiakResult> Delete(RiakObjectId objectId, RiakDeleteOptions options = null);

        /// <summary>
        /// Delete multiple objects identified by a <see cref="System.Collections.Generic.IEnumerable{T}"/> of <see cref="RiakObjectId"/>.
        /// </summary>
        /// <param name="objectIds">
        /// A <see cref="System.Collections.Generic.IEnumerable{T}"/> of <see cref="RiakObjectId"/>s to delete.
        /// </param>
        /// <param name="options">
        /// The <see cref="RiakDeleteOptions" /> responsible for configuring the semantics of this delete request. 
        /// These options will override any previously defined bucket configuration properties.
        /// </param>  
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain an <see cref="System.Collections.Generic.IEnumerable{T}"/> 
        /// of <see cref="RiakResult"/>, one for each individual delete operation.
        /// You should verify the success or failure of each result separately.
        /// </returns>
        Task<IEnumerable<RiakResult>> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null);

        /// <summary>
        /// Perform a Riak Search query.
        /// </summary>
        /// <param name="search">The <see cref="RiakSearchRequest"/> to perform.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{T}"/> 
        /// of <see cref="RiakSearchResult"/>.
        /// </returns>
        Task<RiakResult<RiakSearchResult>> Search(RiakSearchRequest search);

        /// <summary>
        /// Execute a map reduce query.
        /// </summary>
        /// <param name="query">The <see cref="RiakMapReduceQuery"/> to perform.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a  <see cref="RiakResult{T}"/> 
        /// of <see cref="RiakMapReduceResult"/>.
        /// </returns>
        Task<RiakResult<RiakMapReduceResult>> MapReduce(RiakMapReduceQuery query);

        /// <summary>
        /// Perform a map reduce query and stream the results.
        /// </summary>
        /// <param name="query">The map reduce query to perform.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{T}"/> 
        /// of <see cref="RiakStreamedMapReduceResult"/>.
        /// </returns>
        /// <remarks>
        /// <b>Make sure to fully enumerate the <see cref="RiakStreamedMapReduceResult"/> 
        /// or connections may be left open.</b>
        /// </remarks>
        Task<RiakResult<RiakStreamedMapReduceResult>> StreamMapReduce(RiakMapReduceQuery query);

        /// <summary>
        /// Lists all buckets in the Default Bucket Type.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain an 
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> of <see cref="string"/> bucket names.
        /// </returns>
        /// <remarks>
        /// Buckets provide a logical namespace for keys. Listing buckets requires folding over 
        /// all keys in a cluster and reading a list of buckets from disk. 
        /// This operation, while non-blocking in Riak 1.0 and newer, still produces considerable
        /// physical I/O and can take a long time.
        /// </remarks>
        Task<RiakResult<IEnumerable<string>>> ListBuckets();

        // TODO: Implement RiakResult<IEnumerable<string>> ListBuckets(string bucketType); ?

        /// <summary>
        /// Lists all buckets available on the Riak cluster. This uses an <see cref="System.Collections.Generic.IEnumerable{T}"/> 
        /// of <see cref="string"/> to lazy initialize the collection of bucket names. 
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain an 
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> of <see cref="string"/> bucket names.
        /// </returns>
        /// <remarks>
        /// Buckets provide a logical namespace for keys. Listing buckets requires folding over all keys in a 
        /// cluster and reading a list of buckets from disk. This operation, while non-blocking in Riak 1.0 and 
        /// newer, still produces considerable physical I/O and can take a long time. 
        /// <b>Callers should fully enumerate the collection or else close the connection when finished.</b>
        /// </remarks>
        Task<RiakResult<IEnumerable<string>>> StreamListBuckets();

        /// <summary>
        /// Lists all keys in the specified <paramref name="bucket"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain an 
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> of <see cref="string"/> keys.
        /// </returns>
        /// <param name="bucket">
        /// The bucket to list keys from.
        /// </param>
        /// <remarks>
        /// ListKeys is an expensive operation that requires folding over all data in the Riak cluster to produce
        /// a list of keys. This operation, while cheaper in Riak 1.0 than in earlier versions of Riak, should be avoided.
        /// </remarks>
        Task<RiakResult<IEnumerable<string>>> ListKeys(string bucket);

        // TODO: Implement RiakResult<IEnumerable<string>> ListKeys(string bucketType, string bucket); ?

        /// <summary>
        /// Performs a streaming list keys operation.
        /// </summary>
        /// <param name="bucket">The bucket to list keys from.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain an 
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> of keys.
        /// </returns>
        /// <remarks>
        /// While this streams results back to the client, alleviating pressure on Riak, this still relies on
        /// folding over all keys present in the Riak cluster. Use at your own risk. If you are using the LevelDB backend,
        /// a better approach would be to use <see cref="ListKeysFromIndex(string)"/> in most cases.
        /// </remarks>
        Task<RiakResult<IEnumerable<string>>> StreamListKeys(string bucket);

        // TODO: Implement RiakResult<IEnumerable<string>> StreamListKeys(string bucketType, string bucket); ?

        /// <summary>
        /// Returns all properties for a <paramref name="bucket"/>.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/>, that will contain the bucket properties.</returns>
        /// <param name="bucket">The name of the bucket to get properties for.</param>
        Task<RiakResult<RiakBucketProperties>> GetBucketProperties(string bucket);

        // TODO: Implement RiakResult<RiakBucketProperties> GetBucketProperties(string bucketType, string bucket); ?

        /// <summary>
        /// Sets the <see cref="RiakBucketProperties"/> properties of a <paramref name="bucket"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult"/> 
        /// detailing the success or failure of the operation.
        /// </returns>
        /// <param name="bucket">The name of the bucket to set the properties on.</param>
        /// <param name="properties">
        /// The properties to set. Note that only those properties explicitly set in the 
        /// <see cref="RiakBucketProperties"/> will be set on the bucket. 
        /// Those not set in the object will retain their values on Riak's side.
        /// </param>
        Task<RiakResult> SetBucketProperties(string bucket, RiakBucketProperties properties);

        // TODO: Implement RiakResult SetBucketProperties(string bucketType, string bucket, RiakBucketProperties properties);? 

        /// <summary>
        /// Reset the properties on a bucket back to their defaults.
        /// </summary>
        /// <param name="bucket">The name of the bucket to reset the properties on.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult"/> 
        /// detailing the success or failure of the operation.
        /// </returns>
        Task<RiakResult> ResetBucketProperties(string bucket);

        // TODO: Implement RiakResult ResetBucketProperties(string bucketType, string bucket);? 

        /// <summary>
        /// Retrieve arbitrarily deep list of links for a <see cref="RiakObject"/>
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain an <see cref="System.Collections.Generic.IList{T}"/> 
        /// of <see cref="RiakObject"/> identified by the input <paramref name="riakLinks"/>.
        /// </returns>
        /// <param name="riakObject">The initial object to use for the beginning of the link walking.</param>
        /// <param name="riakLinks">A list of link definitions.</param>
        /// <remarks>Refer to http://wiki.basho.com/Links-and-Link-Walking.html for more information.</remarks>
        [Obsolete("Linkwalking has been deprecated as of Riak 2.0. This method will be removed in the next major version.")]
        Task<RiakResult<IList<RiakObject>>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks);
        
        /// <summary>
        /// Get the server information from the connected cluster.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakServerInfo"/> 
        /// object containing information gathered from a node in the cluster.
        /// </returns>
        /// <remarks>
        /// This function will assume that all of the nodes in the cluster are running
        /// the same version of Riak. It will only get executed on a single node, and the content
        /// that is returned technically only relates to that node. All nodes in a cluster should
        /// run on the same version of Riak.
        /// </remarks>
        Task<RiakResult<RiakServerInfo>> GetServerInfo();

        /// <summary>
        /// Query a secondary index for a specific integer value.
        /// </summary>
        /// <param name="index">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="value">The integer value to query for.</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/> responsible for configuring the semantics of this single index request.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{T}"/> 
        /// of <see cref="RiakIndexResult"/>.
        /// </returns>
        Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, BigInteger value, RiakIndexGetOptions options = null);

        /// <summary>
        /// Query a secondary index for a specific string value.
        /// </summary>
        /// <param name="index">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="value">The string value to query for.</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/> responsible for configuring the semantics of this single index request.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{T}"/> 
        /// of <see cref="RiakIndexResult"/>.
        /// </returns>
        Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, string value, RiakIndexGetOptions options = null);

        /// <summary>
        /// Query a secondary index for a range of integer values.
        /// </summary>
        /// <param name="index">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="min">The inclusive min integer value for the query range.</param>
        /// <param name="max">The inclusive max integer value for the query range.</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/> responsible for configuring the semantics of this single index request.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{T}"/> 
        /// of <see cref="RiakIndexResult"/>.
        /// </returns>
        Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, BigInteger min, BigInteger max, RiakIndexGetOptions options = null);

        /// <summary>
        /// Query a secondary index for a range of string values.
        /// </summary>
        /// <param name="index">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="min">The inclusive min string value for the query range.</param>
        /// <param name="max">The inclusive max string value for the query range.</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/> responsible for configuring the semantics of this single index request.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{T}"/> 
        /// of <see cref="RiakIndexResult"/>.
        /// </returns>
        Task<RiakResult<RiakIndexResult>> GetSecondaryIndex(RiakIndexId index, string min, string max, RiakIndexGetOptions options = null);

        /// <summary>
        /// Query a secondary index for a specific integer value, and stream the results back.
        /// </summary>
        /// <param name="index">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="value">The integer value to query for.</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/> responsible for configuring the semantics of this single index request.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{T}"/> 
        /// of <see cref="RiakStreamedIndexResult"/>.
        /// </returns>
        Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, BigInteger value, RiakIndexGetOptions options = null);

        /// <summary>
        /// Query a secondary index for a specific string value, and stream the results back.
        /// </summary>
        /// <param name="index">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="value">The string value to query for.</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/> responsible for configuring the semantics of this single index request.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{T}"/> 
        /// of <see cref="RiakStreamedIndexResult"/>.
        /// </returns>
        Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, string value, RiakIndexGetOptions options = null);

        /// <summary>
        /// Query a secondary index for a range of integer values, and stream the results back.
        /// </summary>
        /// <param name="index">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="min">The inclusive min integer value for the query range.</param>
        /// <param name="max">The inclusive max integer value for the query range.</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/> responsible for configuring the semantics of this single index request.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{T}"/> 
        /// of <see cref="RiakStreamedIndexResult"/>.
        /// </returns>
        Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, BigInteger min, BigInteger max, RiakIndexGetOptions options = null);

        /// <summary>
        /// Query a secondary index for a range of string values, and stream the results back.
        /// </summary>
        /// <param name="index">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="min">The inclusive min string value for the query range.</param>
        /// <param name="max">The inclusive max string value for the query range.</param>
        /// <param name="options">The <see cref="RiakIndexGetOptions"/> responsible for configuring the semantics of this single index request.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain a <see cref="RiakResult{T}"/> 
        /// of <see cref="RiakStreamedIndexResult"/>.
        /// </returns>
        Task<RiakResult<RiakStreamedIndexResult>> StreamGetSecondaryIndex(RiakIndexId index, string min, string max, RiakIndexGetOptions options = null);

        /// <summary>
        /// Return a list of keys from the given bucket.
        /// </summary>
        /// <param name="bucket">The bucket to list keys from.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/>, that will contain an 
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> of keys.
        /// </returns>
        /// <remarks>
        /// This uses the $key special index instead of the list keys API to 
        /// quickly return an unsorted list of keys from Riak. Only works with a LevelDB backend.
        /// </remarks>
        Task<RiakResult<IList<string>>> ListKeysFromIndex(string bucket);

        // TODO: Implement RiakResult<IList<string>> ListKeysFromIndex(string bucketType, string bucket); ?

        /// <summary>
        /// Used to create a batched set of actions to be sent to a Riak cluster.
        /// </summary>
        /// <param name="batchAction">An action that wraps all the operations to batch together.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        Task Batch(Action<IRiakBatchClient> batchAction);

        /// <summary>
        /// Used to create a batched set of actions to be sent to a Riak cluster.
        /// </summary>
        /// <typeparam name="T">The <paramref name="batchFun"/>'s return type.</typeparam>
        /// <param name="batchFunction">A func that wraps all the operations to batch together.</param>
        /// <returns>A <see cref="Task{TResult}"/> that will contain the return value of <paramref name="batchFun"/>.</returns>
        Task<T> Batch<T>(Func<IRiakBatchClient, T> batchFunction);

        // TODO: We're missing everything below:
        /*
        /// <summary>
        /// Fetch a Counter Data Type object from the provided address. 
        /// </summary>
        /// <param name="bucketType">The name of the bucket type containing the <paramref name="bucket"/>.</param>
        /// <param name="bucket">The name of the bucket containing the <paramref name="key"/>.</param>
        /// <param name="key">The key of the data type object.</param>
        /// <param name="options">The <see cref="RiakDtFetchOptions"/> responsible for configuring the semantics of this data type fetch request.</param>
        /// <returns>A <see cref="RiakCounterResult"/> detailing the operation result and current counter value.</returns>
        RiakCounterResult DtFetchCounter(string bucketType, string bucket, string key, RiakDtFetchOptions options = null);

        /// <summary>
        /// Fetch a Counter Data Type object from the provided address.
        /// </summary>
        /// <param name="objectId">The <see cref="RiakObjectId"/> of the counter to fetch.</param>
        /// <param name="options">The <see cref="RiakDtFetchOptions"/> responsible for configuring the semantics of this data type fetch request.</param>
        /// <returns>A <see cref="RiakCounterResult"/> detailing the operation result and current counter value.</returns>
        RiakCounterResult DtFetchCounter(RiakObjectId objectId, RiakDtFetchOptions options = null);

        /// <summary>
        /// Update a Counter Data Type object at the provided address.
        /// </summary>
        /// <param name="bucketType">The name of the bucket type containing the <paramref name="bucket"/>.</param>
        /// <param name="bucket">The name of the bucket containing the <paramref name="key"/>.</param>
        /// <param name="key">The key of the data type object.</param>
        /// <param name="amount">The delta to apply to the counter. To add 1 to the counter, use "1". To subtract 5, use "-5".</param>
        /// <param name="options">The <see cref="RiakDtUpdateOptions"/> responsible for configuring the semantics of this data type update request.</param>
        /// <returns>A <see cref="RiakCounterResult"/> detailing the operation result and current counter value.</returns>
        RiakCounterResult DtUpdateCounter(string bucketType, string bucket, string key, long amount, RiakDtUpdateOptions options = null);

        /// <summary>
        /// Update a Counter Data Type object at the provided address.
        /// </summary>
        /// <param name="objectId">The <see cref="RiakObjectId"/> of the counter to update.</param>
        /// <param name="amount">The delta to apply to the counter. To add 1 to the counter, use "1". To subtract 5, use "-5".</param>
        /// <param name="options">The <see cref="RiakDtUpdateOptions"/> responsible for configuring the semantics of this data type update request.</param>
        /// <returns>A <see cref="RiakCounterResult"/> detailing the operation result and current counter value.</returns>
        RiakCounterResult DtUpdateCounter(RiakObjectId objectId, long amount, RiakDtUpdateOptions options = null);

        /// <summary>
        /// Fetch a Set Data Type object at the provided address.
        /// </summary>
        /// <param name="bucketType">The name of the bucket type containing the <paramref name="bucket"/>.</param>
        /// <param name="bucket">The name of the bucket containing the <paramref name="key"/>.</param>
        /// <param name="key">The key of the data type object.</param>
        /// <param name="options">The <see cref="RiakDtFetchOptions"/> responsible for configuring the semantics of this data type fetch request.</param>
        /// <returns>A <see cref="RiakDtSetResult"/> detailing the operation result, current context, and set values.</returns>
        RiakDtSetResult DtFetchSet(string bucketType, string bucket, string key, RiakDtFetchOptions options = null);

        /// <summary>
        /// Fetch a Set Data Type object at the provided address.
        /// </summary>
        /// <param name="objectId">The <see cref="RiakObjectId"/> of the set to fetch.</param>
        /// <param name="options">The <see cref="RiakDtFetchOptions"/> responsible for configuring the semantics of this data type fetch request.</param>
        /// <returns>A <see cref="RiakDtSetResult"/> detailing the operation result, current context, and set values.</returns>
        RiakDtSetResult DtFetchSet(RiakObjectId objectId, RiakDtFetchOptions options = null);

        /// <summary>
        /// Update a Set Data Type object at the provided address.
        /// </summary>
        /// <typeparam name="T">The type of the objects being stored in the set.</typeparam>
        /// <param name="bucketType">The name of the bucket type containing the <paramref name="bucket"/>.</param>
        /// <param name="bucket">The name of the bucket containing the <paramref name="key"/>.</param>
        /// <param name="key">The key of the data type object.</param>
        /// <param name="serialize">A delegate to serialize the <paramref name="adds"/> and <paramref name="removes"/> lists from objects of type <typeparamref name="T"/> to a byte[].</param>
        /// <param name="context">The most recent known byte[] data type context for this object, to base this operation off of for causality merging.</param>
        /// <param name="adds">A <see cref="List{T}"/> of items to add to the set.</param>
        /// <param name="removes">A <see cref="List{T}"/> of items to remove from the set.</param>
        /// <param name="options">The <see cref="RiakDtUpdateOptions"/> responsible for configuring the semantics of this data type update request.</param>
        /// <returns>A <see cref="RiakDtSetResult"/> detailing the operation result, current context, and set values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> cannot be null if removing any item from the set.</exception>
        RiakDtSetResult DtUpdateSet<T>(string bucketType, string bucket, string key, SerializeObjectToByteArray<T> serialize, byte[] context, List<T> adds = null, List<T> removes = null, RiakDtUpdateOptions options = null);

        /// <summary>
        /// Update a Set Data Type object at the provided address.
        /// </summary>
        /// <typeparam name="T">The type of the objects being stored in the set.</typeparam>
        /// <param name="objectId">The <see cref="RiakObjectId"/> of the set to update.</param>
        /// <param name="serialize">A delegate to serialize the <paramref name="adds"/> and <paramref name="removes"/> lists from objects of type <typeparamref name="T"/> to a byte[].</param>
        /// <param name="context">The most recent known byte[] data type context for this object, to base this operation off of for causality merging.</param>
        /// <param name="adds">A <see cref="List{T}"/> of items to add to the set.</param>
        /// <param name="removes">A <see cref="List{T}"/> of items to remove from the set.</param>
        /// <param name="options">The <see cref="RiakDtUpdateOptions"/> responsible for configuring the semantics of this data type update request.</param>
        /// <returns>A <see cref="RiakDtSetResult"/> detailing the operation result, current context, and set values.</returns>
        /// <remarks>Removal of any item from the set requires that the <paramref name="context"/> be non-null, or else an <see cref="ArgumentNullException"/> will be thrown.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> cannot be null if removing any item from the set.</exception>
        RiakDtSetResult DtUpdateSet<T>(RiakObjectId objectId, SerializeObjectToByteArray<T> serialize, byte[] context, List<T> adds = null, List<T> removes = null, RiakDtUpdateOptions options = null);

        /// <summary>
        /// Fetch a Map Data Type object at the provided address.
        /// </summary>
        /// <param name="bucketType">The name of the bucket type containing the <paramref name="bucket"/>.</param>
        /// <param name="bucket">The name of the bucket containing the <paramref name="key"/>.</param>
        /// <param name="key">The key of the data type object.</param>
        /// <param name="options">The <see cref="RiakDtFetchOptions"/> responsible for configuring the semantics of this data type fetch request.</param>
        /// <returns>A <see cref="RiakDtMapResult"/> detailing the operation result, current context, and map values.</returns>
        RiakDtMapResult DtFetchMap(string bucketType, string bucket, string key, RiakDtFetchOptions options = null);

        /// <summary>
        /// Fetch a Map Data Type object at the provided address.
        /// </summary>
        /// <param name="objectId">The <see cref="RiakObjectId"/> of the map to fetch.</param>
        /// <param name="options">The <see cref="RiakDtFetchOptions"/> responsible for configuring the semantics of this data type fetch request.</param>
        /// <returns>A <see cref="RiakDtMapResult"/> detailing the operation result, current context, and map values.</returns>
        RiakDtMapResult DtFetchMap(RiakObjectId objectId, RiakDtFetchOptions options = null);

        /// <summary>
        /// Update a Map Data Type object at the provided address.
        /// </summary>
        /// <typeparam name="T">The type of the objects being stored in the map.</typeparam>
        /// <param name="bucketType">The name of the bucket type containing the <paramref name="bucket"/>.</param>
        /// <param name="bucket">The name of the bucket containing the <paramref name="key"/>.</param>
        /// <param name="key">The key of the data type object.</param>
        /// <param name="serialize">A delegate to serialize the <paramref name="updates"/> and <paramref name="removes"/> operation lists from objects of type <typeparamref name="T"/> to a byte[].</param>
        /// <param name="context">The most recent known byte[] data type context for this object, to base this operation off of for causality merging.</param>
        /// <param name="removes">A <see cref="List{T}"/> of <see cref="RiakDtMapField"/> to specify which fields to remove.</param>
        /// <param name="updates">A <see cref="List{T}"/> of <see cref="MapUpdate"/> to specify what updates to perform on the map.</param>
        /// <param name="options">The <see cref="RiakDtUpdateOptions"/> responsible for configuring the semantics of this data type update request.</param>
        /// <returns>A <see cref="RiakDtMapResult"/> detailing the operation result, current context, and map values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> cannot be null if removing any field from the map, nested maps, or removing any item from nested sets.</exception>
        RiakDtMapResult DtUpdateMap<T>(string bucketType, string bucket, string key, SerializeObjectToByteArray<T> serialize, byte[] context, List<RiakDtMapField> removes = null, List<MapUpdate> updates = null, RiakDtUpdateOptions options = null);

        /// <summary>
        /// Update a Map Data Type object at the provided address.
        /// </summary>
        /// <typeparam name="T">The type of the objects being stored in the map.</typeparam>
        /// <param name="objectId">The <see cref="RiakObjectId"/> of the map to update.</param>
        /// <param name="serialize">A delegate to serialize the <paramref name="updates"/> and <paramref name="removes"/> operation lists from objects of type <typeparamref name="T"/> to a byte[].</param>
        /// <param name="context">The most recent known byte[] data type context for this object, to base this operation off of for causality merging.</param>
        /// <param name="removes">A <see cref="List{T}"/> of <see cref="RiakDtMapField"/> to specify which fields to remove.</param>
        /// <param name="updates">A <see cref="List{T}"/> of <see cref="MapUpdate"/> to specify what updates to perform on the map.</param>
        /// <param name="options">The <see cref="RiakDtUpdateOptions"/> responsible for configuring the semantics of this data type update request.</param>
        /// <returns>A <see cref="RiakDtMapResult"/> detailing the operation result, current context, and map values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> cannot be null if removing any field from the map, nested maps, or removing any item from nested sets.</exception>
        RiakDtMapResult DtUpdateMap<T>(RiakObjectId objectId, SerializeObjectToByteArray<T> serialize, byte[] context, List<RiakDtMapField> removes = null, List<MapUpdate> updates = null, RiakDtUpdateOptions options = null);

        /// <summary>
        /// Fetches the specified search index from Riak. 
        /// </summary>
        /// <param name="indexName">The name of the index to retrieve.</param>
        /// <returns>A <see cref="RiakResult{T}"/> containing a <see cref="SearchIndexResult"/>.</returns>
        RiakResult<SearchIndexResult> GetSearchIndex(string indexName);

        /// <summary>
        /// Saves the specified <see cref="SearchIndex"/> to Riak.
        /// </summary>
        /// <param name="index">The <see cref="SearchIndex"/> to save.</param>
        /// <returns>A <see cref="RiakResult"/> detailing the success or failure of the operation.</returns>
        RiakResult PutSearchIndex(SearchIndex index);

        /// <summary>
        /// Deletes the specified <see cref="SearchIndex"/> from Riak.
        /// </summary>
        /// <param name="indexName">THe name of the search index to delete.</param>
        /// <returns>A <see cref="RiakResult"/> detailing the success or failure of the operation.</returns>
        RiakResult DeleteSearchIndex(string indexName);

        /// <summary>
        /// Fetches the specified <see cref="SearchSchema"/> from Riak.
        /// </summary>
        /// <param name="schemaName">The name of the schema to fetch.</param>
        /// <returns>A <see cref="RiakResult{T}"/> containing a <see cref="SearchSchema"/>.</returns>
        RiakResult<SearchSchema> GetSearchSchema(string schemaName);

        /// <summary>
        /// Stores the provided <see cref="SearchSchema"/> in Riak. 
        /// </summary>
        /// <param name="schema">The schema to store.</param>
        /// <returns>A <see cref="RiakResult"/> detailing the success or failure of the operation.</returns>
        RiakResult PutSearchSchema(SearchSchema schema);
        */
    }
}
