// <copyright file="FetchSet.cs" company="Basho Technologies, Inc.">
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
    using System.Collections.Generic;
    using Exceptions;
    using Messages;

    /// <summary>
    /// Fetches a Map from Riak
    /// </summary>
    public class FetchSet : FetchCommand<SetResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchSet"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchSetOptions"/></param>
        public FetchSet(FetchSetOptions options)
            : base(options)
        {
        }

        public override void OnSuccess(RiakResp response)
        {
            if (response == null)
            {
                Response = new SetResponse();
            }
            else
            {
                DtFetchResp fetchResp = (DtFetchResp)response;
                if (fetchResp.type != DtFetchResp.DataType.SET)
                {
                    throw new RiakException(
                        string.Format("Requested set, received {0}", fetchResp.type));
                }

                if (fetchResp.value == null)
                {
                    Response = new SetResponse();
                }
                else
                {
                    Response = new SetResponse(
                        Options.Key,
                        fetchResp.context,
                        new HashSet<byte[]>(fetchResp.value.set_value));
                }
            }
        }

        /// <inheritdoc />
        public class Builder
            : FetchCommandBuilder<Builder, FetchSet, FetchSetOptions, SetResponse>
        {
        }
    }
}
