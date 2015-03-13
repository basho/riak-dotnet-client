// <copyright file="RiakCounterResult.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models
{
    /// <summary>
    /// Represents the data returned from a counter operation.
    /// </summary>
    public class RiakCounterResult
    {
        private readonly RiakResult<RiakObject> result;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakCounterResult"/> class.
        /// </summary>
        /// <param name="result">The <see cref="RiakResult{T}"/> to populate the new <see cref="RiakCounterResult"/> from.</param>
        public RiakCounterResult(RiakResult<RiakObject> result)
        {
            this.result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakCounterResult"/> class.
        /// </summary>
        /// <param name="result">The <see cref="RiakResult{T}"/> to populate the new <see cref="RiakCounterResult"/> from.</param>
        /// <param name="value">The current value of the counter.</param>
        public RiakCounterResult(RiakResult<RiakObject> result, long? value) : this(result)
        {
            this.Value = value;
        }

        /// <summary>
        /// The <see cref="RiakResult{T}"/> of the counter operation.
        /// </summary>
        public RiakResult<RiakObject> Result
        {
            get { return result; }
        }

        /// <summary>
        /// The value of the counter (if the operation returned it).
        /// </summary>
        public long? Value
        {
            get; set;
        }
    }
}
