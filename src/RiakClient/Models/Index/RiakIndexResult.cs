// <copyright file="RiakIndexResult.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.Index
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Messages;

    /// <summary>
    /// Represents a result to an index query.
    /// </summary>
    public class RiakIndexResult : IRiakIndexResult
    {
        private readonly IEnumerable<RiakIndexKeyTerm> indexKeyTerms;

        internal RiakIndexResult(bool includeTerms, RiakResult<RpbIndexResp> response)
        {
            if (includeTerms)
            {
                indexKeyTerms = response.Value.results.Select(
                    pair => new RiakIndexKeyTerm(pair.value.FromRiakString(), pair.key.FromRiakString()));
            }
            else
            {
                indexKeyTerms = response.Value.keys.Select(key => new RiakIndexKeyTerm(key.FromRiakString()));
            }
        }

        /// <inheritdoc/>
        public IEnumerable<RiakIndexKeyTerm> IndexKeyTerms
        {
            get { return indexKeyTerms; }
        }
    }
}
