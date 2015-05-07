// <copyright file="DataTypes.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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

namespace RiakClientExamples.Dev.Using
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Util;

    /*
     * http://docs.basho.com/riak/latest/dev/using/data-types/
     */
    public sealed class DataTypes : ExampleBase
    {
        [Test]
        public void CountersIdAndFetch()
        {
            FetchCounter cmd = new FetchCounter.Builder()
                .WithBucketType("counters")
                .WithBucket("counters")
                .WithKey("<insert_key_here>")
                .Build();

            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);

            CounterResponse response = cmd.Response;
            Assert.AreEqual(0, response.Value);

            // NB: for cleanup on test teardown
            options = cmd.Options;
        }

        // Note: decrement example as well
        [Test]
        public void TrafficTicketsCounterFetchAndUpdate()
        {
            var fetchCounterOptions = new FetchCounterOptions("counters", "counters", "traffic_tickts");
            FetchCounter cmd = new FetchCounter(fetchCounterOptions);

            // NB: for cleanup on test teardown
            options = cmd.Options;

            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);

            CounterResponse response = cmd.Response;
            Assert.AreEqual(0, response.Value);

            UpdateCounter updateCmd = new UpdateCounter.Builder()
                .WithBucketType("counters")
                .WithBucket("counters")
                .WithKey("traffic_tickets")
                .WithIncrement(1)
                .Build();

            rslt = client.Execute(updateCmd);
            CheckResult(rslt);

            response = updateCmd.Response;
            Assert.AreEqual(1, response.Value);

            updateCmd = new UpdateCounter.Builder()
                .WithBucketType("counters")
                .WithBucket("counters")
                .WithKey("traffic_tickets")
                .WithIncrement(-1)
                .Build();

            rslt = client.Execute(updateCmd);
            CheckResult(rslt);

            response = updateCmd.Response;
            Assert.AreEqual(0, response.Value);
        }

        // Note: decrement example as well
        [Test]
        public void WhoopsIGotFiveTrafficTickets()
        {
            var fetchCounterOptions = new FetchCounterOptions("counters", "counters", "traffic_tickts");
            FetchCounter cmd = new FetchCounter(fetchCounterOptions);

            // NB: for cleanup
            options = fetchCounterOptions;

            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);

            CounterResponse response = cmd.Response;
            Assert.AreEqual(0, response.Value);

            var builder = new UpdateCounter.Builder();

            builder.WithBucketType("counters")
                .WithBucket("counters")
                .WithKey("traffic_tickets")
                .WithIncrement(5);

            UpdateCounter updateCmd = builder.Build();

            rslt = client.Execute(updateCmd);
            CheckResult(rslt);

            response = updateCmd.Response;
            Assert.AreEqual(5, response.Value);

            // Modify the builder's increment, then construct a new command
            builder.WithIncrement(-5);
            updateCmd = builder.Build();

            rslt = client.Execute(updateCmd);
            CheckResult(rslt);

            response = updateCmd.Response;
            Assert.AreEqual(0, response.Value);
        }

        [Test]
        public void InitialSetIsEmpty()
        {
            var builder = new FetchSet.Builder()
                .WithBucketType("sets")
                .WithBucket("travel")
                .WithKey("cities");

            // NB: builder.Options will only be set after Build() is called.
            FetchSet fetchSetCommand = builder.Build();

            FetchSetOptions options = new FetchSetOptions("sets", "travel", "cities");
            Assert.AreEqual(options, builder.Options);

            RiakResult rslt = client.Execute(fetchSetCommand);
            CheckResult(rslt);

            SetResponse response = fetchSetCommand.Response;
            Assert.IsTrue(EnumerableUtil.IsNullOrEmpty(response.Value));
        }

        [Test]
        public void CitiesSetAddRemoveAndView()
        {
            var adds = new[] { "Toronto", "Montreal" };

            var builder = new UpdateSet.Builder()
                .WithBucketType("sets")
                .WithBucket("travel")
                .WithKey("cities")
                .WithAdditions(adds);

            UpdateSet cmd = builder.Build();
            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);
            SetResponse response = cmd.Response;

            Assert.Contains("Toronto", response.AsStrings.ToArray());
            Assert.Contains("Montreal", response.AsStrings.ToArray());

            var removes = new[] { "Montreal" };
            adds = new[] { "Hamilton", "Ottawa" };

            Assert.True(EnumerableUtil.NotNullOrEmpty(response.Context));

            builder
                .WithAdditions(adds)
                .WithRemovals(removes)
                .WithContext(response.Context);

            cmd = builder.Build();

            rslt = client.Execute(cmd);
            CheckResult(rslt);
            response = cmd.Response;

            var responseStrings = response.AsStrings.ToArray();

            Assert.Contains("Toronto", responseStrings);
            Assert.Contains("Hamilton", responseStrings);
            Assert.Contains("Ottawa", responseStrings);

            foreach (var value in response.AsStrings)
            {
                Console.WriteLine("Cities Set Value: {0}", value);
            }

            Console.WriteLine("Cities Set Size: {0}", responseStrings.Length);

            bool includesVancouver = response.AsStrings.Any(v => v == "Vancouver");
            bool includesOttawa = response.AsStrings.Any(v => v == "Ottawa");

            Assert.False(includesVancouver);
            Assert.True(includesOttawa);
        }

        [Test]
        public void Maps()
        {
            var builder = new UpdateMap.Builder()
                .WithBucketType("maps")
                .WithBucket("customers")
                .WithKey("ahmed_info");

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Initial import of Ahmed's data and contact");

            var mapOperation = new UpdateMap.MapOperation();

            // Ahmed's first name
            mapOperation.SetRegister("first_name", "Ahmed");

            // Ahmed's phone number
            mapOperation.SetRegister("phone_number", "5551234567");

            builder.WithMapOperation(mapOperation);
            MapResponse response = SaveMap(builder);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Set Ahmed to NOT be an enterprise customer right now");

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.SetFlag("enterprise_customer", false);

            builder.WithMapOperation(mapOperation);
            response = SaveMap(builder);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("enterprise_customer flag value");

            Map ahmedMap = response.Value;
            Console.WriteLine("Ahmed enterprise_customer: {0}", ahmedMap.Flags["enterprise_customer"]);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Add page visits counter for Ahmed");

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.IncrementCounter("page_visits", 1);

            builder.WithMapOperation(mapOperation);
            response = SaveMap(builder);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Add Ahmed's interests set");

            var interestsAdds = new[] { "robots", "opera", "motorcycles" };

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.AddToSet("interests", interestsAdds);

            builder.WithMapOperation(mapOperation);
            response = SaveMap(builder);

            ahmedMap = response.Value;

            Assert.True(ahmedMap.Sets.GetValue("interests").Contains("robots"));
            Assert.True(ahmedMap.Sets.GetValue("interests").Contains("opera"));
            Assert.True(ahmedMap.Sets.GetValue("interests").Contains("motorcycles"));

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Updating Ahmed's interests");

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.AddToSet("interests", "indie pop");
            mapOperation.RemoveFromSet("interests", "opera");

            builder
                .WithMapOperation(mapOperation)
                .WithContext(response.Context);
            response = SaveMap(builder);

            ahmedMap = response.Value;

            Assert.False(ahmedMap.Sets.GetValue("interests").Contains("opera"));
            Assert.True(ahmedMap.Sets.GetValue("interests").Contains("indie pop"));

            Assert.True(ahmedMap.Sets.GetValue("interests").Contains("robots"));
            Assert.True(ahmedMap.Sets.GetValue("interests").Contains("motorcycles"));

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Adding Annika map");

            mapOperation = new UpdateMap.MapOperation();

            UpdateMap.MapOperation annikaInfoOperation = mapOperation.Map("annika_info");
            annikaInfoOperation.SetRegister("first_name", "Annika");
            annikaInfoOperation.SetRegister("last_name", "Weiss");
            annikaInfoOperation.SetRegister("phone_number", "5559876543");

            builder.WithMapOperation(mapOperation);
            response = SaveMap(builder);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Annika's first name");

            ahmedMap = response.Value;
            Console.WriteLine(ahmedMap.Maps["annika_info"].Registers.GetValue("first_name"));

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Removing Annika's first name");

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.Map("annika_info").RemoveRegister("first_name");

            builder
                .WithMapOperation(mapOperation)
                .WithContext(response.Context);
            response = SaveMap(builder);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Storing Annika's plan flags");

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.Map("annika_info")
                .SetFlag("enterprise_plan", false)
                .SetFlag("family_plan", false)
                .SetFlag("free_plan", true);

            builder.WithMapOperation(mapOperation);
            response = SaveMap(builder);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Is Annika on enterprise plan?");

            ahmedMap = response.Value;
            Console.WriteLine(ahmedMap.Maps["annika_info"].Flags["enterprise_plan"]);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Adding Annika's widget_purchases counter");

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.Map("annika_info").IncrementCounter("widget_purchases", 1);

            builder.WithMapOperation(mapOperation);
            response = SaveMap(builder);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Adding Annika's interests in a set");

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.Map("annika_info").AddToSet("interests", "tango dancing");

            builder.WithMapOperation(mapOperation);
            response = SaveMap(builder);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Remove \"tango dancing\" from Annika's interests");

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.Map("annika_info").RemoveFromSet("interests", "tango dancing");

            builder
                .WithMapOperation(mapOperation)
                .WithContext(response.Context);
            response = SaveMap(builder);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Add specific purchase info to Annika's data");

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.Map("annika_info").Map("purchase")
                .SetFlag("first_purchase", true)
                .SetRegister("amount", "1271")
                .AddToSet("items", "large widget");

            builder.WithMapOperation(mapOperation);
            response = SaveMap(builder);

            Console.WriteLine("Context: {0}", Convert.ToBase64String(response.Context));
        }

        private MapResponse SaveMap(UpdateMap.Builder builder)
        {
            UpdateMap cmd = builder.Build();
            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);

            MapResponse response = cmd.Response;
            PrintMap(response.Value);
            return response;
        }

        private static void PrintMap(Map map)
        {
            var converter = new ByteArrayAsStringConverter();
            Console.WriteLine("Map: {0}", JsonConvert.SerializeObject(map, converter));
        }
    }
}