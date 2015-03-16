// <copyright file="SearchDataTypes.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2015 - Basho Technologies, Inc.
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

namespace RiakClientExamples.Dev.Search
{
    using System;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;
    using RiakClient.Models.Search;

    /*
     * http://docs.basho.com/riak/latest/dev/search/search-data-types/
     */
    public sealed class SearchDataTypes : ExampleBase
    {
        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            base.CreateClient();

            var getIndexResult = client.GetSearchIndex("scores");
            if (!getIndexResult.IsSuccess)
            {
                var searchIndex = new SearchIndex("scores", "_yz_default");
                CheckResult(client.PutSearchIndex(searchIndex));
            }
        }

        [Test]
        public void StoreSomeCounters()
        {
            var christopherHitchensId = new RiakObjectId("counters", "people", "christ_hitchens");
            var hitchensRslt = client.DtUpdateCounter(christopherHitchensId, 10);
            CheckResult(hitchensRslt.Result);

            var joanRiversId = new RiakObjectId("counters", "people", "joan_rivers");
            var joanRiversRslt = client.DtUpdateCounter(joanRiversId, 25);
            CheckResult(joanRiversRslt.Result);
        }

        [Test]
        public void SearchForCountersWithValueGreaterThan25()
        {
            StoreSomeCounters();
            WaitForSearch();

            var search = new RiakSearchRequest("scores", "counter:[20 TO *]");
            var rslt = client.Search(search);
            CheckResult(rslt);

            RiakSearchResult searchResult = rslt.Value;
            Console.WriteLine("Num found: {0}", searchResult.NumFound);
            Assert.GreaterOrEqual(1, searchResult.Documents.Count);
        }
    }
}
