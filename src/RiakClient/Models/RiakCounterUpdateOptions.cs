// <copyright file="RiakCounterUpdateOptions.cs" company="Basho Technologies, Inc.">
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
    using System.Runtime.InteropServices;
    using Messages;

    /// <summary>
    /// A collection of options that can be modified when updating a 1.4-style counter in Riak.
    /// </summary>
    [ComVisible(false)]
    public class RiakCounterUpdateOptions : RiakOptions<RiakCounterUpdateOptions>
    {
        /// <summary>
        /// Whether or not the updated value should be returned from the counter
        /// </summary>
        public bool? ReturnValue { get; private set; }

        /// <summary>
        /// A fluent setter for the <see cref="ReturnValue"/> property.
        /// Sets whether or not the updated value should be returned from the counter.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public RiakCounterUpdateOptions SetReturnValue(bool value)
        {
            ReturnValue = value;
            return this;
        }

        internal void Populate(RpbCounterUpdateReq request)
        {
            if (W != null)
            {
                request.w = W;
            }

            if (Dw != null)
            {
                request.dw = Dw;
            }

            if (Pw != null)
            {
                request.pw = Pw;
            }

            if (ReturnValue.HasValue)
            {
                request.returnvalue = ReturnValue.Value;
            }
        }
    }
}