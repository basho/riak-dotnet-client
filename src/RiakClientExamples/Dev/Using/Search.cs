// <copyright file="Search.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientExamples.Dev.Using
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;
    using RiakClient.Models.Search;

    /*
     * http://docs.basho.com/riak/latest/dev/using/search/
     */
    public sealed class Search : ExampleBase
    {
        [Test]
        public void CreateSearchIndex()
        {
            var idx = new SearchIndex("famous");
            var rslt = client.PutSearchIndex(idx);
            CheckResult(rslt);
        }

        [Test]
        public void CreateSearchIndexWithDefaultSchema()
        {
            var idx = new SearchIndex("famous", "_yz_default");
            var rslt = client.PutSearchIndex(idx);
            CheckResult(rslt);
        }

        [Test]
        public void AssociateSearchIndexWithBucketViaProperties()
        {
            var properties = new RiakBucketProperties();
            properties.SetSearchIndex("famous");
            var rslt = client.SetBucketProperties("cats", properties);
            CheckResult(rslt);
        }

        [Test]
        public void IndexingValues()
        {
            ids = PutAnimals();
        }

        [Test]
        public void SimpleQuery()
        {
            ids = PutAnimals();

            var search = new RiakSearchRequest("famous", "name_s:Lion*");
            var rslt = client.Search(search);
            CheckResult(rslt);

            RiakSearchResult searchResult = rslt.Value;

            foreach (RiakSearchResultDocument doc in searchResult.Documents)
            {
                var args = new[] {
                    doc.BucketType,
                    doc.Bucket,
                    doc.Key,
                    string.Join(", ", doc.Fields.Select(f => f.Value).ToArray())
                };
                Console.WriteLine(
                    "BucketType: {0} Bucket: {1} Key: {2} Values: {3}",
                    args);
            }
        }

        [Test]
        public void SimpleQueryWithGet()
        {
            ids = PutAnimals();

            var search = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("famous", "name_s")
                    .Search("Lion*")
                    .Build()
            };

            var searchRslt = client.Search(search);
            CheckResult(searchRslt);

            RiakSearchResult searchResult = searchRslt.Value;

            RiakSearchResultDocument doc = searchResult.Documents.First();
            var id = new RiakObjectId(doc.BucketType, doc.Bucket, doc.Key);
            var rslt = client.Get(id);
            CheckResult(rslt);

            RiakObject obj = rslt.Value;
            Console.WriteLine(Encoding.UTF8.GetString(obj.Value));
        }

        [Test]
        public void RangeQuery()
        {
            ids = PutAnimals();

            var search = new RiakSearchRequest("famous", "age_i:[30 TO *]");

            /*
             * Fluent interface:
             * 
            var search = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("famous", "age_i")
                    .Between("30", "*")
                    .Build()
            };
             */

            var rslt = client.Search(search);
            CheckResult(rslt);

            RiakSearchResult searchResult = rslt.Value;

            foreach (RiakSearchResultDocument doc in searchResult.Documents)
            {
                var args = new[] {
                    doc.BucketType,
                    doc.Bucket,
                    doc.Key,
                    string.Join(", ", doc.Fields.Select(f => f.Value).ToArray())
                };
                Console.WriteLine("BucketType: {0} Bucket: {1} Key: {2} Values: {3}", args);
            }
        }

        [Test]
        public void QueryWithOperations()
        {
            var search = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("famous", "leader_b")
                    .Search("true").AndBetween("age_i", "30", "*")
                    .Build()
            };

            var rslt = client.Search(search);
            CheckResult(rslt);
        }

        [Test]
        public void QueryWithPagination()
        {
            ids = PutAnimals();

            int rowsPerPage = 2;
            int page = 2;
            int start = rowsPerPage * (page - 1);

            var search = new RiakSearchRequest
            {
                Start = start,
                Rows = rowsPerPage,
                Query = new RiakFluentSearch("famous", "*")
                    .Search("*")
                    .Build(),
            };

            var rslt = client.Search(search);
            CheckResult(rslt);
            Assert.AreEqual(rowsPerPage, rslt.Value.Documents.Count);
        }

        [Test]
        public void DeleteIndex()
        {
            var rslt = client.DeleteSearchIndex("famous");
            CheckResult(rslt, true);
        }

        private ICollection<RiakObjectId> PutAnimals()
        {
            var properties = new RiakBucketProperties();
            properties.SetSearchIndex("famous");
            var bucketPropsResult = client.SetBucketProperties("animals", "cats", properties);
            CheckResult(bucketPropsResult);

            // Note: objects will be deleted on teardown unless CLEANUP is *undefined*
            var ids = new List<RiakObjectId>();

            var lionoId = new RiakObjectId("animals", "cats", "liono");
            ids.Add(lionoId);
            var lionoObj = new { name_s = "Lion-o", age_i = 30, leader_b = true };
            var lionoRiakObj = new RiakObject(lionoId, lionoObj);

            var cheetaraId = new RiakObjectId("animals", "cats", "cheetara");
            ids.Add(cheetaraId);
            var cheetaraObj = new { name_s = "Cheetara", age_i = 30, leader_b = false };
            var cheetaraRiakObj = new RiakObject(cheetaraId, cheetaraObj);

            var snarfId = new RiakObjectId("animals", "cats", "snarf");
            ids.Add(snarfId);
            var snarfObj = new { name_s = "Snarf", age_i = 43, leader_b = false };
            var snarfRiakObj = new RiakObject(snarfId, snarfObj);

            var panthroId = new RiakObjectId("animals", "cats", "panthro");
            ids.Add(panthroId);
            var panthroObj = new { name_s = "Panthro", age_i = 36, leader_b = false };
            var panthroRiakObj = new RiakObject(panthroId, panthroObj);

            var rslts = client.Put(new[] { lionoRiakObj, cheetaraRiakObj, snarfRiakObj, panthroRiakObj });
            foreach (var rslt in rslts)
            {
                CheckResult(rslt);
            }

            Thread.Sleep(2000);

            return ids;
        }
    }
}
