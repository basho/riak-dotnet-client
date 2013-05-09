// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
// 
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System.Collections.Generic;
using CorrugatedIron.Models;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live
{
    [TestFixture]
    public class WhenQueryingRiakSearchViaPbc : LiveRiakConnectionTestBase
    {
        private const string Bucket = "riak_search_bucket";
        private const string RiakSearchKey = "a.hacker";
        private const string RiakSearchKey2 = "a.public";
        private const string RiakSearchDoc = "{\"name\":\"Alyssa P. Hacker\", \"bio\":\"I'm an engineer, making awesome things.\", \"favorites\":{\"book\":\"The Moon is a Harsh Mistress\",\"album\":\"Magical Mystery Tour\", }}";
        private const string RiakSearchDoc2 = "{\"name\":\"Alan Q. Public\", \"bio\":\"I'm an exciting awesome mathematician\", \"favorites\":{\"book\":\"Prelude to Mathematics\",\"album\":\"The Fame Monster\"}}";

        [SetUp]
        public new void SetUp() 
        {
            base.SetUp();
            
            var props = Client.GetBucketProperties(Bucket, true).Value;
            props.SetSearch(true);
            Client.SetBucketProperties(Bucket, props);
        }

        [Test]
        public void SearchingWithSimpleFluentQueryWorksCorrectly()
        {
            Client.Put(new RiakObject(Bucket, RiakSearchKey, RiakSearchDoc, RiakConstants.ContentTypes.ApplicationJson));

            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("riak_search_bucket", "name").Search("Alyssa").Build()
            };

            var result = Client.Search(req);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.NumFound.ShouldEqual(1u);
            result.Value.Documents.Count.ShouldEqual(1);
            result.Value.Documents[0].Fields.Count.ShouldEqual(5);
            result.Value.Documents[0].Id.Value.ShouldEqual("a.hacker");
        }

        [Test]
        public void SearchingWithWildcardFluentQueryWorksCorrectly()
        {
            Client.Put(new RiakObject(Bucket, RiakSearchKey, RiakSearchDoc, RiakConstants.ContentTypes.ApplicationJson));
            Client.Put(new RiakObject(Bucket, RiakSearchKey2, RiakSearchDoc2, RiakConstants.ContentTypes.ApplicationJson));

            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("riak_search_bucket", "name").Search(Token.StartsWith("Al")).Build()
            };

            var result = Client.Search(req);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.NumFound.ShouldEqual(2u);
            result.Value.Documents.Count.ShouldEqual(2);
        }

        [Test]
        public void SearchingWithMoreComplexFluentQueryWorksCorrectly()
        {
            Client.Put(new RiakObject(Bucket, RiakSearchKey, RiakSearchDoc, RiakConstants.ContentTypes.ApplicationJson));
            Client.Put(new RiakObject(Bucket, RiakSearchKey2, RiakSearchDoc2, RiakConstants.ContentTypes.ApplicationJson));

            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("riak_search_bucket", "bio")
            };

            req.Query.Search("awesome")
                .And("an")
                .And("mathematician", t => t.Or("favorites_ablum", "Fame"));

            var result = Client.Search(req);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.NumFound.ShouldEqual(1u);
            result.Value.Documents.Count.ShouldEqual(1);
            result.Value.Documents[0].Fields.Count.ShouldEqual(5);
            result.Value.Documents[0].Id.Value.ShouldEqual("a.public");
        }

        [Test]
        public void SettingFieldListReturnsOnlyFieldsSpecified()
        {
            Client.Put(new RiakObject(Bucket, RiakSearchKey, RiakSearchDoc, RiakConstants.ContentTypes.ApplicationJson));
            Client.Put(new RiakObject(Bucket, RiakSearchKey2, RiakSearchDoc2, RiakConstants.ContentTypes.ApplicationJson));

            var req = new RiakSearchRequest
            {
                Query = new RiakFluentSearch("riak_search_bucket", "bio"),
                PreSort = PreSort.Key,
                DefaultOperation = DefaultOperation.Or,
                ReturnFields = new List<string>
                {
                    "bio", "favorites_album"
                }
            };

            req.Query.Search("awesome")
                .And("an")
                .And("mathematician", t => t.Or("favorites_ablum", "Fame"));

            var result = Client.Search(req);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.NumFound.ShouldEqual(1u);
            result.Value.Documents.Count.ShouldEqual(1);
            // "id" field is always returned
            result.Value.Documents[0].Fields.Count.ShouldEqual(3);
            result.Value.Documents[0].Id.ShouldNotBeNull();
        }
    }
}
