// <copyright file="Search.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientExamples.Dev.Using
{
    using System.Collections.Generic;
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
            rslt = client.PutSearchIndex(idx);
        }

        [Test]
        public void CreateSearchIndexWithDefaultSchema()
        {
            var idx = new SearchIndex("famous", "_yz_default");
            rslt = client.PutSearchIndex(idx);
        }

        [Test]
        public void AssociateSearchIndexWithBucketViaProperties()
        {
            var properties = new RiakBucketProperties();
            properties.SetSearchIndex("famous");
            rslt = client.SetBucketProperties("cats", properties);
        }

        [Test]
        public void IndexingValues()
        {
            var properties = new RiakBucketProperties();
            properties.SetSearchIndex("famous");
            var bucketPropsResult = client.SetBucketProperties("animals", "cats", properties);
            CheckResult(bucketPropsResult);

            // Note: objects will be deleted on teardown unless CLEANUP is *undefined*
            ids = new List<RiakObjectId>();

            var lionoId = new RiakObjectId("animals", "cats", "liono");
            ids.Add(lionoId);
            string lionoJson = "{\"name_s\":\"Lion-o\",\"age\":30,\"leader\":true}";
            var liono = new RiakObject(lionoId, lionoJson);

            var cheetaraId = new RiakObjectId("animals", "cats", "cheetara");
            ids.Add(cheetaraId);
            string cheetaraJson = "{\"name_s\":\"Cheetara\",\"age\":30,\"leader\":false}";
            var cheetara = new RiakObject(cheetaraId, cheetaraJson);

            var snarfId = new RiakObjectId("animals", "cats", "snarf");
            ids.Add(snarfId);
            string snarfJson = "{\"name_s\":\"Snarf\",\"age\":43}";
            var snarf = new RiakObject(snarfId, snarfJson);

            var panthroId = new RiakObjectId("animals", "cats", "panthro");
            ids.Add(panthroId);
            string panthroJson = "{\"name_s\":\"Panthro\",\"age_i\":36}";
            var panthro = new RiakObject(panthroId, panthroJson);

            var rslts = client.Put(new[] { liono, cheetara, snarf, panthro });
            foreach (var rslt in rslts)
            {
                CheckResult(rslt);
            }
        }
    }
}
