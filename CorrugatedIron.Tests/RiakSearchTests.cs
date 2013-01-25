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

using CorrugatedIron.Models.Search;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

            var expected = @"key:This\ is\\\ a\ \""Test\""\ to\ make\ \'sure\'\ it\ \(the\ text\)\ is\ \[characterised\]\ correctly\ \(master\:slave\)\ \+\ includes\ \-\ this\ url\:\ http\:\/\/foo.com\/bar\?baz=quux";
            Assert.AreEqual(expected, query);
        }

        [Test]
        public void SimpleIndexFieldUnaryTermSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "field")
                .Search("foo")
                .Build();
            var q = s.ToString();
            Assert.AreEqual("field:foo", q);
        }

        [Test]
        public void SimpleUnaryTermWithBoostSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Boost(5)
                .Build();
            var q = s.ToString();
            Assert.AreEqual("key:foo^5", q);
        }

        [Test]
        public void SimpleUnaryTermWithProximitySerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Proximity(5, "foo", "bar", "baz")
                .Build();
            var q = s.ToString();
            Assert.AreEqual(@"key:""foo bar baz""~5", q);
        }

        [Test]
        public void SimpleAndTermSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .And("bar")
                .Build();
            var q = s.ToString();
            Assert.AreEqual("key:foo AND key:bar", q);
        }

        [Test]
        public void SimpleRangeTermSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Between("10", "20", false)
                .Build();
            var q = s.ToString();
            Assert.AreEqual("key:{10 TO 20}", q);
        }

        [Test]
        public void SimpleInclusiveRangeTermSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Between("10", "20")
                .Build();
            var q = s.ToString();
            Assert.AreEqual("key:[10 TO 20]", q);
        }

        [Test]
        public void SimpleOrTermSerializesCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Or("bar")
                .Build();
            var q = s.ToString();
            Assert.AreEqual("key:foo OR key:bar", q);
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
            Assert.AreEqual("key:foo OR key:bar AND key:baz", q);
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
            Assert.AreEqual("key:foo OR key:bar^3 AND key:baz^5", q);
        }

        [Test]
        public void InitialGroupedTermsSerializeCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Group("foo", t => t.Or("bar").And("baz", x => x.And("schmoopy")))
                .Or("bar", t => t.And("slop"))
                .Build();
            var q = s.ToString();
            Assert.AreEqual("key:(key:foo OR key:bar AND (key:baz AND key:schmoopy)) OR (key:bar AND key:slop)", q);
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
            Assert.AreEqual("key:foo OR key:bar AND (key:baz OR key:quux) OR (key:baz AND key:schmoopy^6 AND (key:dooby OR key:fooby))", q);
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
            Assert.AreEqual("key:foo OR NOT key:bar AND NOT (key:baz OR key:quux) OR (key:baz AND key:schmoopy^6 AND (key:dooby OR NOT key:fooby))", q);
        }

        [Test]
        public void ComplicatedTermsSerializeCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Or("bar").Not()
                .AndBetween("10", "20")
                .And("baz", t => t.Or("quux").OrBetween("la", "da", false)).Not()
                .AndProximity(3, "testing", "these words")
                .Or("baz", t => t.And("schmoopy for president+")
                    .Boost(6)
                    .And(Token.StartsWith("dooby"), x => x.Or("fooby").Not()))
                .Build();
            var q = s.ToString();
            Assert.AreEqual(@"key:foo OR NOT key:bar AND key:[10 TO 20] AND NOT (key:baz OR key:quux OR key:{la TO da}) AND key:""testing these\ words""~3 OR (key:baz AND key:schmoopy\ for\ president\+^6 AND (key:dooby* OR NOT key:fooby))", q);
        }

        [Test]
        public void ComplicatedTermsWithExtraFieldsSerializeCorrectly()
        {
            var s = new RiakFluentSearch("bucket", "key")
                .Search("foo")
                .Or("bar").Not()
                .AndBetween("10", "20", true)
                .Or("otherkey", "baz", t => t.And("hash", Token.StartsWith("schmoopy for president+"))
                    .Boost(6)
                    .And("bash", "dooby", x => x.Or("dash", "fooby").Not())
                    .Or("smelly"))
                .And("baz", t => t.Or("zoom", "quux").OrBetween("la", "da", false)).Not()
                .OrProximity("lala", 10, "wouldn't", "haven't").Not()
                .Build();
            var q = s.ToString();
            Assert.AreEqual(@"key:foo OR NOT key:bar AND key:[10 TO 20] OR (otherkey:baz AND hash:schmoopy\ for\ president\+*^6 AND (bash:dooby OR NOT dash:fooby) OR bash:smelly) AND NOT (otherkey:baz OR zoom:quux OR zoom:{la TO da}) OR NOT lala:""wouldn\'t haven\'t""~10", q);
        }

        [Test]
        public void FluentSearchOfSameFieldsInALoopSerializesCorrectly()
        {
            var values = new List<string>
            {
                "value1",
                "value2",
                "value3",
                "value4",
                "value5"
            };

            var term = default(Term);

            foreach (var v in values)
            {
                term = term == null ? new RiakFluentSearch("bucket", "field").Search(v) : term.And(v);
            }

            var s = term.Build();

            var q = s.ToString();
            Assert.AreEqual("field:value1 AND field:value2 AND field:value3 AND field:value4 AND field:value5", q);
        }

        [Test]
        public void FluentSearchOfSameFieldsInAListSerializesCorrectly()
        {
            var fieldsAndValues = new List<string>
            {
                "value1",
                "value2",
                "value3",
                "value4",
                "value5"
            };

            var s = fieldsAndValues.Aggregate((Term)null,
                (a, v) => a == null ? new RiakFluentSearch("bucket", "field").Search(v) : a.And(v))
                .Build();

            var q = s.ToString();
            Assert.AreEqual("field:value1 AND field:value2 AND field:value3 AND field:value4 AND field:value5", q);
        }

        [Test]
        public void FluentSearchOfDifferentFieldsInALoopSerializesCorrectly()
        {
            var fieldsAndValues = new List<Tuple<string, string>>
            {
                Tuple.Create("field1", "value1"),
                Tuple.Create("field2", "value2"),
                Tuple.Create("field3", "value3"),
                Tuple.Create("field4", "value4"),
                Tuple.Create("field5", "value5"),
            };

            var term = default(Term);

            foreach(var t in fieldsAndValues)
            {
                term = term == null ? new RiakFluentSearch("bucket", t.Item1).Search(t.Item2) : term.And(t.Item1, t.Item2);
            }

            var s = term.Build();
            var q = s.ToString();

            Assert.AreEqual("field1:value1 AND field2:value2 AND field3:value3 AND field4:value4 AND field5:value5", q);
        }

        [Test]
        public void FluentSearchOfDifferentFieldsInAListSerializesCorrectly()
        {
            var fieldsAndValues = new List<Tuple<string, string>>
            {
                Tuple.Create("field1", "value1"),
                Tuple.Create("field2", "value2"),
                Tuple.Create("field3", "value3"),
                Tuple.Create("field4", "value4"),
                Tuple.Create("field5", "value5"),
            };

            var s = fieldsAndValues.Aggregate((Term)null,
                (a, t) => a == null ? new RiakFluentSearch("bucket", t.Item1).Search(t.Item2) : a.And(t.Item1, t.Item2))
                .Build();

            var q = s.ToString();
            Assert.AreEqual("field1:value1 AND field2:value2 AND field3:value3 AND field4:value4 AND field5:value5", q);
        }
    }
}
