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

using NUnit.Framework;
using CorrugatedIron.Models.Search;

namespace CorrugatedIron.Tests
{
    [TestFixture]
    public class SimpleRiakSearchSerializationTests
    {
        [Test]
        public void ValueTextEscapedCorrectly()
        {
            var search = new RiakFluentSearch("bucket", "key")
                .Search(@"This is\ a ""Test"" to make 'sure' it (the text) is [characterised] correctly (master:slave) + includes - this url: http://foo.com/bar?baz=quux")
                .Build();

            var query = search.ToString();

            var expected = @"bucket.key:This\ is\\\ a\ \""Test\""\ to\ make\ \'sure\'\ it\ \(the\ text\)\ is\ \[characterised\]\ correctly\ \(master\:slave\)\ \+\ includes\ \-\ this\ url\:\ http\:\/\/foo.com\/bar\?baz=quux";
            Assert.AreEqual(expected, query);
        }

        [Test]
        public void SimpleIndexFieldUnaryTermSerializesCorrectly()
        {
            var s = new RiakFluentSearch("index", "field")
                .Search("foo")
                .Build();
            var q = s.ToString();
            Assert.AreEqual("index.field:foo", q);
        }

        [Test]
        public void SimpleUnaryTermWithBoostSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Boost(5)
                .Build();
            var q = s.ToString();
            Assert.AreEqual("bucket.key:foo^5", q);
        }

        [Test]
        public void SimpleAndTermSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .And("bar")
                .Build();
            var q = s.ToString();
            Assert.AreEqual("bucket.key:foo AND key:bar", q);
        }

        [Test]
        public void SimpleRangeTermSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("10", "20")
                .Build();
            var q = s.ToString();
            Assert.AreEqual("bucket.key:{10 TO 20}", q);
        }

        [Test]
        public void SimpleInclusiveRangeTermSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("10", "20", true)
                .Build();
            var q = s.ToString();
            Assert.AreEqual("bucket.key:[10 TO 20]", q);
        }

        [Test]
        public void SimpleOrTermSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Or("bar")
                .Build();
            var q = s.ToString();
            Assert.AreEqual("bucket.key:foo OR key:bar", q);
        }

        [Test]
        public void SimpleOrAndTermSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Or("bar")
                .And("baz")
                .Build();
            var q = s.ToString();
            Assert.AreEqual("bucket.key:foo OR key:bar AND key:baz", q);
        }

        [Test]
        public void SimpleOrAndTermWithBoostSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Or("bar").Boost(3)
                .And("baz").Boost(5)
                .Build();
            var q = s.ToString();
            Assert.AreEqual("bucket.key:foo OR key:bar^3 AND key:baz^5", q);
        }

        [Test]
        public void InitialGroupedTermsSerializeCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Group("foo", t => t.Or("bar").And("baz", x => x.And("schmoopy")))
                .Or("bar", t => t.And("slop"))
                .Build();
            var q = s.ToString();
            Assert.AreEqual("bucket.key:(key:foo OR key:bar AND (key:baz AND key:schmoopy)) OR (key:bar AND key:slop)", q);
        }

        [Test]
        public void GroupedTermsSerializeCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Or("bar")
                .And("baz", t => t.Or("quux"))
                .Or("baz", t => t.And("schmoopy")
                    .Boost(6)
                    .And("dooby", x => x.Or("fooby")))
                .Build();
            var q = s.ToString();
            Assert.AreEqual("bucket.key:foo OR key:bar AND (key:baz OR key:quux) OR (key:baz AND key:schmoopy^6 AND (key:dooby OR key:fooby))", q);
        }

        [Test]
        public void GroupedNotTermsSerializeCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Or("bar").Not()
                .And("baz", t => t.Or("quux")).Not()
                .Or("baz", t => t.And("schmoopy")
                    .Boost(6)
                    .And("dooby", x => x.Or("fooby").Not()))
                .Build();
            var q = s.ToString();
            Assert.AreEqual("bucket.key:foo OR NOT key:bar AND NOT (key:baz OR key:quux) OR (key:baz AND key:schmoopy^6 AND (key:dooby OR NOT key:fooby))", q);
        }

        [Test]
        public void ComplicatedTermsSerializeCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Or("bar").Not()
                .AndRange("10", "20", true)
                .And("baz", t => t.Or("quux").OrRange("la", "da")).Not().Proximity(10)
                .Or("baz", t => t.And("schmoopy for president+")
                    .Boost(6)
                    .And("dooby", x => x.Or("fooby").Not()))
                .Build();
            var q = s.ToString();
            Assert.AreEqual(@"bucket.key:foo OR NOT key:bar AND key:[10 TO 20] AND NOT (key:baz OR key:quux OR key:{la TO da})~10 OR (key:baz AND key:schmoopy\ for\ president\+^6 AND (key:dooby OR NOT key:fooby))", q);
        }

        [Test]
        public void ComplicatedTermsWithExtraFieldsSerializeCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Or("bar").Not()
                .AndRange("10", "20", true)
                .Or("otherkey", "baz", t => t.And("schmoopy for president+")
                    .Boost(6)
                    .And("bash", "dooby", x => x.Or("dash", "fooby").Not())
                    .Or("smelly"))
                .And("baz", t => t.Or("quux").OrRange("la", "da")).Not().Proximity(10)
                .Build();
            var q = s.ToString();
            Assert.AreEqual(@"bucket.key:foo OR NOT key:bar AND key:[10 TO 20] OR (otherkey:baz AND otherkey:schmoopy\ for\ president\+^6 AND (bash:dooby OR NOT dash:fooby) OR key:smelly) AND NOT (key:baz OR key:quux OR key:{la TO da})~10", q);
        }

    }
}
