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
        private const int ThreadCount = 70;
        private const int ActionCount = 30;
        //private const int ThreadCount = 1;
        //private const int ActionCount = 1;

        public WhenUnderLoad()
            : base("riakLoadTestConfiguration")
        {
        }

        [Test]
        public void LotsOfConcurrentMapRedRequestsShouldWork()
        {
            var keys = new List<string>();

            for (var i = 1; i < 11; i++)
            {
                var key = "key" + i;
                var doc = new RiakObject(MapReduceBucket, key, new { value = i });
                keys.Add(key);

                var result = Client.Put(doc, new RiakPutOptions { ReturnBody = true });
                result.IsSuccess.ShouldBeTrue();
            }

            var query = new RiakMapReduceQuery()
                .Inputs(keys.Select(k => new RiakBucketKeyInput(MapReduceBucket, k)).ToList())
                .MapJs(m => m.Source(@"function(o){return[1];}"))
                .ReduceJs(r => r.Name(@"Riak.reduceSum").Keep(true));
            query.Compile();

            var batch = ThreadCount.Times(() => Tuple.Create(query, new Thread(DoMapRed), new List<RiakResult<RiakMapReduceResult>>())).ToArray();
            batch.ForEach(b => b.Item2.Start(b));

            int failures = 0;
            foreach(var b in batch)
            {
                b.Item2.Join();
                b.Item3.ForEach(r =>
                                    {
                                        if (r.IsSuccess)
                                        {
                                            var json = JArray.Parse(r.Value.PhaseResults[1].Value.FromRiakString());
                                            json[0].Value<int>().ShouldEqual(10);
                                        }
                                        else
                                        {
                                            // the only acceptable result is that it ran out of retries when
                                            // talking to the cluster (trying to get a connection)
                                            r.ResultCode.ShouldEqual(ResultCode.NoRetries);
                                            ++failures;
                                        }
                                    });
            }

            Console.WriteLine("Total of {0} out of {1} failed to execute due to connection contention", failures, ThreadCount * ActionCount);
        }

        private void DoMapRed(object input)
        {
            Thread.CurrentThread.Name = "TestThread - " + Guid.NewGuid();
            var inputs = (Tuple<RiakMapReduceQuery, Thread, List<RiakResult<RiakMapReduceResult>>>)input;
            var query = inputs.Item1;
            var results = inputs.Item3;
            var client = ClientGenerator();

            ActionCount.Times(() => results.Add(client.MapReduce(query)));
        }

        [Test]
        public void LotsOfConcurrentStreamingMapRedRequestsShouldWork()
        {
            var keys = new List<string>();

            for (var i = 1; i < 11; i++)
            {
                var key = "key" + i;
                var doc = new RiakObject(MapReduceBucket, key, new { value = i });
                keys.Add(key);

                var result = Client.Put(doc, new RiakPutOptions { ReturnBody = true });
                result.IsSuccess.ShouldBeTrue();
            }

            var query = new RiakMapReduceQuery()
                .Inputs(keys.Select(k => new RiakBucketKeyInput(MapReduceBucket, k)).ToList())
                .MapJs(m => m.Source(@"function(o){return[1];}"))
                .ReduceJs(r => r.Name(@"Riak.reduceSum").Keep(true));
            query.Compile();

            var batch = ThreadCount.Times(() => Tuple.Create(query, new Thread(DoStreamingMapRed), new List<RiakMapReduceResultPhase>())).ToArray();
            batch.ForEach(b => b.Item2.Start(b));

            int failures = 0;
            foreach(var b in batch)
            {
                b.Item2.Join();

                if (b.Item3.Count > 0)
                {
                    var finalResult = b.Item3.OrderByDescending(r => r.Phase).First();
                    var json = JArray.Parse(finalResult.Value.FromRiakString());
                    json[0].Value<int>().ShouldEqual(10);
                }
                else
                {
                    ++failures;
                }
            }
            Console.WriteLine("Total of {0} out of {1} failed to execute due to connection contention", failures, ThreadCount * ActionCount);
        }

        private void DoStreamingMapRed(object input)
        {
            Thread.CurrentThread.Name = "TestThread - " + Guid.NewGuid();
            var inputs = (Tuple<RiakMapReduceQuery, Thread, List<RiakMapReduceResultPhase>>)input;
            var query = inputs.Item1;
            var results = inputs.Item3;
            var client = ClientGenerator();

            ActionCount.Times(() =>
                {
                    var streamedResults = client.StreamMapReduce(query);
                    if (streamedResults.IsSuccess)
                    {
                        streamedResults.Value.PhaseResults.ForEach(results.Add);
                    }
                });
        }
    }
}
