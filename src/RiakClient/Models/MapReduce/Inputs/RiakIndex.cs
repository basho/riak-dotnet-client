// <copyright file="RiakIndex.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.MapReduce.Inputs
{
    using System;
    using System.Numerics;

    /// <summary>
    /// Helps create secondary index mapreduce input queries. 
    /// </summary>
    public static class RiakIndex
    {
        /// <summary>
        /// Create a mapreduce input from a string secondary index match query.
        /// </summary>
        /// <param name="bucket">The bucket containing the <see cref="index"/> to query.</param>
        /// <param name="index">The index to query.</param>
        /// <param name="key">The index key to match for the query.</param>
        /// <returns></returns>
        [Obsolete("Use Match(RiakIndexId, key) instead. This will be removed in the next version.")]
        public static RiakIndexInput Match(string bucket, string index, string key)
        {
            return new RiakBinIndexEqualityInput(bucket, index, key);
        }

        /// <summary>
        /// Create a mapreduce input from a string secondary index range query.
        /// </summary>
        /// <param name="bucket">The bucket containing the <see cref="index"/> to query.</param>
        /// <param name="index">The index to query.</param>
        /// <param name="start">The lower bound of the key range for the query.</param>
        /// <param name="end">The upper bound of the key range for the query.</param>
        /// <returns></returns>
        [Obsolete("Use Range(RiakIndexId, start, end) instead. This will be removed in the next version.")]
        public static RiakIndexInput Range(string bucket, string index, string start, string end)
        {
            return new RiakBinIndexRangeInput(bucket, index, start, end);
        }

        /// <summary>
        /// Create a mapreduce input from an integer secondary index match query.
        /// </summary>
        /// <param name="bucket">The bucket containing the <see cref="index"/> to query.</param>
        /// <param name="index">The index to query.</param>
        /// <param name="key">The index key to match for the query.</param>
        /// <returns></returns>
        [Obsolete("Use Match(RiakIndexId, key) instead. This will be removed in the next version.")]
        public static RiakIndexInput Match(string bucket, string index, BigInteger key)
        {
            return new RiakIntIndexEqualityInput(bucket, index, key);
        }

        /// <summary>
        /// Create a mapreduce input from an integer secondary index range query.
        /// </summary>
        /// <param name="bucket">The bucket containing the <see cref="index"/> to query.</param>
        /// <param name="index">The index to query.</param>
        /// <param name="start">The lower bound of the key range for the query.</param>
        /// <param name="end">The upper bound of the key range for the query.</param>
        /// <returns></returns>
        [Obsolete("Use Range(RiakIndexId, start, end) instead. This will be removed in the next version.")]
        public static RiakIndexInput Range(string bucket, string index, BigInteger start, BigInteger end)
        {
            return new RiakIntIndexRangeInput(bucket, index, start, end);
        }

        /// <summary>
        /// Create a mapreduce input from a string secondary index match query.
        /// </summary>
        /// <param name="indexId">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="key">The index key to match for the query.</param>
        /// <returns></returns>
        public static RiakIndexInput Match(RiakIndexId indexId, string key)
        {
            return new RiakBinIndexEqualityInput(indexId, key);
        }

        /// <summary>
        /// Create a mapreduce input from a string secondary index range query.
        /// </summary>
        /// <param name="indexId">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="start">The lower bound of the key range for the query.</param>
        /// <param name="end">The upper bound of the key range for the query.</param>
        /// <returns></returns>
        public static RiakIndexInput Range(RiakIndexId indexId, string start, string end)
        {
            return new RiakBinIndexRangeInput(indexId, start, end);
        }

        /// <summary>
        /// Create a mapreduce input from an integer secondary index match query.
        /// </summary>
        /// <param name="indexId">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="key">The index key to match for the query.</param>
        /// <returns></returns>
        public static RiakIndexInput Match(RiakIndexId indexId, BigInteger key)
        {
            return new RiakIntIndexEqualityInput(indexId, key);
        }

        /// <summary>
        /// Create a mapreduce input from an integer secondary index range query.
        /// </summary>
        /// <param name="indexId">The <see cref="RiakIndexId"/> identifying the index to query.</param>
        /// <param name="start">The lower bound of the key range for the query.</param>
        /// <param name="end">The upper bound of the key range for the query.</param>
        /// <returns></returns>
        public static RiakIndexInput Range(RiakIndexId indexId, BigInteger start, BigInteger end)
        {
            return new RiakIntIndexRangeInput(indexId, start, end);
        }

        /// <summary>
        /// Create a Map reduce input phase to retrieve all keys from a bucket
        /// </summary>
        /// <returns>The keys.</returns>
        /// <param name="bucket">Bucket Name</param>
        /// <remarks><para>This is a wrapper around a range query on the $key index
        /// in Riak. Working with secondary indexes requires that users enable
        /// the LevelDB backend and restart their cluster.</para>
        /// <para>This makes the assumption that all keys fall between the Unicode
        /// characters \u000000 and \u10FFFF (16-bit Unicode). This would typically
        /// be used in a stand alone Map phase to return all keys as a List&lt;RiakObjectId&gt;.
        /// See https://gist.github.com/peschkaj/4772825 for a working example.
        /// </para></remarks>
        public static RiakIndexInput AllKeys(string bucket)
        {
            return AllKeys(null, bucket);
        }

        /// <summary>
        /// Create a Map reduce input phase to retrieve all keys from a bucket
        /// </summary>
        /// <returns>The keys.</returns>
        /// <param name="bucketType">Bucket Type.</param>
        /// <param name="bucket">Bucket Name.</param>
        /// <remarks><para>This is a wrapper around a range query on the $key index
        /// in Riak. Working with secondary indexes requires that users enable
        /// the LevelDB backend and restart their cluster.</para>
        /// <para>This makes the assumption that all keys fall between the Unicode
        /// characters \u000000 and \u10FFFF (16-bit Unicode). This would typically
        /// be used in a stand alone Map phase to return all keys as a List&lt;RiakObjectId&gt;.
        /// See https://gist.github.com/peschkaj/4772825 for a working example.
        /// </para></remarks>
        public static RiakIndexInput AllKeys(string bucketType, string bucket)
        {
            return Match(new RiakIndexId(bucketType, bucket, RiakConstants.SystemIndexKeys.RiakBucketIndex), bucket);
        }

        /// <summary>
        /// Retrieve a list of keys between the start and end values
        /// </summary>
        /// <param name="bucket">Bucket Name.</param>
        /// <param name="start">Beginning of key range to be retrieved</param>
        /// <param name="end">End of key range being retrieved</param>
        /// <remarks>This is a wrapper around a range query on the $key index
        /// in Riak. Working with secondary indexes requires that users enable
        /// the LevelDB backend and restart their cluster.</remarks>
        /// <returns>List of keys between start and end values</returns>
        public static RiakIndexInput Keys(string bucket, string start, string end)
        {
            return Keys(null, bucket, start, end);
        }

        /// <summary>
        /// Retrieve a list of keys between the start and end values
        /// </summary>
        /// <param name="bucketType">Bucket Type.</param>
        /// <param name="bucket">Bucket Name.</param>
        /// <param name="start">Beginning of key range to be retrieved</param>
        /// <param name="end">End of key range being retrieved</param>
        /// <remarks>This is a wrapper around a range query on the $key index
        /// in Riak. Working with secondary indexes requires that users enable
        /// the LevelDB backend and restart their cluster.</remarks>
        /// <returns>List of keys between the start and end values</returns>
        public static RiakIndexInput Keys(string bucketType, string bucket, string start, string end)
        {
            return Range(new RiakIndexId(bucketType, bucket, RiakConstants.SystemIndexKeys.RiakKeysIndex), start, end);
        }
    }
}
