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

using System.Collections.Generic;
using System.Linq;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live
{
    public class RiakObjectTestBase : LiveRiakConnectionTestBase
    {
        protected const string Jeremiah = "jeremiah";
        protected const string OJ = "oj";
        protected const string Brent = "brent";
        protected const string Rob = "rob";

        protected class Person
        {
            public string Name { get; set; }
            public string CurrentlyDrinking { get; set; }
        }

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
        }

        public void CreateLinkedObjects(string bucketName)
        {
            var oj = new RiakObject(bucketName, OJ, new Person() { Name = "oj" });
            var jeremiah = new RiakObject(bucketName, Jeremiah, new Person() { Name = "jeremiah" });
            var brent = new RiakObject(bucketName, Brent, new Person() { Name = "brent" });
            var rob = new RiakObject(bucketName, Rob, new Person() { Name = "rob" });

            oj.ContentType = RiakConstants.ContentTypes.ApplicationJson;
            jeremiah.ContentType = RiakConstants.ContentTypes.ApplicationJson;
            brent.ContentType = RiakConstants.ContentTypes.ApplicationJson;
            rob.ContentType = RiakConstants.ContentTypes.ApplicationJson;

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
    }

    [TestFixture]
    public class WhenSavingObjects : RiakObjectTestBase
    {
        private string Bucket;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            Bucket = System.Guid.NewGuid().ToString();

            CreateLinkedObjects(Bucket);

            var props = new RiakBucketProperties()
                .SetAllowMultiple(true);
            Client.SetBucketProperties(Bucket, props);
        }

        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(Bucket);
        }

        [Test]
        public void WriteableVectorClocksCanBeUsedToForceSiblings()
        {
            var oj = Client.Get(Bucket, OJ).Value;
            var vclock = oj.VectorClock;

            var ojTea = oj.GetObject<Person>();
            var ojCoffee = oj.GetObject<Person>();

            ojTea.CurrentlyDrinking = "tea";
            ojCoffee.CurrentlyDrinking = "coffee";

            oj.SetObject(ojTea);
            Client.Put(oj);

            oj.SetObject(ojCoffee);
            Client.Put(oj);

            var multiOj = Client.Get(Bucket, OJ).Value;

            oj.VectorClock.ShouldEqual(vclock);
            oj.VectorClock.ShouldNotEqual(multiOj.VectorClock);

            multiOj.Siblings.Count.ShouldBeGreaterThan(0);
        }
    }

    [TestFixture]
    public class WhenCreatingLinks : RiakObjectTestBase
    {
        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            CreateLinkedObjects(TestBucket);

            var props = new RiakBucketProperties()
                .SetAllowMultiple(false);

            Client.SetBucketProperties(TestBucket, props);
        }

        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(TestBucket);
        }

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

            var input = new RiakBucketKeyInput()
                .Add(TestBucket, Jeremiah);

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

            var mrLinks = listOfLinks.SelectMany(l => l).ToList();

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

            var result = Client.Put(jeremiah, new RiakPutOptions { ReturnBody = true });
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
