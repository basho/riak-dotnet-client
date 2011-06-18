// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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

using System.Collections.Generic;
using CorrugatedIron.KeyFilters;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live
{
    [TestFixture]
    public class WhenCreatingLinks : LiveRiakConnectionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();

            var oj = new RiakObject(TestBucket, OJ, @"{""name"":""oj""}", Constants.ContentTypes.ApplicationJson);
            var jeremiah = new RiakObject(TestBucket, Jeremiah, @"{""name"":""jeremiah""}",
                                          Constants.ContentTypes.ApplicationJson);
            var brent = new RiakObject(TestBucket, Brent, @"{""name"":""brent""}",
                                       Constants.ContentTypes.ApplicationJson);
            var rob = new RiakObject(TestBucket, Rob, @"{""name"":""rob""}", Constants.ContentTypes.ApplicationJson);

            oj.Links.Add(jeremiah.ToRiakLink("friends"));
            oj.Links.Add(jeremiah.ToRiakLink("coworkers"));
            jeremiah.Links.Add(brent.ToRiakLink("friends"));
            jeremiah.Links.Add(brent.ToRiakLink("coworkers"));
            jeremiah.Links.Add(rob.ToRiakLink("ozzies"));
            jeremiah.Links.Add(oj.ToRiakLink("ozzies"));

            Client.Put(oj);
            Client.Put(jeremiah);
            Client.Put(brent);
            Client.Put(rob);
        }

        #endregion

        private const string Jeremiah = "jeremiah";
        private const string OJ = "oj";
        private const string Brent = "brent";
        private const string Rob = "rob";

        [Test]
        public void LinkMetadataIsRetrievedWithAnObject()
        {
            var jeremiah = Client.Get(TestBucket, Jeremiah).Value;

            jeremiah.Links.Count.IsAtLeast(4);
        }

        [Test]
        public void RiakObjectLinksAreTheSameAsLinksRetrievedViaMapReduce()
        {
            var jeremiah = Client.Get(TestBucket, Jeremiah).Value;
            var jLinks = jeremiah.Links;

            var query = new RiakMapReduceQuery()
                .Inputs(new RiakPhaseInputs(new List<RiakBucketKeyInput>
                                                {
                                                    new RiakBucketKeyInput(TestBucket, Jeremiah )
                                                }))
                .Link(l => l.Empty());

            var mrResult = Client.MapReduce(query);
            mrResult.IsSuccess.ShouldBeTrue();

            var mrLinkString = mrResult.Value.PhaseResults[0].Value.FromRiakString();
            var mrLinks = RiakLink.ParseArrayFromJsonString(mrLinkString);

            mrLinks.Count.ShouldEqual(jLinks.Count);
            foreach (var link in jLinks)
            {
                mrLinks.ShouldContain(link);
            }
        }

        [Test]
        public void LinksAreRetrievedWithAMapReducePhase()
        {
            var query = new RiakMapReduceQuery()
                .Inputs(new RiakPhaseInputs(TestBucket))
                .Filter(new Matches<string>(Jeremiah))
                .Link(l => l.Tag("friends")
                               .Bucket(TestBucket))
                .Reduce(r => r.Langauge(RiakPhase.PhaseLanguage.Erlang)
                                 .Module("riak_kv_mapreduce")
                                 .Function("reduce_set_union")
                                 .Keep(true)
                );

            RiakResult<RiakMapReduceResult> result = Client.MapReduce(query);
            result.IsSuccess.ShouldBeTrue();

            var mrResult = result.Value;
            mrResult.PhaseResults.ShouldNotBeNull();
            mrResult.PhaseResults.Count.ShouldEqual(2);
        }
    }
}