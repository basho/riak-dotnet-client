// <copyright file="BucketPropertyTests.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

namespace RiakClientTests.Deprecated
{
    using System;
    using System.Linq;
    using Live;
    using NUnit.Framework;
    using RiakClient.Models;
    using RiakClient.Models.Search;

    [TestFixture, DeprecatedTest]
    public class WhenDealingWithBucketProperties : LiveRiakConnectionTestBase
    {
        // use the one node configuration here because we might run the risk
        // of hitting different nodes in the configuration before the props
        // are replicated to other nodes.

        [Test()]
        public void SettingLegacySearchOnRiakBucketMakesBucketSearchable()
        {
            var bucket = Guid.NewGuid().ToString();
            var key = Guid.NewGuid().ToString();
            var props = Client.GetBucketProperties(bucket).Value;
            props.SetLegacySearch(true);

            var setResult = Client.SetBucketProperties(bucket, props);
            setResult.IsSuccess.ShouldBeTrue(setResult.ErrorMessage);

            var obj = new RiakObject(bucket, key, new { name = "OJ", age = 34 });
            var putResult = Client.Put(obj);
            putResult.IsSuccess.ShouldBeTrue(putResult.ErrorMessage);

            var q = new RiakFluentSearch(bucket, "name")
                .Search("OJ")
                .And("age", "34")
                .Build();

            var search = new RiakSearchRequest
            {
                Query = q
            };

            var searchResult = Client.Search(search);
            searchResult.IsSuccess.ShouldBeTrue(searchResult.ErrorMessage);
            searchResult.Value.NumFound.ShouldEqual(1u);
            searchResult.Value.Documents[0].Fields.Count.ShouldEqual(3);
            searchResult.Value.Documents[0].Fields.First(x => x.Key == "id").Value.ShouldEqual(key);
        }
    }
}
