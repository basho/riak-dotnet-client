// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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
using System.Linq;
using CorrugatedIron.Comms;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.Extensions;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.MapReduce
{
    [TestFixture]
    public class WhenUsingSearchAsInput : RiakMapReduceTestBase
    {
        private const string BucketType = "search_type";
        private new const string Bucket = "yoko_bucket";
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
        public void SetUp()
        {
            Cluster = new RiakCluster(ClusterConfig, new RiakConnectionFactory());
            Client = Cluster.CreateClient();
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
#pragma warning disable 618
                .Inputs(new RiakBucketSearchInput(Index, "name_s:" + _randomId + "Al*"));
#pragma warning restore 618


            Func<RiakResult<RiakMapReduceResult>> doMapReduce = () => Client.MapReduce(mr);

            var result = doMapReduce.WaitUntil(OnePhaseWithTwoResultsFound);

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var phaseResults = result.Value.PhaseResults.ToList();
            phaseResults.Count.ShouldEqual(1);

            CheckThatResultContainAllKeys(result);
        }

        private bool OnePhaseWithTwoResultsFound(RiakResult<RiakMapReduceResult> result)
        {
            if (!result.IsSuccess || result.Value == null) return false;

            var phaseResults = result.Value.PhaseResults.ToList();

            if (phaseResults.Count != 1) return false;

            var phase1Results = phaseResults[0].Values;

            return phase1Results.Count == 2;

        }

        [Test]
        public void SearchingViaFluentSearchObjectWorks()
        {
            var search = new RiakFluentSearch(Index, "name_s").Search(Token.StartsWith(_randomId + "Al")).Build();
            var mr = new RiakMapReduceQuery()
#pragma warning disable 618
                .Inputs(new RiakBucketSearchInput(search));
#pragma warning restore 618


            Func<RiakResult<RiakMapReduceResult>> doMapReduce = () => Client.MapReduce(mr);

            var result = doMapReduce.WaitUntil(OnePhaseWithTwoResultsFound);

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            CheckThatResultContainAllKeys(result);
        }

        private static void CheckThatResultContainAllKeys(RiakResult<RiakMapReduceResult> result)
        {
            var phaseResults = result.Value.PhaseResults.ToList();
            phaseResults.Count.ShouldEqual(1);

            var searchResults = phaseResults[0];
            searchResults.Values.ShouldNotBeNull();
            searchResults.Values.Count.ShouldEqual(2);

            foreach (var searchResult in searchResults.Values)
            {
                var s = searchResult.FromRiakString();
                if (!(s.Contains(RiakSearchKey) || s.Contains(RiakSearchKey2)))
                    Assert.Fail("Results did not contain either \"{0}\" or \"{1}\". \r\nResult was:\"{2}\"", RiakSearchKey,
                        RiakSearchKey2, s);
            }
        }

    }
}
