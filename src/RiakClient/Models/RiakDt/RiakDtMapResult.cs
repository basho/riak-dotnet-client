// <copyright file="RiakDtMapResult.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

namespace RiakClient.Models.RiakDt
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents an operation result for a Riak Map data type.
    /// </summary>
    public class RiakDtMapResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakDtMapResult"/> class.
        /// </summary>
        /// <param name="result">The operation result.</param>
        /// <param name="context">The current context, if one was returned with the operation result.</param>
        /// <param name="values">The current collection of <see cref="RiakDtMapEntry"/>s, if they were returned with the operation result.</param>
        public RiakDtMapResult(
            RiakResult<RiakObject> result,
            byte[] context = null,
            List<RiakDtMapEntry> values = null)
        {
            Result = result;

            if (context != null)
            {
                Context = context;
            }

            Values = values ?? new List<RiakDtMapEntry>();
        }

        /// <summary>
        /// The operation result.
        /// </summary>
        public RiakResult<RiakObject> Result { get; private set; }

        /// <summary>
        /// The updated data type context.
        /// </summary>
        public byte[] Context { get; internal set; }

        /// <summary>
        /// A collection of <see cref="RiakDtMapEntry"/>, which represent the updated map fields.
        /// </summary>
        public List<RiakDtMapEntry> Values { get; internal set; }
    }
}
