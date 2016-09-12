// <copyright file="FetchHll.cs" company="Basho Technologies, Inc.">
// Copyright 2016 - Basho Technologies, Inc.
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
    public class FetchHll : FetchCommand<HllResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchHll"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchHllOptions"/></param>
        public FetchHll(FetchHllOptions options)
            : base(options)
        {
        }

        public override void OnSuccess(RiakResp response)
        {
            if (response == null)
            {
                Response = new HllResponse();
            }
            else
            {
                DtFetchResp fetchResp = (DtFetchResp)response;
                if (fetchResp.type != DtFetchResp.DataType.HLL)
                {
                    throw new RiakException(
                        string.Format("Requested hyperloglog, received {0}", fetchResp.type));
                }

                if (fetchResp.value == null)
                {
                    Response = new HllResponse();
                }
                else
                {
                    Response = new HllResponse(Options.Key, fetchResp.value.hll_value);
                }
            }
        }

        /// <inheritdoc />
        public class Builder
            : FetchCommandBuilder<Builder, FetchHll, FetchHllOptions, HllResponse>
        {
        }
    }
}