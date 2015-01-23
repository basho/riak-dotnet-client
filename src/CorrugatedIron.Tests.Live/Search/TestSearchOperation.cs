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

using System;
using System.Collections.Generic;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.Search
{
    [TestFixture]
    public class TestSearchOperation : LiveRiakConnectionTestBase
    {
        private const string BucketType = "search_type";
        private const string Bucket = "yoko_bucket";
        private const string Index = "yoko_index";
        private const string RiakSearchKey = "a.hacker";
        private const string RiakSearchKey2 = "a.public";
        private readonly Random _random = new Random();
        private int _randomId;
        private RiakObjectId _alyssaRiakId;

        // See https://raw.githubusercontent.com/basho/yokozuna/develop/priv/default_schema.xml for dynamic field suffix meanings.
        private const string RiakSearchDoc =
            "{{\"name_s\":\"{0}Alyssa P. Hacker\", \"age_i\":35, \"leader_b\":true, \"bio_tsd\":\"I'm an engineer, making awesome things.\", \"favorites\":{{\"book_tsd\":\"The Moon is a Harsh Mistress\",\"album_tsd\":\"Magical Mystery Tour\" }}}}";

        private const string RiakSearchDoc2 =
            "{{\"name_s\":\"{0}Alan Q. Public\", \"age_i\":38, \"bio_tsd\":\"I'm an exciting awesome mathematician\", \"favorites\":{{\"book_tsd\":\"Prelude to Mathematics\",\"album_tsd\":\"The Fame Monster\"}}}}";

        [TestFixtureSetUp]
        public void Init()
        {
            SetUp();

            var index = new SearchIndex(Index);
            Client.PutSearchIndex(index);

            Func<RiakResult<RiakBucketProperties>, bool> indexIsSet =
                result => result.IsSuccess &&
                          result.Value != null &&
                          !string.IsNullOrEmpty(result.Value.SearchIndex);

            Func<RiakResult<RiakBucketProperties>> setBucketProperties =
                () =>
                {
                    RiakBucketProperties props = Client.GetBucketProperties(BucketType, Bucket).Value;
                    props.SetSearchIndex(Index);
                    Client.SetBucketProperties(BucketType, Bucket, props);
                    return Client.GetBucketProperties(BucketType, Bucket);
                };

            setBucketProperties.WaitUntil(indexIsSet);
            System.Threading.Thread.Sleep(5000); // Wait for Yoko to start up
            PrepSearch();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Client.DeleteBucket(BucketType, Bucket);
        }

        private void PrepSearch()
        {
            _randomId = _random.Next();
            var alyssaKey = _randomId + RiakSearchKey;
            _alyssaRiakId = new RiakObjectId(BucketType, Bucket, alyssaKey);
            var alyssaDoc = String.Format(RiakSearchDoc, _randomId);

            var alanKey = _randomId + RiakSearchKey2;
            var alanRiakId = new RiakObjectId(BucketType, Bucket, alanKey);
            var alanDoc = String.Format(RiakSearchDoc2, _randomId);

            Console.WriteLine("Using {0}, {1} for Yokozuna search keys", alyssaKey, alanKey);

            Func<RiakResult<RiakObject>> put1 = () => Client.Put(new RiakObject(_alyssaRiakId, alyssaDoc.ToRiakString(),
                RiakConstants.ContentTypes.ApplicationJson, RiakConstants.CharSets.Utf8));

            var put1Result = put1.WaitUntil();

            Func<RiakResult<RiakObject>> put2 = () => Client.Put(new RiakObject(alanRiakId, alanDoc.ToRiakString(),
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
                Query = new RiakFluentSearch(Index, "name_s")
                    .Search(_randomId + "Alyssa P. Hacker")
                    .Build()
            };

            var searchResult = Client.RunSolrQuery(req).WaitUntil(SearchTestHelpers.AnyMatchIsFound);

            searchResult.IsSuccess.ShouldBeTrue(searchResult.ErrorMessage);
            searchResult.Value.NumFound.ShouldEqual(1u);
            searchResult.Value.Documents.Count.ShouldEqual(1);

            // [ _yz_rt, _yz_rb, _yz_rk, score, _yz_id ]
            // [ name_s, age_i, leader_b, bio_tsd, favorites.book_tsd , favorites.album_tsd ]
            searchResult.Value.Documents[0].Fields.Count.ShouldEqual(11);
            searchResult.Value.Documents[0].RiakObjectId.ShouldEqual(_alyssaRiakId);
        }

        [Test]
        public void SearchingWithWildcardFluentQueryWorksCorrectly()
        {
            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch(Index, "name_s").Search(Token.StartsWith(_randomId + "Al")).Build()
            };

            var searchResult = Client.RunSolrQuery(req).WaitUntil(SearchTestHelpers.TwoMatchesFound);
            searchResult.IsSuccess.ShouldBeTrue(searchResult.ErrorMessage);
            searchResult.Value.NumFound.ShouldEqual(2u);
            searchResult.Value.Documents.Count.ShouldEqual(2);
        }

        [Test]
        public void SearchingWithMoreComplexFluentQueryWorksCorrectly()
        {
            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch(Index, "name_s")
            };

            // name_s:{integer}Al* AND bio_tsd:awesome AND bio_tsd:an AND (bio_tsd:mathematician OR favorites.album_tsd:Fame)
            req.Query.Search(Token.StartsWith(_randomId + "Al"))
                .And("bio_tsd", "awesome")
                .And("an")
                .And("mathematician", s => s.Or("favorites.album_tsd", "Fame"));

            Console.WriteLine(req.Query.ToString());

            var result = Client.RunSolrQuery(req).WaitUntil(SearchTestHelpers.AnyMatchIsFound);

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.NumFound.ShouldEqual(1u);
            result.Value.Documents.Count.ShouldEqual(1);

            // [ _yz_rt, _yz_rb, _yz_rk, score, _yz_id ]
            // [ name_s, age_i, leader_b, bio_tsd, favorites.book_tsd , favorites.album_tsd ]
            result.Value.Documents[0].Fields.Count.ShouldEqual(10);
            var id = result.Value.Documents[0].Id;
            id.Contains("a.public").ShouldBeTrue(string.Format("{0} does not contain {1}", id, "a.public"));
        }

        [Test]
        public void SettingFieldListReturnsOnlyFieldsSpecified()
        {
            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch(Index, "name_s"),
                PreSort = PreSort.Key,
                DefaultOperation = DefaultOperation.Or,
                ReturnFields = new List<string>
                {
                    RiakConstants.SearchFieldKeys.Id,
                    "bio_tsd",
                    "favorites.album_tsd"
                }
            };


            // name_s:{integer}Al* AND bio_tsd:awesome AND bio_tsd:an AND (bio_tsd:mathematician OR favorites.album_tsd:Fame)
            req.Query.Search(Token.StartsWith(_randomId + "Al"))
                .And("bio_tsd", "awesome")
                .And("an")
                .And("mathematician", s => s.Or("favorites.album_tsd", "Fame"));


            Console.WriteLine(req.Query.ToString());

            var result = Client.RunSolrQuery(req).WaitUntil(SearchTestHelpers.AnyMatchIsFound);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.NumFound.ShouldEqual(1u);
            result.Value.Documents.Count.ShouldEqual(1);
            result.Value.Documents[0].Fields.Count.ShouldEqual(3);
            result.Value.Documents[0].Id.ShouldNotBeNull();
        }
    }
}