// <copyright file="SearchDataTypes.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientExamples.Dev.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
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

            getIndexResult = client.GetSearchIndex("hobbies");
            if (!getIndexResult.IsSuccess)
            {
                var searchIndex = new SearchIndex("hobbies", "_yz_default");
                CheckResult(client.PutSearchIndex(searchIndex));
            }

            getIndexResult = client.GetSearchIndex("customers");
            if (!getIndexResult.IsSuccess)
            {
                var searchIndex = new SearchIndex("customers", "_yz_default");
                CheckResult(client.PutSearchIndex(searchIndex));
            }
        }

        [Test]
        public void SearchForCountersWithValueGreaterThan25()
        {
            var cmd = new UpdateCounter.Builder()
                .WithBucketType("counters")
                .WithBucket("people")
                .WithKey("christ_hitchens")
                .WithIncrement(10)
                .Build();
            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);

            cmd = new UpdateCounter.Builder()
                .WithBucketType("counters")
                .WithBucket("people")
                .WithKey("joan_rivers")
                .WithIncrement(25)
                .Build();
            rslt = client.Execute(cmd);
            CheckResult(rslt);

            WaitForSearch();

            DoSearch("scores", "counter:[20 TO *]");
        }

        [Test]
        public void SearchForSetsContainingFootballString()
        {
            var cmd = new UpdateSet.Builder()
                .WithBucketType("sets")
                .WithBucket("people")
                .WithKey("ditka")
                .WithAdditions(new HashSet<string> { "football", "winning" })
                .Build();
            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);

            cmd = new UpdateSet.Builder()
                .WithBucketType("sets")
                .WithBucket("people")
                .WithKey("dio")
                .WithAdditions(new HashSet<string> { "wailing", "rocking", "winning" })
                .Build();
            rslt = client.Execute(cmd);
            CheckResult(rslt);

            WaitForSearch();

            DoSearch("hobbies", "set:football");
        }

        [Test]
        public void Maps()
        {
            const string firstNameRegister = "first_name";
            const string lastNameRegister = "last_name";
            const string enterpriseCustomerFlag = "enterprise_customer";
            const string pageVisitsCounter = "page_visits";
            const string interestsSet = "interests";

            var idrisAdds = new[] { "acting", "being Stringer Bell" };

            var mapOp = new UpdateMap.MapOperation()
                .SetRegister(firstNameRegister, "Idris")
                .SetRegister(lastNameRegister, "Elba")
                .SetFlag(enterpriseCustomerFlag, false)
                .IncrementCounter(pageVisitsCounter, 10)
                .AddToSet(interestsSet, idrisAdds);

            var cmd = new UpdateMap.Builder()
                .WithBucketType("maps")
                .WithBucket("customers")
                .WithKey("idris_elba")
                .WithMapOperation(mapOp)
                .Build();

            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);

            var joanJettAdds = new[] { "loving rock and roll", "being in the Blackhearts" };

            mapOp = new UpdateMap.MapOperation()
                .SetRegister(firstNameRegister, "Joan")
                .SetRegister(lastNameRegister, "Jett")
                .SetFlag(enterpriseCustomerFlag, false)
                .IncrementCounter(pageVisitsCounter, 25)
                .AddToSet(interestsSet, joanJettAdds);

            cmd = new UpdateMap.Builder()
                .WithBucketType("maps")
                .WithBucket("customers")
                .WithKey("joan_jett")
                .WithMapOperation(mapOp)
                .Build();

            rslt = client.Execute(cmd);
            CheckResult(rslt);

            WaitForSearch();

            DoSearch("customers", "page_visits_counter:[15 TO *]");

            // Add "alter ego" sub-map
            const string nameRegister = "name";
            const string alterEgoMap = "alter_ego";

            mapOp = new UpdateMap.MapOperation();
            mapOp.Map(alterEgoMap).SetRegister(nameRegister, "John Luther");

            cmd = new UpdateMap.Builder()
                .WithBucketType("maps")
                .WithBucket("customers")
                .WithKey("idris_elba")
                .WithMapOperation(mapOp)
                .Build();

            rslt = client.Execute(cmd);
            CheckResult(rslt);

            PrintObject(cmd.Response.Value);

            WaitForSearch();

            DoSearch("customers", "alter_ego_map.name_register:*");
        }

        private void DoSearch(string index, string solrQuery)
        {
            var search = new RiakSearchRequest(index, solrQuery);
            var rslt = client.Search(search);
            CheckResult(rslt);

            RiakSearchResult searchResult = rslt.Value;
            Console.WriteLine("Num found: {0}", searchResult.NumFound);
            Assert.GreaterOrEqual(searchResult.Documents.Count, 1);

            Console.WriteLine("Search results for '{0}':", solrQuery);
            foreach (var doc in searchResult.Documents)
            {
                Console.WriteLine("\tKey: {0} Bucket Type: {1} Bucket: {2}",
                    doc.Key, doc.BucketType, doc.Bucket);
            }
        }
    }
}