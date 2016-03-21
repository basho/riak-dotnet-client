// <copyright file="FetchCounter.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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

namespace RiakClient.Commands.CRDT
{
    using Exceptions;
    using Messages;

    /// <summary>
    /// Fetches a Map from Riak
    /// </summary>
    public class FetchCounter : FetchCommand<CounterResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchCounter"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchCounterOptions"/></param>
        public FetchCounter(FetchCounterOptions options)
            : base(options)
        {
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = new CounterResponse();
            }
            else
            {
                DtFetchResp fetchResp = (DtFetchResp)response;
                if (fetchResp.type != DtFetchResp.DataType.COUNTER)
                {
                    throw new RiakException(
                        string.Format("Requested counter, received {0}", fetchResp.type));
                }

                if (fetchResp.value == null)
                {
                    Response = new CounterResponse();
                }
                else
                {
                    Response = new CounterResponse(Options.Key, fetchResp.context, fetchResp.value.counter_value);
                }
            }
        }

        /// <inheritdoc />
        public class Builder
            : FetchCommandBuilder<Builder, FetchCounter, FetchCounterOptions, CounterResponse>
        {
        }
    }
}