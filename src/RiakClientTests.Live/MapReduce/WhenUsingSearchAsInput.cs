// <copyright file="WhenUsingSearchAsInput.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.Live.MapReduce
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Extensions;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Extensions;
    using RiakClient.Models;
    using RiakClient.Models.MapReduce;
    using RiakClient.Models.MapReduce.Inputs;
    using RiakClient.Models.Search;

    [TestFixture, IntegrationTest, SkipMono]
    public class WhenUsingSearchAsInput : RiakMapReduceTestBase
    {
        private const string BucketType = "search_type";
        private new const string Bucket = "yoko_bucket";
        private const string Index = "yoko_index";
        private const string HackerKey = "a.hacker";
        private const string PublicKey = "a.public";
        // See https://raw.githubusercontent.com/basho/yokozuna/develop/priv/default_schema.xml for dynamic field suffix meanings.
        private const string HackerDoc =
            "{{\"name_s\":\"{0}Alyssa P. Hacker\", \"age_i\":35, \"leader_b\":true, \"bio_tsd\":\"I'm an engineer, making awesome things.\", \"favorites\":{{\"book_tsd\":\"The Moon is a Harsh Mistress\",\"album_tsd\":\"Magical Mystery Tour\" }}}}";

        private const string PublicDoc =
            "{{\"name_s\":\"{0}Alan Q. Public\", \"age_i\":38, \"bio_tsd\":\"I'm an exciting awesome mathematician\", \"favorites\":{{\"book_tsd\":\"Prelude to Mathematics\",\"album_tsd\":\"The Fame Monster\"}}}}";

        private readonly Random _random = new Random();
        private RiakObjectId _alyssaRiakId;
        private int _randomId;

        [TestFixtureSetUp]
        public override void SetUp()
        {
            base.SetUp();

            SetupSearchIndexes();
            LoadDataIntoRiak();
        }

        private void SetupSearchIndexes()
        {
            var index = new SearchIndex(Index);
            index.Timeout = TimeSpan.FromSeconds(60);
            RiakResult rrslt = Client.PutSearchIndex(index);
            Assert.True(rrslt.IsSuccess, rrslt.ErrorMessage);

            RiakBucketProperties props = null;

            Func<RiakResult<RiakBucketProperties>, bool> propsExist =
                result => result != null && result.Value != null && props != null;

            Func<RiakResult<RiakBucketProperties>, bool> indexIsSet =
                result => result != null &&
                          result.IsSuccess &&
                          result.Value != null &&
                          !string.IsNullOrEmpty(result.Value.SearchIndex);

            Func<RiakResult<RiakBucketProperties>> getBucketProperties = () =>
                {
                    RiakResult<RiakBucketProperties> rslt = Client.GetBucketProperties(BucketType, Bucket);
                    if (rslt.Value != null)
                    {
                        props = rslt.Value;
                    }
                    return rslt;
                };

            getBucketProperties.WaitUntil(propsExist);

            props.SetSearchIndex(Index);
            rrslt = Client.SetBucketProperties(BucketType, Bucket, props);
            Assert.True(rrslt.IsSuccess, rrslt.ErrorMessage);

            getBucketProperties.WaitUntil(indexIsSet);
            Thread.Sleep(5000); // Wait for Yoko to start up
        }

        private void LoadDataIntoRiak()
        {
            _randomId = _random.Next();
            var alyssaKey = _randomId + HackerKey;
            _alyssaRiakId = new RiakObjectId(BucketType, Bucket, alyssaKey);
            var alyssaDoc = string.Format(HackerDoc, _randomId);

            var alanKey = _randomId + PublicKey;
            var alanRiakId = new RiakObjectId(BucketType, Bucket, alanKey);
            var alanDoc = string.Format(PublicDoc, _randomId);

            Console.WriteLine("Using {0}, {1} for Yokozuna/MapReduce search keys", alyssaKey, alanKey);

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
        public void SearchingByNameReturnsTheObjectId()
        {
            var mr = new RiakMapReduceQuery()
                .Inputs(new RiakSearchInput(Index, "name_s:" + _randomId + "Al*"));


            var result = Client.RunMapReduceQuery(mr).WaitUntil(MapReduceTestHelpers.OnePhaseWithTwoResultsFound);

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var phaseResults = result.Value.PhaseResults.ToList();
            phaseResults.Count.ShouldEqual(1);

            AssertThatResultContainsAllKeys(result);
        }

        [Test]
        public void SearchingViaOldInterfaceFluentSearchObjectWorks()
        {
            var search = new RiakFluentSearch(Index, "name_s").Search(Token.StartsWith(_randomId + "Al")).Build();

#pragma warning disable 618
            var mr = new RiakMapReduceQuery().Inputs(new RiakBucketSearchInput(search));
#pragma warning restore 618

            var result = Client.RunMapReduceQuery(mr).WaitUntil(MapReduceTestHelpers.OnePhaseWithTwoResultsFound);

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            AssertThatResultContainsAllKeys(result);
        }

        [Test]
        public void SearchingViaFluentSearchObjectWorks()
        {
            var search = new RiakFluentSearch(Index, "name_s").Search(Token.StartsWith(_randomId + "Al")).Build();
            var mr = new RiakMapReduceQuery().Inputs(new RiakSearchInput(search));

            var result = Client.RunMapReduceQuery(mr).WaitUntil(MapReduceTestHelpers.OnePhaseWithTwoResultsFound);

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            AssertThatResultContainsAllKeys(result);
        }

        [Test]
        public void SearchingComplexQueryWorks()
        {
            var search = new RiakFluentSearch(Index, "name_s");

            // name_s:{integer}Al* AND bio_tsd:awesome AND bio_tsd:an AND (bio_tsd:mathematician OR favorites.album_tsd:Fame)
            search.Search(Token.StartsWith(_randomId + "Al"))
                .And("bio_tsd", "awesome")
                .And("an")
                .And("mathematician", s => s.Or("favorites.album_tsd", "Fame"));

            var mr = new RiakMapReduceQuery().Inputs(new RiakSearchInput(search));

            var result = Client.RunMapReduceQuery(mr).WaitUntil(MapReduceTestHelpers.OnePhaseWithOneResultFound);

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            var singleResult = result.Value.PhaseResults.First().Values[0].FromRiakString();
            var failureMessage = string.Format("Results did not contain \"{0}\". \r\nResult was:\"{1}\"",
                PublicKey, singleResult);
            singleResult.Contains(PublicKey).ShouldBeTrue(failureMessage);
        }


        private static void AssertThatResultContainsAllKeys(RiakResult<RiakMapReduceResult> mapReduceResult)
        {
            var phaseResults = mapReduceResult.Value.PhaseResults.ToList();
            phaseResults.Count.ShouldEqual(1);

            var searchResults = phaseResults[0];
            searchResults.Values.ShouldNotBeNull();
            searchResults.Values.Count.ShouldEqual(2);

            var allKeys = new List<string> { HackerKey, PublicKey };
            var solrResults = searchResults.Values.Select(searchResult => searchResult.FromRiakString());

            var usedKeys = solrResults.SelectMany(result => allKeys.Where(result.Contains));
            var unusedKeys = allKeys.Except(usedKeys).ToList();

            Assert.AreEqual(0, unusedKeys.Count, "Results did not contain the following keys: {0}",
                string.Join(", ", allKeys));
        }
    }
}
