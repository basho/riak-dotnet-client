namespace RiakClientTests.Deprecated
{
    using System;
    using System.Linq;
    using Live;
    using Live.MapReduce;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Extensions;
    using RiakClient.Models;
    using RiakClient.Models.MapReduce;
    using RiakClient.Models.MapReduce.Inputs;
    using RiakClient.Models.Search;

    [TestFixture, DeprecatedTest]
    public class RiakSearchMapReduceInputTests : RiakMapReduceTestBase
    {
        private new const string Bucket = "riak_search_bucket";
        private const string RiakSearchKey = "a.hacker";
        private const string RiakSearchKey2 = "a.public";
        private const string RiakSearchDoc = "{\"name\":\"Alyssa P. Hacker\", \"bio\":\"I'm an engineer, making awesome things.\", \"favorites\":{\"book\":\"The Moon is a Harsh Mistress\",\"album\":\"Magical Mystery Tour\", }}";
        private const string RiakSearchDoc2 = "{\"name\":\"Alan Q. Public\", \"bio\":\"I'm an exciting mathematician\", \"favorites\":{\"book\":\"Prelude to Mathematics\",\"album\":\"The Fame Monster\"}}";

        [TestFixtureSetUp]
        public override void SetUp()
        {
            base.SetUp();

            var props = Client.GetBucketProperties(Bucket).Value;
            props.SetLegacySearch(true);
            Client.SetBucketProperties(Bucket, props);

            PrepSearchData();
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
        public void SearchingByNameReturnsTheObjectId()
        {
#pragma warning disable 612, 618
            var mr = new RiakMapReduceQuery().Inputs(new RiakBucketSearchInput(Bucket, "name:Al*"));
#pragma warning restore 612, 618

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var phaseResults = result.Value.PhaseResults.ToList();
            phaseResults.Count.ShouldEqual(1);

            CheckThatResultContainAllKeys(result);
        }

        [Test]
        public void SearchingViaFluentSearchObjectWorks()
        {
            var search = new RiakFluentSearch(Bucket, "name").Search(Token.StartsWith("Al")).Build();
#pragma warning disable 612, 618
            var mr = new RiakMapReduceQuery().Inputs(new RiakBucketSearchInput(search));
#pragma warning restore 612, 618

            var result = Client.MapReduce(mr);
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
