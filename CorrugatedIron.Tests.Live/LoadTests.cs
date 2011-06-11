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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.LoadTests
{
    [TestFixture]
    public class WhenUnderLoad : LiveRiakConnectionTestBase
    {
        // thread count is currently set to the same
        // value as the number of connections available
        // across the cluster. This is deliberate until
        // connection recovery/retry across nodes is
        // functioning and the load-balancing strategies
        // are in place.
        private const int ThreadCount = 60;
        private const int ActionCount = 20;

        public WhenUnderLoad()
            : base("riakLoadTestConfiguration")
        {
        }

        [Test]
        public void LotsOfConcurrentMapRedRequestsShouldWork()
        {
            const string dummyData = "{{ value: {0} }}";
            var keys = new List<string>();

            for (var i = 1; i < 11; i++)
            {
                var key = "key" + i;
                var newData = string.Format(dummyData, i);
                var doc = new RiakObject(MapReduceBucket, key, newData, Constants.ContentTypes.ApplicationJson);
                keys.Add(key);

                var result = Client.Put(doc, new RiakPutOptions { ReturnBody = true });
                result.IsSuccess.ShouldBeTrue();
            }

            var query = new RiakMapReduceQuery()
                .Inputs(new RiakPhaseInputs(keys.Select(k => new RiakBucketKeyInput(MapReduceBucket, k)).ToList()))
                .Map(m => m.Source(@"function(o){return[1];}"))
                .Reduce(r => r.Name(@"Riak.reduceSum").Keep(true));

            var batch = ThreadCount.Times(() => Tuple.Create(query, new Thread(DoMapRed), new List<RiakResult<RiakMapReduceResult>>())).ToArray();
            batch.ForEach(b => b.Item2.Start(b));
            batch.ForEach(b => b.Item2.Join());
            batch.ForEach(b => b.Item3.ForEach(r =>
                {
                    r.IsSuccess.ShouldBeTrue();
                    var json = JArray.Parse(r.Value.PhaseResults[1].Value.FromRiakString());
                    json[0].Value<int>().ShouldEqual(10);
                }));
        }

        private void DoMapRed(object input)
        {
            var inputs = (Tuple<RiakMapReduceQuery, Thread, List<RiakResult<RiakMapReduceResult>>>)input;
            var query = inputs.Item1;
            var results = inputs.Item3;
            var client = ClientGenerator();

            ActionCount.Times(() => results.Add(client.MapReduce(query)));
        }
    }
}
