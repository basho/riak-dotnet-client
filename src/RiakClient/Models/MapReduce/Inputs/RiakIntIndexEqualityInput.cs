// <copyright file="RiakIntIndexEqualityInput.cs" company="Basho Technologies, Inc.">
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
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an integer secondary index match query mapreduce input.
    /// </summary>
    public class RiakIntIndexEqualityInput : RiakIndexInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakIntIndexEqualityInput"/> class.
        /// </summary>
        /// <param name="bucket">The bucket that contains the <paramref name="index"/> to query.</param>
        /// <param name="index">
        /// The index to query. The output of that query will be used as input for the mapreduce job.
        /// </param>
        /// <param name="key">The index key to query for.</param>
        [Obsolete("Use the constructor that accepts a RiakIndexId instead. This will be removed in the next version.")]
        public RiakIntIndexEqualityInput(string bucket, string index, BigInteger key)
            : this(new RiakIndexId(bucket, index), key)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakIntIndexEqualityInput"/> class.
        /// </summary>
        /// <param name="indexId">
        /// The <see cref="RiakIndexId"/> that specifies which index to query.
        /// The output of that query will be used as input for the mapreduce job.
        /// </param>
        /// <param name="key">The index key to query for.</param>
        public RiakIntIndexEqualityInput(RiakIndexId indexId, BigInteger key)
            : base(indexId.ToIntIndexId())
        {
            Key = key;
        }

        // TODO: immutable

        /// <summary>
        /// The index key to query for.
        /// </summary>
        public BigInteger Key { get; set; }

        /// <inheritdoc/>
        public override JsonWriter WriteJson(JsonWriter writer)
        {
            WriteIndexHeaderJson(writer);

            writer.WritePropertyName("key");
            writer.WriteValue(Key.ToString());
            writer.WriteEndObject();

            return writer;
        }
    }
}
