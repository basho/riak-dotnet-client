namespace RiakClientTests.Models.MapReduce.Inputs
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RiakClient.Models;
    using RiakClient.Models.MapReduce.Inputs;

    [TestFixture, UnitTest]
    public class BucketKeyInputSerializationTests : MapReduceSerializationTestsBase
    {
        private const string SerializedRiakBucketKeyInput =
            "\"inputs\":[" +
            "[\"foo\",\"bar\",\"\"]," +
            "[\"foo\",\"baz\",\"\"]," +
            "[\"dooby\",\"scooby\",\"\"]," +
            "[\"foo\",\"baz\",\"\"]," +
            "[\"dooby\",\"scooby\",\"\"]]";

        private const string SerializedRiakBucketKeyInputWithTypes =
            "\"inputs\":[" +
            "[\"foo\",\"bar\",\"qux_type\"]," +
            "[\"foo\",\"baz\",\"qux_type\"]," +
            "[\"dooby\",\"scooby\",\"dog_type\"]," +
            "[\"foo\",\"baz\",\"qux_type\"]," +
            "[\"dooby\",\"scooby\",\"dog_type\"]]";

        [Test]
        public void RiakBucketKeyInputSerializesCorrectlyOldInterface()
        {
            var inputList = new List<Tuple<string, string>>
            { 
                new Tuple<string, string>("foo", "baz"),
                new Tuple<string, string>("dooby", "scooby")
            };

#pragma warning disable 612, 618
            var input = new RiakBucketKeyInput()
                .Add("foo", "bar")
                .Add(inputList)
                .Add(inputList[0], inputList[1]);
#pragma warning restore 612, 618

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s, SerializedRiakBucketKeyInput);
        }

        [Test]
        public void RiakBucketKeyInputSerializesCorrectly()
        {
            var inputList = new List<RiakObjectId>
            {
                new RiakObjectId("foo", "baz"),
                new RiakObjectId("dooby", "scooby")
            };

            var input = new RiakBucketKeyInput()
                .Add(new RiakObjectId("foo", "bar"))
                .Add(inputList)
                .Add(inputList[0], inputList[1]);

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s, SerializedRiakBucketKeyInput);
        }

        [Test]
        public void RiakBucketKeyInputWithTypesSerializesCorrectly()
        {
            var inputList = new List<RiakObjectId>
            {
                new RiakObjectId("qux_type", "foo", "baz"),
                new RiakObjectId("dog_type", "dooby", "scooby")
            };

            var input = new RiakBucketKeyInput()
                .Add(new RiakObjectId("qux_type", "foo", "bar"))
                .Add(inputList)
                .Add(inputList[0], inputList[1]);

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s,
                SerializedRiakBucketKeyInputWithTypes);
        }

        [Test]
        public void FromRiakObjectIdsHelperMethodSerializesCorrectly()
        {
            var ids = new List<RiakObjectId>
            {
                new RiakObjectId("bazType", "foo", "bar"),
                new RiakObjectId("bazType", "foo", "baz"),
                new RiakObjectId("bazType", "dooby", "scooby")
            };

            var input = RiakBucketKeyInput.FromRiakObjectIds(ids);
            var s = Serialize(input.WriteJson);

            Assert.AreEqual(s,
                "\"inputs\":[[\"foo\",\"bar\",\"bazType\"],[\"foo\",\"baz\",\"bazType\"],[\"dooby\",\"scooby\",\"bazType\"]]");
        }
    }
}

