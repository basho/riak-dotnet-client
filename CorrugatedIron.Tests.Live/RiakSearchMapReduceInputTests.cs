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

using CorrugatedIron.Comms;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using NUnit.Framework;
using System.Linq;

namespace CorrugatedIron.Tests.Live
{
    [TestFixture]
    public class RiakSearchMapReduceInputTests : RiakMapReduceTests
    {
        // N.B. You need to install the search hooks on the riak_search_bucket first via `bin/search-cmd install riak_search_bucket`
        private const string RiakSearchKey = "a.hacker";
        private const string RiakSearchKey2 = "a.public";
        private const string RiakSearchDoc = "{\"name\":\"Alyssa P. Hacker\", \"bio\":\"I'm an engineer, making awesome things.\", \"favorites\":{\"book\":\"The Moon is a Harsh Mistress\",\"album\":\"Magical Mystery Tour\", }}";
        private const string RiakSearchDoc2 = "{\"name\":\"Alan Q. Public\", \"bio\":\"I'm an exciting mathematician\", \"favorites\":{\"book\":\"Prelude to Mathematics\",\"album\":\"The Fame Monster\"}}";

        public RiakSearchMapReduceInputTests ()
        {
            Bucket = "riak_search_bucket";
        }
        
        [SetUp]
        public void SetUp() 
        {
            Cluster = new RiakCluster(ClusterConfig, new RiakConnectionFactory());
            Client = Cluster.CreateClient();
            
            var props = Client.GetBucketProperties(Bucket).Value;
            props.SetSearch(true);
            Client.SetBucketProperties(Bucket, props);
        }
        
        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(Bucket);
        }
        
        [Test]
        public void SearchingByNameReturnsTheObjectId()
        {
            Client.Put(new RiakObject(Bucket, RiakSearchKey, RiakSearchDoc, RiakConstants.ContentTypes.ApplicationJson));
            Client.Put(new RiakObject(Bucket, RiakSearchKey2, RiakSearchDoc2, RiakConstants.ContentTypes.ApplicationJson));

            var mr = new RiakMapReduceQuery()
                .Inputs(new RiakBucketSearchInput(Bucket, "name:A1*"));

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            
            var mrResult = result.Value;
            mrResult.PhaseResults.Count().ShouldEqual(1);
            
            mrResult.PhaseResults.ElementAt(0).Values.ShouldNotBeNull();
            // TODO Add data introspection to test - need to verify the results, after all.
        }
        
        [Test]
        public void SearchingViaFluentSearchObjectWorks()
        {
            Client.Put(new RiakObject(Bucket, RiakSearchKey, RiakSearchDoc, RiakConstants.ContentTypes.ApplicationJson));
            Client.Put(new RiakObject(Bucket, RiakSearchKey2, RiakSearchDoc2, RiakConstants.ContentTypes.ApplicationJson));

            var search = new RiakFluentSearch(Bucket, "name").Search(Token.StartsWith("A1")).Build();
            var mr = new RiakMapReduceQuery()
                .Inputs(new RiakBucketSearchInput(search));

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            
            var mrResult = result.Value;
            mrResult.PhaseResults.Count().ShouldEqual(1);
            
            mrResult.PhaseResults.ElementAt(0).Values.ShouldNotBeNull();
            // TODO Add data introspection to test - need to verify the results, after all.
        }
    }
}
