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

using System;
using CorrugatedIron.Models.Search;

namespace CorrugatedIron.Tests.Live.Extensions
{
    public static class SearchTestHelpers
    {
        public static Func<RiakResult<RiakSearchResult>, bool> AnyMatchIsFound
        {
            get
            {
                Func<RiakResult<RiakSearchResult>, bool> matchIsFound =
                    result => result.IsSuccess &&
                              result.Value != null &&
                              result.Value.NumFound > 0;
                return matchIsFound;
            }
        }

        public static Func<RiakResult<RiakSearchResult>, bool> TwoMatchesFound
        {
            get
            {
                Func<RiakResult<RiakSearchResult>, bool> twoMatchesFound =
                    result => result.IsSuccess &&
                              result.Value != null &&
                              result.Value.NumFound == 2;
                return twoMatchesFound;
            }
        }

        public static Func<RiakResult<RiakSearchResult>> RunSolrQuery(this IRiakClient client, RiakSearchRequest req)
        {
            Func<RiakResult<RiakSearchResult>> runSolrQuery =
                () => client.Search(req);
            return runSolrQuery;
        }
    }
}