namespace RiakClientTests.Models.MapReduce.Inputs
{
    using NUnit.Framework;
    using RiakClient.Models.MapReduce.Inputs;

    [TestFixture, UnitTest]
    public class SearchInputSerializationTests : MapReduceSerializationTestsBase
    {
        [Test]
        public void TestLegacySerialization()
        {
#pragma warning disable 618
            var input = new RiakBucketSearchInput("my_bucket", "my_query");
#pragma warning restore 618
            var json = Serialize(input.WriteJson);
            Assert.AreEqual("\"inputs\":{\"bucket\":\"my_bucket\",\"query\":\"my_query\"}", json);
        }

        [Test]
        public void TestYokozunaSerialization()
        {
            var input = new RiakSearchInput("my_index", "my_query");
            var json = Serialize(input.WriteJson);
            Assert.AreEqual("\"inputs\":{\"index\":\"my_index\",\"query\":\"my_query\"}", json);
        }
    }
}
