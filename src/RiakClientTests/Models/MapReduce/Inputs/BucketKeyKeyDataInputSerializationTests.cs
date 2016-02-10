namespace RiakClientTests.Models.MapReduce.Inputs
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RiakClient.Models;
    using RiakClient.Models.MapReduce.Inputs;

    [TestFixture, UnitTest]
    public class BucketKeyKeyDataInputSerializationTests : MapReduceSerializationTestsBase
    {
        private const string SerializedRiakBucketKeyKeyDataInput =
            "\"inputs\":[" +
            "[\"foo\",\"bar\",\"baz\"]," +
            "[\"foo\",\"baz\",130]," +
            "[\"dooby\",\"scooby\",{\"la\":\"ding\",\"ray\":\"me\",\"wit\":0}]," +
            "[\"foo\",\"baz\",130]," +
            "[\"dooby\",\"scooby\",{\"la\":\"ding\",\"ray\":\"me\",\"wit\":0}]]";

        [Test]
        public void RiakBucketKeyKeyDataInputSerializesCorrectlyOldInterface()
        {
            var inputs = new List<Tuple<string, string, object>>
            {
                new Tuple<string, string, object>("foo", "baz", 130),
                new Tuple<string, string, object>("dooby", "scooby", new {la = "ding", ray = "me", wit = 0})
            };

#pragma warning disable 612, 618
            var input = new RiakBucketKeyKeyDataInput()
                .Add("foo", "bar", "baz")
                .Add(inputs) // IEnumerable based builder
                .Add(inputs[0], inputs[1]); // Params based builder
#pragma warning restore 612, 618

            var s = Serialize(input.WriteJson);

            Assert.AreEqual(s, SerializedRiakBucketKeyKeyDataInput);
        }

        [Test]
        public void RiakBucketKeyKeyDataInputSerializesCorrectly()
        {
            var inputs = new List<Tuple<RiakObjectId, object>>
            {
                new Tuple<RiakObjectId, object>(new RiakObjectId("foo", "baz"), 130),
                new Tuple<RiakObjectId, object>(new RiakObjectId("dooby", "scooby"),
                    new {la = "ding", ray = "me", wit = 0})
            };

            var input = new RiakBucketKeyKeyDataInput()
                .Add(new RiakObjectId("foo", "bar"), "baz")
                .Add(inputs) // IEnumerable based builder
                .Add(inputs[0], inputs[1]); // Params based builder


            var s = Serialize(input.WriteJson);

            Assert.AreEqual(s, SerializedRiakBucketKeyKeyDataInput);
        }

        [Test]
        public void RiakBucketKeyKeyDataInputWithTypeSerializesCorrectly()
        {
            var inputs = new List<Tuple<RiakObjectId, object>>
            {
                new Tuple<RiakObjectId, object>(new RiakObjectId("qux_type", "foo", "baz"), 130),
                new Tuple<RiakObjectId, object>(new RiakObjectId("dog_type", "dooby", "scooby"),
                    new {la = "ding", ray = "me", wit = 0})
            };

            var input = new RiakBucketKeyKeyDataInput()
                .Add(new RiakObjectId("qux_type", "foo", "bar"), "")
                .Add(new RiakObjectId("qux_type", "foo", "bar"), "baz")
                .Add(inputs) // IEnumerable based builder
                .Add(inputs[0], inputs[1]); // Params based builder


            var s = Serialize(input.WriteJson);

            Assert.AreEqual(s,
                "\"inputs\":[" +
                "[\"foo\",\"bar\",\"\",\"qux_type\"]," +
                "[\"foo\",\"bar\",\"baz\",\"qux_type\"]," +
                "[\"foo\",\"baz\",130,\"qux_type\"]," +
                "[\"dooby\",\"scooby\",{\"la\":\"ding\",\"ray\":\"me\",\"wit\":0},\"dog_type\"]," +
                "[\"foo\",\"baz\",130,\"qux_type\"]," +
                "[\"dooby\",\"scooby\",{\"la\":\"ding\",\"ray\":\"me\",\"wit\":0},\"dog_type\"]]");
        }
    }
}

