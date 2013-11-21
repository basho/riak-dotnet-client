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
using System.Numerics;
using CorrugatedIron.Util;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public static class RiakIndex
    {

        public static RiakIndexInput Match(string bucket, string index, string key)
        {
            return new RiakBinIndexEqualityInput(bucket, index, key);
        }

        public static RiakIndexInput Range(string bucket, string index, string start, string end)
        {
            return new RiakBinIndexRangeInput(bucket, index, start, end);
        }

        public static RiakIndexInput Match(string bucket, string index, BigInteger key)
        {
            return new RiakIntIndexEqualityInput(bucket, index, key);
        }

        public static RiakIndexInput Range(string bucket, string index, BigInteger start, BigInteger end)
        {
            return new RiakIntIndexRangeInput(bucket, index, start, end);
        }

        /// <summary>
        /// Create a Map reduce input phase to retrieve all keys from a bucket
        /// </summary>
        /// <returns>The keys.</returns>
        /// <param name="bucket">Bucket.</param>
        /// <remarks><para>This is a wrapper around a range query on the $key index
        /// in Riak. Working with secondary indices requires that users enable
        /// the LevelDB backend and restart their cluster.</para>
        /// <para>This makes the assumption that all keys fall between the Unicode
        /// characters \u000000 and \u10FFFF (16-bit Unicode). This would typically
        /// be used in a stand alone Map phase to return all keys as a List&lt;RiakObjectId&gt;.
        /// See https://gist.github.com/peschkaj/4772825 for a working example.
        /// </remarks>
        public static RiakIndexInput AllKeys(string bucket)
        {
            return Match(bucket, RiakConstants.SystemIndexKeys.RiakBucketIndex, bucket);
        }

        /// <summary>
        /// Retrieve a list of keys between the start and end values
        /// </summary>
        /// <param name="bucket">Bucket name</param>
        /// <param name="start">Beginning of key range to be retrieved</param>
        /// <param name="end">End of key range being retrieved</param>
        /// <remarks>This is a wrapper around a range query on the $key index
        /// in Riak. Working with secondary indices requires that users enable
        /// the LevelDB backend and restart their cluster.</remarks>
        public static RiakIndexInput Keys(string bucket, string start, string end) 
        {
            return new RiakBinIndexRangeInput(bucket, RiakConstants.SystemIndexKeys.RiakKeysIndex, start, end);
        }
    }
}
