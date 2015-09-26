// <copyright file="WhenUsingFluentKeyFilters.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.Live.MapReduce
{
    using System.Linq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Extensions;
    using RiakClient.Models;
    using RiakClient.Models.MapReduce;

    [TestFixture, IntegrationTest]
    public class WhenUsingFluentKeyFilters : RiakMapReduceTestBase
    {
        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(Bucket);
        }

        [Test]
        public void EqualsFindsOneKey()
        {
            for (int i = 0; i < 10; i++)
            {
                Client.Put(new RiakObject(Bucket, string.Format("time_{0}", i), EmptyBody,
                    RiakConstants.ContentTypes.ApplicationJson));
            }

            var mr = new RiakMapReduceQuery();

#pragma warning disable 618
            mr.Inputs(Bucket)
                .Filter(f => f.Equal("time_8"))
                .MapJs(m => m.Source("function (o) { return [1]; }"))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));
#pragma warning restore 618

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();

            var mrResult = result.Value;
            mrResult.PhaseResults.ShouldNotBeNull();
            mrResult.PhaseResults.Count().ShouldEqual(2);

            mrResult.PhaseResults.ElementAt(0).Phase.ShouldEqual(0u);
            mrResult.PhaseResults.ElementAt(1).Phase.ShouldEqual(1u);

            mrResult.PhaseResults.ElementAt(0).Values.Count().ShouldEqual(0);
            mrResult.PhaseResults.ElementAt(1).Values.Count().ShouldNotEqual(0);


            var values = JsonConvert.DeserializeObject<int[]>(mrResult.PhaseResults
                                                                      .ElementAt(1)
                                                                      .Values
                                                                      .First()
                                                                      .FromRiakString());

            values[0].ShouldEqual(1);
        }

        [Test]
        public void StartsWithFindsAllKeys()
        {
            for (int i = 0; i < 10; i++)
            {
                Client.Put(new RiakObject(Bucket, string.Format("time_{0}", i), EmptyBody,
                    RiakConstants.ContentTypes.ApplicationJson));
            }

#pragma warning disable 618
            var mr = new RiakMapReduceQuery();
            mr.Inputs(Bucket)
                .Filter(f => f.StartsWith("time"))
                .MapJs(m => m.Source("function (o) { return [1]; }"))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));
#pragma warning restore 618

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();

            var mrResult = result.Value;
            mrResult.PhaseResults.ShouldNotBeNull();
            mrResult.PhaseResults.Count().ShouldEqual(2);

            mrResult.PhaseResults.ElementAt(0).Phase.ShouldEqual(0u);
            mrResult.PhaseResults.ElementAt(1).Phase.ShouldEqual(1u);

            mrResult.PhaseResults.ElementAt(0).Values.Count().ShouldEqual(0);
            mrResult.PhaseResults.ElementAt(1).Values.Count().ShouldNotEqual(0);


            var values = result.Value.PhaseResults.ElementAt(1).GetObjects<int[]>().First();
            values[0].ShouldEqual(10);
        }

        [Test]
        public void StartsWithAndBetweenReturnASubsetOfAllKeys()
        {
            for (var i = 0; i < 10; i++)
            {
                Client.Put(new RiakObject(Bucket, string.Format("time_{0}", i), EmptyBody,
                    RiakConstants.ContentTypes.ApplicationJson));
            }

#pragma warning disable 618
            var mr = new RiakMapReduceQuery();
            mr.Inputs(Bucket)
                .Filter(f => f.And(l => l.StartsWith("time"),
                                   r => r.Tokenize("_", 2)
                                            .StringToInt()
                                            .Between(3, 7, true)))
                .MapJs(m => m.Source("function (o) { return [1]; }").Keep(false))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));
#pragma warning restore 618

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();

            var mrResult = result.Value;
            mrResult.PhaseResults.ShouldNotBeNull();
            mrResult.PhaseResults.Count().ShouldEqual(2);

            var values = result.Value.PhaseResults.ElementAt(1).GetObjects<int[]>().First();
            values[0].ShouldEqual(5);
        }
    }
}
