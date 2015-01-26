// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2015 - Basho Technologies, Inc.
// 
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using System.Collections.Generic;
using RiakClient.Extensions;
using NUnit.Framework;
using RiakClient.Models;
using RiakClient.Models.Search;
using RiakClient.Tests.Live.Extensions;
using RiakClient.Util;

namespace RiakClient.Tests.Live.Deprecated
{
    [TestFixture]
    public class WhenQueryingRiakLegacySearchViaPbc : LiveRiakConnectionTestBase
    {
        private const string Bucket = "riak_search_bucket";
        private const string RiakSearchKey = "a.hacker";
        private const string RiakSearchKey2 = "a.public";
        private const string RiakSearchDoc = "{\"name\":\"Alyssa P. Hacker\", \"bio\":\"I'm an engineer, making awesome things.\", \"favorites\":{\"book\":\"The Moon is a Harsh Mistress\",\"album\":\"Magical Mystery Tour\", }}";
        private const string RiakSearchDoc2 = "{\"name\":\"Alan Q. Public\", \"bio\":\"I'm an exciting awesome mathematician\", \"favorites\":{\"book\":\"Prelude to Mathematics\",\"album\":\"The Fame Monster\"}}";

        [TestFixtureSetUp]
        public override void SetUp()
        {
            base.SetUp();

            var props = Client.GetBucketProperties(Bucket).Value;
            props.SetLegacySearch(true);
            Client.SetBucketProperties(Bucket, props);

            PrepSearchData();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Client.DeleteBucket(Bucket);
        }

        private void PrepSearchData()
        {
            Func<RiakResult<RiakObject>> put1 = () => Client.Put(new RiakObject(Bucket, RiakSearchKey, RiakSearchDoc.ToRiakString(),
                                                      RiakConstants.ContentTypes.ApplicationJson, RiakConstants.CharSets.Utf8));

            var put1Result = put1.WaitUntil();

            Func<RiakResult<RiakObject>> put2 = () => Client.Put(new RiakObject(Bucket, RiakSearchKey2, RiakSearchDoc2.ToRiakString(),
                                                      RiakConstants.ContentTypes.ApplicationJson, RiakConstants.CharSets.Utf8));

            var put2Result = put2.WaitUntil();

            put1Result.IsSuccess.ShouldBeTrue(put1Result.ErrorMessage);
            put2Result.IsSuccess.ShouldBeTrue(put2Result.ErrorMessage);
        }


        [Test]
        public void SearchingWithSimpleFluentQueryWorksCorrectly()
        {
            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("riak_search_bucket", "name").Search("Alyssa").Build()
            };

            var result = Client.RunSolrQuery(req).WaitUntil(SearchTestHelpers.AnyMatchIsFound);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.NumFound.ShouldEqual(1u);
            result.Value.Documents.Count.ShouldEqual(1);
            result.Value.Documents[0].Fields.Count.ShouldEqual(5);
            result.Value.Documents[0].Id.ShouldEqual("a.hacker");
        }

        [Test]
        public void SearchingWithWildcardFluentQueryWorksCorrectly()
        {
            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("riak_search_bucket", "name").Search(Token.StartsWith("Al")).Build()
            };


            var result = Client.RunSolrQuery(req).WaitUntil(SearchTestHelpers.TwoMatchesFound);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.NumFound.ShouldEqual(2u);
            result.Value.Documents.Count.ShouldEqual(2);
        }

        [Test]
        public void SearchingWithMoreComplexFluentQueryWorksCorrectly()
        {
            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("riak_search_bucket", "bio")
            };

            req.Query.Search("awesome")
                .And("an")
                .And("mathematician", t => t.Or("favorites_ablum", "Fame"));

            var result = Client.RunSolrQuery(req).WaitUntil(SearchTestHelpers.AnyMatchIsFound);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.NumFound.ShouldEqual(1u);
            result.Value.Documents.Count.ShouldEqual(1);
            result.Value.Documents[0].Fields.Count.ShouldEqual(5);
            result.Value.Documents[0].Id.ShouldEqual("a.public");
        }

        [Test]
        public void SettingFieldListReturnsOnlyFieldsSpecified()
        {
            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("riak_search_bucket", "bio"),
                PreSort = PreSort.Key,
                DefaultOperation = DefaultOperation.Or,
                ReturnFields = new List<string>
                {
                    "bio", "favorites_album"
                }
            };

            req.Query.Search("awesome")
                .And("an")
                .And("mathematician", t => t.Or("favorites_ablum", "Fame"));

            var result = Client.RunSolrQuery(req).WaitUntil(SearchTestHelpers.AnyMatchIsFound);

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.NumFound.ShouldEqual(1u);
            result.Value.Documents.Count.ShouldEqual(1);
            // "id" field is always returned
            result.Value.Documents[0].Fields.Count.ShouldEqual(3);
            result.Value.Documents[0].Id.ShouldNotBeNull();
        }


    }
}
