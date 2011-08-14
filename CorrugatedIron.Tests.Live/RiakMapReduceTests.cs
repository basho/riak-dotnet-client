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

using System;
using System.Linq;
using CorrugatedIron.Comms;
using CorrugatedIron.Config;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using CorrugatedIron.Extensions;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live
{

    public class RiakMapReduceTests
    {
        protected IRiakCluster Cluster;
        protected IRiakClient Client;
        protected IRiakClusterConfiguration ClusterConfig;
        protected Func<IRiakClient> ClientGenerator;

        protected const string MrContentType = RiakConstants.ContentTypes.ApplicationJson;
        protected const string bucket = "fluent_key_bucket";
        protected const string emptyBody = "{}";

        public RiakMapReduceTests(string section = "riak1NodeConfiguration")
        {
            ClusterConfig = RiakClusterConfiguration.LoadFromConfig(section);
        }
    }

    [TestFixture]
    public class WhenUsingFluentKeyFilters : RiakMapReduceTests
    {

        [SetUp]
        public void SetUp()
        {
            Cluster = new RiakCluster(ClusterConfig, new RiakConnectionFactory());
            ClientGenerator = () => new RiakClient(Cluster);
            Client = ClientGenerator();
        }

        [Test]
        public void EqualsFindsOneKey()
        {
            for (int i = 0; i < 10; i++)
            {
                Client.Put(new RiakObject(bucket, String.Format("time_{0}", i), emptyBody, RiakConstants.ContentTypes.ApplicationJson));
            }

            var mr = new RiakMapReduceQuery {ContentType = MrContentType};

            mr.Inputs(bucket)
                .Filter(f => f.Equal("time_8"))
                .MapJs(m => m.Source("function (o) { return [1]; }"))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();

            var mrResult = result.Value;
            mrResult.PhaseResults.ShouldNotBeNull();
            mrResult.PhaseResults.Count().ShouldEqual(2);

            mrResult.PhaseResults.ElementAt(0).Phase.ShouldEqual(0u);
            mrResult.PhaseResults.ElementAt(1).Phase.ShouldEqual(1u);

            mrResult.PhaseResults.ElementAt(0).Value.ShouldBeNull();
            mrResult.PhaseResults.ElementAt(1).Value.ShouldNotBeNull();

            var values = result.Value.PhaseResults.ElementAt(1).GetObject<int[]>();
            values[0].ShouldEqual(1);
        }

        [Test]
        public void StartsWithFindsAllKeys()
        {
            for (int i = 0; i < 10; i++)
            {
                Client.Put(new RiakObject(bucket, String.Format("time_{0}", i), emptyBody, RiakConstants.ContentTypes.ApplicationJson));
            }

            var mr = new RiakMapReduceQuery { ContentType = MrContentType };

            mr.Inputs(bucket)
                .Filter(f => f.StartsWith("time"))
                .MapJs(m => m.Source("function (o) { return [1]; }"))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();

            var mrResult = result.Value;
            mrResult.PhaseResults.ShouldNotBeNull();
            mrResult.PhaseResults.Count().ShouldEqual(2);

            mrResult.PhaseResults.ElementAt(0).Phase.ShouldEqual(0u);
            mrResult.PhaseResults.ElementAt(1).Phase.ShouldEqual(1u);

            mrResult.PhaseResults.ElementAt(0).Value.ShouldBeNull();
            mrResult.PhaseResults.ElementAt(1).Value.ShouldNotBeNull();

            var values = result.Value.PhaseResults.ElementAt(1).GetObject<int[]>();
            values[0].ShouldEqual(10);
        }

        [Test]
        public void StartsWithAndBetweenReturnASubsetOfAllKeys()
        {
            for (int i = 0; i < 10; i++)
            {
                Client.Put(new RiakObject(bucket, String.Format("time_{0}", i), emptyBody, RiakConstants.ContentTypes.ApplicationJson));
            }

            var mr = new RiakMapReduceQuery { ContentType = MrContentType };

            mr.Inputs(bucket)
                .Filter(f => f.And(l => l.StartsWith("time"),
                                   r => r.Tokenize("_", 1)
                                            .Between(3, 7, true)))
                .MapJs(m => m.Source("function (o) { return [1]; }"))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();

            var mrResult = result.Value;
            mrResult.PhaseResults.ShouldNotBeNull();
            mrResult.PhaseResults.Count().ShouldEqual(2);

            mrResult.PhaseResults.ElementAt(0).Phase.ShouldEqual(0u);
            mrResult.PhaseResults.ElementAt(1).Phase.ShouldEqual(1u);

            mrResult.PhaseResults.ElementAt(0).Value.ShouldBeNull();
            mrResult.PhaseResults.ElementAt(1).Value.ShouldNotBeNull();

            var values = result.Value.PhaseResults.ElementAt(1).GetObject<int[]>();
            values[0].ShouldEqual(5);
        }
    }
}
