// <copyright file="SetDtEdgeCaseTests.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies
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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RiakClient.Models;
using RiakClient.Models.RiakDt;

#pragma warning disable 618

namespace RiakClientTests.Live.DataTypes
{
    [TestFixture]
    public class SetDtEdgeCaseTests : DataTypeTestsBase
    {
        private static readonly byte[] NoContext = null;
        private static readonly List<string> Adds1 = new List<string> { "foo", "bar", "baz" };
        private static readonly List<string> Removes1 = new List<string> { "foo" };
        private static readonly List<string> Adds2 = new List<string>();
        private static readonly List<string> Removes2 = new List<string> { "fritz" };
        private static readonly List<string> Adds3 = new List<string> { "bar" };
        private static readonly List<string> Removes3 = new List<string>();
        private static readonly List<string> Adds4 = new List<string> { "bar" };
        private static readonly List<string> Removes4 = new List<string> { "bar" };
        private static readonly List<string> Removes5 = new List<string> { "baz" };
        private static readonly List<string> Adds5 = new List<string> { "baz" };
        private static readonly List<string> ConcurrentAdds6 = new List<string> { "concur" };
        private static readonly List<string> ConcurrentRemovess6 = new List<string>();
        private static readonly List<string> Adds6 = new List<string>();
        private static readonly List<string> Removes6 = new List<string> { "concur" };

        [Test]
        public void Test1()
        {
            var id = new RiakObjectId(BucketTypeNames.Sets, Bucket, GetRandomKey("Test1"));

            Assert.Throws<ArgumentNullException>(() => Client.DtUpdateSet(id, Serializer, NoContext, Adds1, Removes1));
        }

        [Test]
        public void Test2NoContext()
        {
            IdContext idContext = BuildStartupSet("SetsTest2NoContext");

            Assert.Throws<ArgumentNullException>(
                () => Client.DtUpdateSet(idContext.Id, Serializer, NoContext, Adds2, Removes2));
        }

        [Test]
        public void Test2WithContext()
        {
            IdContext idContext = BuildStartupSet("SetsTest2WithContext");

            var updatedSet = Client.DtUpdateSet(idContext.Id, Serializer, idContext.Context, Adds2, Removes2);
            var updatedValues = updatedSet.GetObjects(Deserializer).ToList();
            updatedValues.Count.ShouldEqual(2);
            updatedValues.ShouldContain("bar");
            updatedValues.ShouldContain("baz");
        }

        [Test]
        public void Test3NoContext()
        {
            IdContext idContext = BuildStartupSet("SetsTest3NoContext");
            var updatedSet = Client.DtUpdateSet(idContext.Id, Serializer, NoContext, Adds3, Removes3);
            var updatedValues = updatedSet.GetObjects(Deserializer).ToList();
            updatedValues.Count.ShouldEqual(2);
            updatedValues.ShouldContain("bar");
            updatedValues.ShouldContain("baz");
        }

        [Test]
        public void Test3WithContext()
        {
            IdContext idContext = BuildStartupSet("SetsTest3WithContext");
            var updatedSet = Client.DtUpdateSet(idContext.Id, Serializer, idContext.Context, Adds3, Removes3);
            var updatedValues = updatedSet.GetObjects(Deserializer).ToList();
            updatedValues.Count.ShouldEqual(2);
            updatedValues.ShouldContain("bar");
            updatedValues.ShouldContain("baz");
        }

        [Test]
        public void Test4NoContext()
        {
            IdContext idContext = BuildStartupSet("SetsTest4NoContext");
            Assert.Throws<ArgumentNullException>(
                () => Client.DtUpdateSet(idContext.Id, Serializer, NoContext, Adds4, Removes4));
        }

        [Test]
        public void Test4WithContext()
        {
            IdContext idContext = BuildStartupSet("SetsTest4WithContext");
            var updatedSet = Client.DtUpdateSet(idContext.Id, Serializer, idContext.Context, Adds4, Removes4);
            var updatedValues = updatedSet.GetObjects(Deserializer).ToList();
            updatedValues.Count.ShouldEqual(2);
            updatedValues.ShouldContain("bar");
            updatedValues.ShouldContain("baz");
        }

        [Test]
        public void Test5NoContext()
        {
            IdContext idContext = BuildStartupSet("SetsTest5NoContext");
            Assert.Throws<ArgumentNullException>(
                () => Client.DtUpdateSet(idContext.Id, Serializer, NoContext, Adds5, Removes5));
        }

        [Test]
        public void Test5WithContext()
        {
            IdContext idContext = BuildStartupSet("SetsTest5WithContext");
            var updatedSet = Client.DtUpdateSet(idContext.Id, Serializer, idContext.Context, Adds5, Removes5);
            var updatedValues = updatedSet.GetObjects(Deserializer).ToList();
            updatedValues.Count.ShouldEqual(2);
            updatedValues.ShouldContain("bar");
            updatedValues.ShouldContain("baz");
        }

        [Test]
        public void Test6OldContext()
        {
            IdContext oldestIdContext = BuildStartupSet("Test6OldContext");
            var update1 = Client.DtUpdateSet(oldestIdContext.Id, Serializer, oldestIdContext.Context, ConcurrentAdds6, ConcurrentRemovess6);
            update1.Result.IsSuccess.ShouldBeTrue();
            var update2 = Client.DtUpdateSet(oldestIdContext.Id, Serializer, oldestIdContext.Context, Adds6, Removes6);
            var updatedValues = update2.GetObjects(Deserializer).ToList();
            updatedValues.Count.ShouldEqual(3);
            updatedValues.ShouldContain("bar");
            updatedValues.ShouldContain("baz");
            updatedValues.ShouldContain("concur");
        }

        [Test]
        public void Test6WithNewContext()
        {
            IdContext oldestIdContext = BuildStartupSet("Test6NewContext");
            var update1 = Client.DtUpdateSet(oldestIdContext.Id, Serializer, oldestIdContext.Context, ConcurrentAdds6, ConcurrentRemovess6);
            var update2 = Client.DtUpdateSet(oldestIdContext.Id, Serializer, update1.Context, Adds6, Removes6);
            var updatedValues = update2.GetObjects(Deserializer).ToList();
            updatedValues.Count.ShouldEqual(2);
            updatedValues.ShouldContain("bar");
            updatedValues.ShouldContain("baz");
        }

        private IdContext BuildStartupSet(string name)
        {
            var id = new RiakObjectId(BucketTypeNames.Sets, Bucket, GetRandomKey(name));
            var adds = new List<string> { "bar", "baz" };
            var startingSet = Client.DtUpdateSet(id, Serializer, null, adds);
            startingSet.Result.IsSuccess.ShouldBeTrue("Initial set setup failed: " + startingSet.Result.ErrorMessage);

            List<string> startingValues = startingSet.GetObjects(Deserializer).ToList();
            startingValues.Count.ShouldEqual(2, "Initial set setup failed");
            startingValues.ShouldContain("bar", "Initial set setup failed");
            startingValues.ShouldContain("baz", "Initial set setup failed");

            return new IdContext(startingSet);
        }

        private class IdContext
        {
            public IdContext(RiakDtSetResult startingSet)
            {
                Id = startingSet.Result.Value.ToRiakObjectId();
                Context = startingSet.Context;
            }

            public RiakObjectId Id { get; set; }
            public byte[] Context { get; set; }
        }
    }
}

#pragma warning restore 618