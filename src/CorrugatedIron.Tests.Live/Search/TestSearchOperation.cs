using System;
using System.Collections.Generic;
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

            PrepSearch();
        }

        public TestSearchOperation()
            : base("riak1NodeConfiguration")
        {
        }

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
            
            var searchResult = RunSolrQuery(req).WaitUntil(AnyMatchIsFound);

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

            var searchResult = RunSolrQuery(req).WaitUntil(TwoMatchesFound);
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

            var result = RunSolrQuery(req).WaitUntil(AnyMatchIsFound);

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

            var result = RunSolrQuery(req).WaitUntil(AnyMatchIsFound);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.NumFound.ShouldEqual(1u);
            result.Value.Documents.Count.ShouldEqual(1);
            result.Value.Documents[0].Fields.Count.ShouldEqual(3);
            result.Value.Documents[0].Id.ShouldNotBeNull();
        }


        private Func<RiakResult<RiakSearchResult>> RunSolrQuery(RiakSearchRequest req)
        {
            Func<RiakResult<RiakSearchResult>> runSolrQuery =
                () => Client.Search(req);
            return runSolrQuery;
        }
        
        private static Func<RiakResult<RiakSearchResult>, bool> AnyMatchIsFound
        {
            get
            {
                Func<RiakResult<RiakSearchResult>, bool> matchIsFound =
                    result => result.IsSuccess &&
                              result.Value != null &&
                              result.Value.NumFound > 0;
                return matchIsFound;
            }
        }

        private static Func<RiakResult<RiakSearchResult>, bool> TwoMatchesFound
        {
            get
            {
                Func<RiakResult<RiakSearchResult>, bool> twoMatchesFound =
                    result => result.IsSuccess &&
                              result.Value != null &&
                              result.Value.NumFound == 2;
                return twoMatchesFound;
            }
        }
    }
}