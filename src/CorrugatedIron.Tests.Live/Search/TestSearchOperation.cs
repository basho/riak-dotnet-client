// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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
using System.Linq;
using System.Threading;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.Search
{
    [TestFixture]
    public class TestSearchOperation : LiveRiakConnectionTestBase
    {
        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            var index = new SearchIndex(Index);
            Client.PutSearchIndex(index);
            var props = Client.GetBucketProperties(BucketType, Bucket).Value;
            props.SetSearchIndex(Index);
            Client.SetBucketProperties(BucketType, Bucket, props);

        }

        private const string BucketType = "search_type";
        private const string Bucket = "yoko_bucket";
        private const string Index = "yoko_index";
        private const string RiakSearchKey = "a.hacker";
        private const string RiakSearchKey2 = "a.public";
        private readonly Random _random = new Random();

        private string RiakSearchDoc =
            "{{\"name_s\":\"Alyssa{0}P. Hacker\", \"age_i\":35, \"leader_b\":true, \"bio_s\":\"I'm an engineer, making awesome things.\", \"favorites_s\":{{\"book_s\":\"The Moon is a Harsh Mistress\",\"album_s\":\"Magical Mystery Tour\", }}}}";
        
        private string RiakSearchDoc2 =
            "{{\"name_s\":\"Alan{0} Q. Public\", \"age_i\":38, \"bio_s\":\"I'm an exciting awesome mathematician\", \"favorites_s\":{{\"book_s\":\"Prelude to Mathematics\",\"album_s\":\"The Fame Monster\"}}}}";

        [Test]
        public void SearchingWithSimpleFluentQueryWorksCorrectly()
        {
            var randomId = _random.Next();
            var alyssaKey = RiakSearchKey + randomId;
            var alanKey = RiakSearchKey2 + randomId;
            var alyssaDoc = String.Format(RiakSearchDoc, randomId);
            var alanDoc = String.Format(RiakSearchDoc2, randomId);


            Console.WriteLine("Using {0}, {1} for SearchingWithSimpleFluentQueryWorksCorrectly() key", alyssaKey, alanKey);

            Client.Put(new RiakObject(BucketType, Bucket, alyssaKey , alyssaDoc.ToRiakString(),
                RiakConstants.ContentTypes.ApplicationJson, RiakConstants.CharSets.Utf8));

            Client.Put(new RiakObject(BucketType, Bucket, alanKey, alanDoc.ToRiakString(),
                RiakConstants.ContentTypes.ApplicationJson, RiakConstants.CharSets.Utf8));

            
            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch(Index, "name_s")
                                .Search("Alyssa" + randomId + "*")
                                .Build()
            };

            Func<RiakResult<RiakSearchResult>, bool> successCriteria =
                result => result.IsSuccess && 
                          result.Value != null && 
                          result.Value.NumFound > 0;

            Func<RiakResult<RiakSearchResult>> func = 
                () => Client.Search(req);

            var searchResult = func.WaitUntil(successCriteria);

            searchResult.IsSuccess.ShouldBeTrue(searchResult.ErrorMessage);
            searchResult.Value.NumFound.ShouldEqual(1u);
            searchResult.Value.Documents.Count.ShouldEqual(1);
            searchResult.Value.Documents[0].Fields.Count.ShouldEqual(11); // [ score, _yz_rb, _yz_rt, _yz_rk, _yz_id, name_s, age_i, leader_b, bio_s favorites_s.book_s, favorites_s.album_s ]
            searchResult.Value.Documents[0].Key.ShouldEqual(alyssaKey);
        }
    }
}