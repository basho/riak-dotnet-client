// <copyright file="RiakStreamedIndexResult.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.Index
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Messages;

    /// <summary>
    /// Represents a result to a streaming index query.
    /// </summary>
    public class RiakStreamedIndexResult : IRiakIndexResult
    {
        private readonly IEnumerable<RiakResult<RpbIndexResp>> responseReader;
        private readonly bool includeTerms;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakStreamedIndexResult"/> class.
        /// </summary>
        /// <param name="includeTerms">The option to include the terms in the results.</param>
        /// <param name="responseReader">The <see cref="IEnumerable{T}"/> to read results from.</param>
        public RiakStreamedIndexResult(bool includeTerms, IEnumerable<RiakResult<RpbIndexResp>> responseReader)
        {
            this.responseReader = responseReader;
            this.includeTerms = includeTerms;
        }

        /// <inheritdoc/>
        public IEnumerable<RiakIndexKeyTerm> IndexKeyTerms
        {
            get
            {
                return responseReader.SelectMany(item => GetIndexKeyTerm(item.Value));
            }
        }

        private IEnumerable<RiakIndexKeyTerm> GetIndexKeyTerm(RpbIndexResp response)
        {
            IEnumerable<RiakIndexKeyTerm> indexKeyTerms = null;

            if (includeTerms)
            {
                indexKeyTerms = response.results.Select(
                    pair => new RiakIndexKeyTerm(pair.value.FromRiakString(), pair.key.FromRiakString()));
            }
            else
            {
                indexKeyTerms = response.keys.Select(key => new RiakIndexKeyTerm(key.FromRiakString()));
            }

            return indexKeyTerms;
        }
    }
}
