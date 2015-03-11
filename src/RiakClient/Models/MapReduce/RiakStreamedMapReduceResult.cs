// <copyright file="RiakStreamedMapReduceResult.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.MapReduce
{
    using System.Collections.Generic;
    using System.Linq;
    using Messages;

    /// <summary>
    /// Represents a Riak streaming mapreduce result.
    /// </summary>
    public class RiakStreamedMapReduceResult : IRiakMapReduceResult
    {
        private readonly IEnumerable<RiakResult<RpbMapRedResp>> responseReader;

        internal RiakStreamedMapReduceResult(IEnumerable<RiakResult<RpbMapRedResp>> responseReader)
        {
            this.responseReader = responseReader;
        }

        /// <inheritdoc/>
        public IEnumerable<RiakMapReduceResultPhase> PhaseResults
        {
            get
            {
                return responseReader.Select(item => item.IsSuccess
                    ? new RiakMapReduceResultPhase(
                            item.Value.phase,
                            new List<RpbMapRedResp>
                            {
                                item.Value
                            })
                    : new RiakMapReduceResultPhase());
            }
        }
    }
}
