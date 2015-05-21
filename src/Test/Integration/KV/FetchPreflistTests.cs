// <copyright file="FetchPreflistTests.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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

namespace Test.Integration.CRDT
{
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.KV;

    public class FetchPreflistTests : TestBase
    {
        protected override RiakString BucketType
        {
            get { return new RiakString("plain"); }
        }

        protected override RiakString Bucket
        {
            get { return new RiakString("fetch_preflists"); }
        }

        [Test]
        public void Can_Fetch_Preflist()
        {
            RiakMinVersion(2, 1, 0);

            var fetch = new FetchPreflist.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey("key_1")
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            if (rslt.IsSuccess)
            {
                PreflistResponse response = fetch.Response;
                Assert.IsNotNull(response);

                Assert.AreEqual(3, response.Value.Count());
            }
            else
            {
                // TODO: remove this else case when this fix is in Riak:
                // https://github.com/basho/riak_kv/pull/1116
                // https://github.com/basho/riak_core/issues/706
                bool foundMessage =
                    (rslt.ErrorMessage.IndexOf("Permission denied") >= 0 && rslt.ErrorMessage.IndexOf("riak_kv.get_preflist") >= 0) ||
                    (rslt.ErrorMessage.IndexOf("error:badarg") >= 0 && rslt.ErrorMessage.IndexOf("characters_to_binary") >= 0);
                Assert.True(foundMessage, rslt.ErrorMessage);
            }
        }
    }
}