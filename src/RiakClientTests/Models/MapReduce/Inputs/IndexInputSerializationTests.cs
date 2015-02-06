// <copyright file="IndexInputSerializationTests.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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

namespace RiakClientTests.Models.MapReduce.Inputs
{
    using NUnit.Framework;
    using RiakClient.Models;
    using RiakClient.Models.MapReduce.Inputs;
    using RiakClient.Util;

    [TestFixture]
    public class BinEqualityIndexSerializationTests : MapReduceSerializationTestsBase
    {
        [Test]
        public void TestIndexSuffixIsSetCorrectly()
        {
            var input = new RiakBinIndexEqualityInput(new RiakIndexId("bucket", "index"), "dave");
            input.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Binary).ShouldBeTrue();
        }

        [Test]
        public void TestBinIndexEqualitySerializationOldInterface()
        {
#pragma warning disable 612, 618
            var input = new RiakBinIndexEqualityInput("bucket", "index", "dave");
#pragma warning restore 612, 618

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s, "\"inputs\":{\"bucket\":\"bucket\",\"index\":\"index_bin\",\"key\":\"dave\"}");
        }

        [Test]
        public void TestBinIndexEqualitySerialization()
        {
            var input = new RiakBinIndexEqualityInput(new RiakIndexId("bucket", "index"), "dave");

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s, "\"inputs\":{\"bucket\":\"bucket\",\"index\":\"index_bin\",\"key\":\"dave\"}");
        }

        [Test]
        public void TestBinIndexEqualitySerializationWithType()
        {
            var input = new RiakBinIndexEqualityInput(new RiakIndexId("type", "bucket", "index"), "dave");

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s, "\"inputs\":{\"bucket\":[\"type\",\"bucket\"],\"index\":\"index_bin\",\"key\":\"dave\"}");
        }

        [Test]
        public void TestOldPropertiesWork()
        {
            var input = new RiakBinIndexEqualityInput(new RiakIndexId("bucket", "index"), "dave");
#pragma warning disable 612, 618
            Assert.AreEqual("bucket", input.Bucket);
            Assert.AreEqual("index_bin", input.Index);
#pragma warning restore 612, 618
        }
    }

    [TestFixture]
    public class BinRangeIndexSerializationTests : MapReduceSerializationTestsBase
    {
        [Test]
        public void TestIndexSuffixIsSetCorrectly()
        {
            var input = new RiakBinIndexRangeInput(new RiakIndexId("bucket", "index"), "dave", "ed");
            input.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Binary).ShouldBeTrue();
        }

        [Test]
        public void TestBinIndexRangeSerializationOldInterface()
        {
#pragma warning disable 612, 618
            var input = new RiakBinIndexRangeInput("bucket", "index", "dave", "ed");
#pragma warning restore 612, 618

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s,
                "\"inputs\":{\"bucket\":\"bucket\",\"index\":\"index_bin\",\"start\":\"dave\",\"end\":\"ed\"}");
        }

        [Test]
        public void TestBinIndexRangeSerialization()
        {
            var input = new RiakBinIndexRangeInput(new RiakIndexId("bucket", "index"), "dave", "ed");

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s,
                "\"inputs\":{\"bucket\":\"bucket\",\"index\":\"index_bin\",\"start\":\"dave\",\"end\":\"ed\"}");
        }

        [Test]
        public void TestBinIndexRangeSerializationWithType()
        {
            var input = new RiakBinIndexRangeInput(new RiakIndexId("type", "bucket", "index"), "dave", "ed");

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s,
                "\"inputs\":{\"bucket\":[\"type\",\"bucket\"],\"index\":\"index_bin\",\"start\":\"dave\",\"end\":\"ed\"}");
        }
    }

    [TestFixture]
    public class IntIndexEqualitySerializationTests : MapReduceSerializationTestsBase
    {
        [Test]
        public void TestIndexSuffixIsSetCorrectly()
        {
            var input = new RiakIntIndexEqualityInput(new RiakIndexId("bucket", "index"), 42);
            input.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Integer).ShouldBeTrue();
        }

        [Test]
        public void TestIntIndexEqualitySerializationOldInterface()
        {
#pragma warning disable 612, 618
            var input = new RiakIntIndexEqualityInput("bucket", "index", 42);
#pragma warning restore 612, 618

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s, "\"inputs\":{\"bucket\":\"bucket\",\"index\":\"index_int\",\"key\":\"42\"}");
        }

        [Test]
        public void TestIntIndexEqualitySerialization()
        {
            var input = new RiakIntIndexEqualityInput(new RiakIndexId("bucket", "index"), 42);

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s, "\"inputs\":{\"bucket\":\"bucket\",\"index\":\"index_int\",\"key\":\"42\"}");
        }

        [Test]
        public void TestIntIndexEqualitySerializationWithType()
        {
            var input = new RiakIntIndexEqualityInput(new RiakIndexId("type", "bucket", "index"), 42);

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s, "\"inputs\":{\"bucket\":[\"type\",\"bucket\"],\"index\":\"index_int\",\"key\":\"42\"}");
        }
    }

    [TestFixture]
    public class IntIndexRangeSerializationTests : MapReduceSerializationTestsBase
    {
        [Test]
        public void TestIndexSuffixIsSetCorrectly()
        {
            var input = new RiakIntIndexRangeInput(new RiakIndexId("bucket", "index"), 42, 100);
            input.IndexId.IndexName.EndsWith(RiakConstants.IndexSuffix.Integer).ShouldBeTrue();
        }

        [Test]
        public void TestIntIndexRangeSerializationOldInterface()
        {
#pragma warning disable 612, 618
            var input = new RiakIntIndexRangeInput("bucket", "index", 42, 100);
#pragma warning restore 612, 618

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s,
                "\"inputs\":{\"bucket\":\"bucket\",\"index\":\"index_int\",\"start\":\"42\",\"end\":\"100\"}");
        }

        [Test]
        public void TestIntIndexRangeSerialization()
        {
            var input = new RiakIntIndexRangeInput(new RiakIndexId("bucket", "index"), 42, 100);

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s,
                "\"inputs\":{\"bucket\":\"bucket\",\"index\":\"index_int\",\"start\":\"42\",\"end\":\"100\"}");
        }

        [Test]
        public void TestIntIndexRangeSerializationWithType()
        {
            var input = new RiakIntIndexRangeInput(new RiakIndexId("type", "bucket", "index"), 42, 100);

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s,
                "\"inputs\":{\"bucket\":[\"type\",\"bucket\"],\"index\":\"index_int\",\"start\":\"42\",\"end\":\"100\"}");
        }
    }
}

