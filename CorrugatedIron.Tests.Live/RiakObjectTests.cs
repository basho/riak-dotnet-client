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

using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;


namespace CorrugatedIron.Tests.Live
{
    [TestFixture]
    public class WhenCreatingLinks : LiveRiakConnectionTestBase
    {
        [SetUp]
        public new void SetUp()
        {
            base.SetUp();

            var oj = new RiakObject(TestBucket, OJ, new { name = "oj" });
            var jeremiah = new RiakObject(TestBucket, Jeremiah, new { name = "jeremiah" });
            var brent = new RiakObject(TestBucket, Brent, new { name = "brent" });
            var rob = new RiakObject(TestBucket, Rob, new { name = "rob" });

            oj.LinkTo(jeremiah, "friends");
            oj.LinkTo(jeremiah, "coworkers");
            
            jeremiah.LinkTo(oj, "friends");
            jeremiah.LinkTo(oj, "coworkers");
            jeremiah.LinkTo(oj, "ozzies");
            jeremiah.LinkTo(brent, "friends");
            jeremiah.LinkTo(brent, "coworkers");
            jeremiah.LinkTo(rob, "ozzies");

            brent.LinkTo(jeremiah, "coworkers");
            brent.LinkTo(jeremiah, "friends");

            Client.Put(new[] { oj, jeremiah, brent, rob });
        }

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

            var input = new RiakBucketKeyInput();
            input.AddBucketKey(TestBucket, Jeremiah);

            var query = new RiakMapReduceQuery()
                .Inputs(input)
                .Link(l => l.AllLinks().Keep(true));

            var mrResult = Client.MapReduce(query);
            mrResult.IsSuccess.ShouldBeTrue();
            
            // TODO Is *this* chunk of code acceptable?
            // This should probably be taken care of in the RiakClient.WalkLinks
            var listOfLinks = mrResult.Value.PhaseResults.OrderBy(pr => pr.Phase)
                                  .ElementAt(0).Values
                                  .Select(v => RiakLink.ParseArrayFromJsonString(v.FromRiakString()));
            
            var mrLinks = from link in listOfLinks
                          from item in link
                          select item;


            mrLinks.Count().ShouldEqual(jLinks.Count);
            foreach (var link in jLinks)
            {
                mrLinks.ShouldContain(link);
            }
        }

        [Test]
        public void LinksAreRetrievedWithAMapReducePhase()
        {
            var query = new RiakMapReduceQuery()
                .Inputs(TestBucket)
                //.Filter(new Matches<string>(Jeremiah))
                .Filter(f => f.Matches(Jeremiah))
                .Link(l => l.Tag("friends").Bucket(TestBucket).Keep(false))
                .ReduceErlang(r => r.ModFun("riak_kv_mapreduce", "reduce_set_union").Keep(true));

            var result = Client.MapReduce(query);
            result.IsSuccess.ShouldBeTrue();

            var mrResult = result.Value;
            mrResult.PhaseResults.ShouldNotBeNull();
            mrResult.PhaseResults.Count().ShouldEqual(2);
        }

        [Test]
        public void LinksAreRemovedSuccessfullyInMemory()
        {
            var jeremiah = Client.Get(TestBucket, Jeremiah).Value;
            var linkToRemove = new RiakLink(TestBucket, OJ, "ozzies");

            jeremiah.RemoveLink(linkToRemove);

            var ojLinks = new List<RiakLink>
                              {
                                  new RiakLink(TestBucket, OJ, "friends"),
                                  new RiakLink(TestBucket, OJ, "coworkers")
                              };

            jeremiah.Links.ShouldNotContain(linkToRemove);

            ojLinks.ForEach(l => jeremiah.Links.ShouldContain(l));
        }

        [Test]
        public void LinksAreRemovedAfterSaving()
        {
            var jeremiah = Client.Get(TestBucket, Jeremiah).Value;
            var linkToRemove = new RiakLink(TestBucket, OJ, "ozzies");

            jeremiah.RemoveLink(linkToRemove);

            var result = Client.Put(jeremiah, new RiakPutOptions{ ReturnBody = true });
            result.IsSuccess.ShouldBeTrue();

            jeremiah = result.Value;

            var ojLinks = new List<RiakLink>
                              {
                                  new RiakLink(TestBucket, OJ, "friends"),
                                  new RiakLink(TestBucket, OJ, "coworkers")
                              };

            jeremiah.Links.ShouldNotContain(linkToRemove);

            ojLinks.ForEach(l => jeremiah.Links.ShouldContain(l));
        }

        [Test]
        public void LinkWalkingSuccessfullyRetrievesNLevels()
        {
            var oj = Client.Get(TestBucket, OJ).Value;
            var linkPhases = new List<RiakLink>
                                 {
                                     RiakLink.AllLinks,
                                     RiakLink.AllLinks
                                 };


            var linkPeople = Client.WalkLinks(oj, linkPhases);
            linkPeople.IsSuccess.ShouldBeTrue();
            linkPeople.Value.Count.ShouldEqual(6);
        }
    }
}
