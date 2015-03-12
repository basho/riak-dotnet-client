// <copyright file="SecondaryIndexes.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2015 - Basho Technologies, Inc.
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
// specific language governing permidssions and limitations
// under the License.
// </copyright>

namespace RiakClientExamples.Dev.Using
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;
    using RiakClient.Models.Index;

    /*
     * http://docs.basho.com/riak/latest/dev/using/2i/
     */
    public sealed class SecondaryIndexes : ExampleBase
    {
        [Test]
        public void InsertingObjectsWithSecondaryIndexes()
        {
            PutJohnSmith();
        }

        [Test]
        public void QueryingObjectsWithSecondaryIndexes()
        {
            PutJohnSmith();

            var idxId = new RiakIndexId("default", "users", "twitter");
            var rslt = client.GetSecondaryIndex(idxId, "jsmith123");
            CheckResult(rslt);

            var idxRslt = rslt.Value;
            foreach (var keyTerm in idxRslt.IndexKeyTerms)
            {
                Debug.WriteLine(keyTerm.Key);
            }
        }

        [Test]
        public void IndexingFourObjects()
        {
            ids = PutObjectsWithIndexValues();

            var idxId = new RiakIndexId("indexes", "people", "field1");
            var rslt = client.GetSecondaryIndex(idxId, "val1", "val4");
            CheckResult(rslt);
            PrintKeys(rslt.Value);
            DeleteObjects(ids);
        }

        [Test]
        public void InvalidFieldNamesAndTypes()
        {
            id = new RiakObjectId("indexes", "mykey", "test value");
            var obj = new RiakObject("indexes", "mykey", "test value");
            var intIdx = obj.IntIndex("test-int-idx");
            Assert.Throws<FormatException>(() => intIdx.Add("invalid-value"));
        }

        [Test]
        public void QueryingExactMatch()
        {
            ids = PutObjectsWithIndexValues();

            var idxId = new RiakIndexId("indexes", "people", "field1");
            var rslt = client.GetSecondaryIndex(idxId, "val1");
            CheckResult(rslt);
            Assert.AreEqual(1, rslt.Value.IndexKeyTerms.Count());
            PrintKeys(rslt.Value);
        }

        [Test]
        public void QueryingIntegerIndex()
        {
            ids = PutObjectsWithIndexValues();

            var idxId = new RiakIndexId("indexes", "people", "field2");
            var rslt = client.GetSecondaryIndex(idxId, 1001);
            CheckResult(rslt);
            Assert.AreEqual(1, rslt.Value.IndexKeyTerms.Count());
            PrintKeys(rslt.Value);
        }

        [Test]
        public void QueryingBinaryRange()
        {
            ids = PutObjectsWithIndexValues();

            var idxId = new RiakIndexId("indexes", "people", "field1");
            var rslt = client.GetSecondaryIndex(idxId, "val2", "val4");
            CheckResult(rslt);
            Assert.AreEqual(3, rslt.Value.IndexKeyTerms.Count());
            PrintKeys(rslt.Value);
        }

        [Test]
        public void QueryingIntegerRange()
        {
            ids = PutObjectsWithIndexValues();

            var idxId = new RiakIndexId("indexes", "people", "field2");
            var rslt = client.GetSecondaryIndex(idxId, 1002, 1004);
            CheckResult(rslt);
            Assert.AreEqual(3, rslt.Value.IndexKeyTerms.Count());
            PrintKeys(rslt.Value);
        }

        [Test]
        public void QueryingIntegerRangeWithReturnTerms()
        {
            ids = PutObjectsWithIndexValues();

            var idxId = new RiakIndexId("indexes", "people", "field2");
            var options = new RiakIndexGetOptions();
            options.SetReturnTerms(true);
            var rslt = client.GetSecondaryIndex(idxId, 1002, 1004, options);
            CheckResult(rslt);
            Assert.AreEqual(3, rslt.Value.IndexKeyTerms.Count());
            PrintKeys(rslt.Value, printTerms: true);
        }

        [Test]
        public void Pagination()
        {
            ids = PutObjectsWithIndexValues();

            var idxId = new RiakIndexId("indexes", "people", "field2");
            var options = new RiakIndexGetOptions();
            options.SetMaxResults(2);
            var rslt = client.GetSecondaryIndex(idxId, 1002, 1004, options);
            CheckResult(rslt);
            Assert.AreEqual(2, rslt.Value.IndexKeyTerms.Count());
            PrintKeys(rslt.Value);

            options.SetContinuation(rslt.Continuation);
            rslt = client.GetSecondaryIndex(idxId, 1002, 1004, options);
            CheckResult(rslt);
            Assert.AreEqual(1, rslt.Value.IndexKeyTerms.Count());
            PrintKeys(rslt.Value);
        }

        [Test]
        public void Streaming()
        {
            ids = PutObjectsWithIndexValues();

            var idxId = new RiakIndexId("indexes", "people", "field2");
            var rslt = client.StreamGetSecondaryIndex(idxId, 1001, 1004);
            CheckResult(rslt);

            ushort count = 0;
            foreach (var kt in rslt.Value.IndexKeyTerms)
            {
                PrintKeyTerm(kt);
                ++count;
            }
            Assert.AreEqual(4, count);
        }

        [Test]
        public void ReturnKeysViaDollarBucketIndex()
        {
            ids = PutObjectsWithIndexValues();

            var idxId = new RiakIndexId("indexes", "people", "$bucket");
            var rslt = client.GetSecondaryIndex(idxId, "_");
            CheckResult(rslt);
            PrintKeys(rslt.Value);
        }

        private void PutJohnSmith()
        {
            id = new RiakObjectId("default", "users", "john_smith");
            var obj = new RiakObject(id, "...user data...",
                RiakConstants.ContentTypes.TextPlain);
            obj.BinIndex("twitter").Set("jsmith123");
            obj.BinIndex("email").Set("jsmith@basho.com");
            CheckResult(client.Put(obj));
        }

        private ICollection<RiakObjectId> PutObjectsWithIndexValues()
        {
            var ids = new List<RiakObjectId>();

            var larryId = new RiakObjectId("indexes", "people", "larry");
            var larry = new RiakObject(larryId, "My name is Larry",
                RiakConstants.ContentTypes.TextPlain);

            larry.BinIndex("field1").Set("val1");
            larry.IntIndex("field2").Set(1001);

            ids.Add(larryId);
            CheckResult(client.Put(larry));

            var moeId = new RiakObjectId("indexes", "people", "moe");
            var moe = new RiakObject(moeId, "My name is Moe",
                RiakConstants.ContentTypes.TextPlain);

            moe.BinIndex("Field1").Set("val2");
            moe.IntIndex("Field2").Set(1002);

            ids.Add(moeId);
            CheckResult(client.Put(moe));

            var curlyId = new RiakObjectId("indexes", "people", "curly");
            var curly = new RiakObject(curlyId, "My name is Curly",
                RiakConstants.ContentTypes.TextPlain);

            curly.BinIndex("FIELD1").Set("val3");
            curly.IntIndex("FIELD2").Set(1003);

            ids.Add(curlyId);
            CheckResult(client.Put(curly));

            var veronicaId = new RiakObjectId("indexes", "people", "veronica");
            var veronica = new RiakObject(veronicaId, "My name is Curly",
                RiakConstants.ContentTypes.TextPlain);

            veronica.BinIndex("FIELD1").Set(new string[] { "val4", "val4" });
            veronica.IntIndex("FIELD2").Set(new BigInteger[] {
                1004, 1005, 1006, 1004, 1004, 1007
            });

            ids.Add(veronicaId);
            CheckResult(client.Put(veronica));

            return ids;
        }

        private static void PrintKeys(RiakIndexResult rslt, bool printTerms = false)
        {
            foreach (var kt in rslt.IndexKeyTerms)
            {
                PrintKeyTerm(kt, printTerms);
            }
        }

        private static void PrintKeyTerm(RiakIndexKeyTerm kt, bool printTerm = false)
        {
            if (printTerm)
            {
                var args = new[] { kt.Key, kt.Term };
                Debug.WriteLine(format: "Key: {0} Term: {1}", args: args);
            }
            else
            {
                Debug.WriteLine(kt.Key);
            }
        }
    }
}
