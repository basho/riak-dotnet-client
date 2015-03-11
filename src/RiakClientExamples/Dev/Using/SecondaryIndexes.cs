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
// specific language governing permissions and limitations
// under the License.
// </copyright>

namespace RiakClientExamples.Dev.Using
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Numerics;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;

    public sealed class SecondaryIndexes : ExampleBase
    {
        /*
         * Note: Before running these examples, be sure to set up your
         * cluster to use the leveldb backend as the default and create
         * expected bucket types.
         * 
         * These commands will set up your devrel:
         * tools/setup-dev-cluster -b leveldb
         * tools/setup-examples
         */
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

            var idxId = new RiakIndexId("indexes", "people", "field1");
            var rslt = client.GetSecondaryIndex(idxId, "val1", "val4");
            CheckResult(rslt);

            var idxRslt = rslt.Value;
            foreach (var keyTerm in idxRslt.IndexKeyTerms)
            {
                Debug.WriteLine(keyTerm.Key);
            }

            foreach (var id in ids)
            {
                DeleteObject(id);
            }
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
    }
}
