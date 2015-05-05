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

#pragma warning disable 618

namespace RiakClientExamples.Dev.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Messages;
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
            var christopherHitchensId = new RiakObjectId("counters", "people", "christ_hitchens");
            var hitchensRslt = client.DtUpdateCounter(christopherHitchensId, 10);
            CheckResult(hitchensRslt.Result);

            var joanRiversId = new RiakObjectId("counters", "people", "joan_rivers");
            var joanRiversRslt = client.DtUpdateCounter(joanRiversId, 25);
            CheckResult(joanRiversRslt.Result);

            WaitForSearch();

            DoSearch("scores", "counter:[20 TO *]");
        }

        [Test]
        public void SearchForSetsContainingFootballString()
        {
            var mikeDitkaId = new RiakObjectId("sets", "people", "ditka");
            var ditkaAdds = new List<string> { "football", "winning" };
            var ditkaRslt = client.DtUpdateSet(mikeDitkaId, Serializer, null, ditkaAdds);
            CheckResult(ditkaRslt.Result);

            var dioId = new RiakObjectId("sets", "people", "dio");
            var dioAdds = new List<string> { "wailing", "rocking", "winning" };
            var dioRslt = client.DtUpdateSet(dioId, Serializer, null, dioAdds);
            CheckResult(dioRslt.Result);

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

            var idrisElbaId = new RiakObjectId("maps", "customers", "idris_elba");
            var idrisMapUpdates = new List<MapUpdate>();
            idrisMapUpdates.Add(new MapUpdate
            {
                register_op = Serializer("Idris"),
                field = new MapField
                {
                    name = Serializer(firstNameRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            idrisMapUpdates.Add(new MapUpdate
            {
                register_op = Serializer("Elba"),
                field = new MapField
                {
                    name = Serializer(lastNameRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            idrisMapUpdates.Add(new MapUpdate
            {
                flag_op = MapUpdate.FlagOp.DISABLE,
                field = new MapField
                {
                    name = Serializer(enterpriseCustomerFlag),
                    type = MapField.MapFieldType.FLAG
                }
            });

            idrisMapUpdates.Add(new MapUpdate
            {
                counter_op = new CounterOp { increment = 10 },
                field = new MapField
                {
                    name = Serializer(pageVisitsCounter),
                    type = MapField.MapFieldType.COUNTER
                }
            });

            var idrisAdds = new[] { "acting", "being Stringer Bell" };
            var idrisSetOp = new SetOp();
            idrisSetOp.adds.AddRange(idrisAdds.Select(x => Serializer(x)));
            idrisMapUpdates.Add(new MapUpdate
            {
                set_op = idrisSetOp,
                field = new MapField
                {
                    name = Serializer(interestsSet),
                    type = MapField.MapFieldType.SET
                }
            });

            var idrisRslt = client.DtUpdateMap(idrisElbaId, Serializer, null, null, idrisMapUpdates);
            CheckResult(idrisRslt.Result);

            var joanJettId = new RiakObjectId("maps", "customers", "joan_jett");
            var joanJettMapUpdates = new List<MapUpdate>();
            joanJettMapUpdates.Add(new MapUpdate
            {
                register_op = Serializer("Joan"),
                field = new MapField
                {
                    name = Serializer(firstNameRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            joanJettMapUpdates.Add(new MapUpdate
            {
                register_op = Serializer("Jett"),
                field = new MapField
                {
                    name = Serializer(lastNameRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            joanJettMapUpdates.Add(new MapUpdate
            {
                flag_op = MapUpdate.FlagOp.DISABLE,
                field = new MapField
                {
                    name = Serializer(enterpriseCustomerFlag),
                    type = MapField.MapFieldType.FLAG
                }
            });

            joanJettMapUpdates.Add(new MapUpdate
            {
                counter_op = new CounterOp { increment = 25 },
                field = new MapField
                {
                    name = Serializer(pageVisitsCounter),
                    type = MapField.MapFieldType.COUNTER
                }
            });

            var joanJettAdds = new[] { "loving rock and roll", "being in the Blackhearts" };
            var joanJettSetOp = new SetOp();
            joanJettSetOp.adds.AddRange(joanJettAdds.Select(x => Serializer(x)));
            joanJettMapUpdates.Add(new MapUpdate
            {
                set_op = joanJettSetOp,
                field = new MapField
                {
                    name = Serializer(interestsSet),
                    type = MapField.MapFieldType.SET
                }
            });

            var joanJettRslt = client.DtUpdateMap(joanJettId, Serializer, null, null, joanJettMapUpdates);
            CheckResult(joanJettRslt.Result);

            WaitForSearch();

            DoSearch("customers", "page_visits_counter:[15 TO *]");

            // Add "alter ego" sub-map
            const string nameRegister = "name";
            const string alterEgoMap = "alter_ego";

            idrisElbaId = new RiakObjectId("maps", "customers", "idris_elba");
            var idrisGetRslt = client.DtFetchMap(idrisElbaId);
            CheckResult(idrisGetRslt.Result);

            var alterEgoMapOp = new MapOp();
            alterEgoMapOp.updates.Add(new MapUpdate
            {
                register_op = Serializer("John Luther"),
                field = new MapField
                {
                    name = Serializer(nameRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            var alterEgoMapUpdate = new MapUpdate
            {
                map_op = alterEgoMapOp,
                field = new MapField
                {
                    name = Serializer(alterEgoMap),
                    type = MapField.MapFieldType.MAP
                }
            };

            var idrisUpdateRslt = client.DtUpdateMap(idrisElbaId, Serializer,
                idrisGetRslt.Context, null, new List<MapUpdate> { alterEgoMapUpdate });
            CheckResult(idrisUpdateRslt.Result);
            PrintMapValues(idrisUpdateRslt.Values);

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

#pragma warning restore 618