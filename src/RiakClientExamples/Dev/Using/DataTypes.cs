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
            UpdateMap cmd = builder.Build();
            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);

            MapResponse response = cmd.Response;
            PrintMap(response.Value);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Set Ahmed to NOT be an enterprise customer right now");

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.SetFlag("enterprise_customer", false);

            builder.WithMapOperation(mapOperation);
            cmd = builder.Build();
            rslt = client.Execute(cmd);
            CheckResult(rslt);

            response = cmd.Response;
            PrintMap(response.Value);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("enterprise_customer flag value");

            Map ahmedMap = response.Value;
            Console.WriteLine("Ahmed enterprise_customer: {0}", ahmedMap.Flags["enterprise_customer"]);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Add page visits counter for Ahmed");

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.IncrementCounter("page_visits", 1);

            builder.WithMapOperation(mapOperation);
            cmd = builder.Build();
            rslt = client.Execute(cmd);
            CheckResult(rslt);

            response = cmd.Response;
            PrintMap(response.Value);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Add Ahmed's interests set");

            var interestsAdds = new[] { "robots", "opera", "motorcycles" };

            mapOperation = new UpdateMap.MapOperation();
            mapOperation.AddToSet("interests", interestsAdds);

            builder.WithMapOperation(mapOperation);
            cmd = builder.Build();
            rslt = client.Execute(cmd);
            CheckResult(rslt);

            response = cmd.Response;
            PrintMap(response.Value);

            /*
            // Maps within Maps (Nested Maps)
            var annikaMapUpdates = new List<MapUpdate>
            {
                new MapUpdate
                {
                    register_op = Serializer("Annika"),
                    field = new MapField
                    {
                        name = Serializer(firstNameRegister),
                        type = MapField.MapFieldType.REGISTER
                    },
                },
                new MapUpdate
                {
                    register_op = Serializer("Weiss"),
                    field = new MapField
                    {
                        name = Serializer(lastNameRegister),
                        type = MapField.MapFieldType.REGISTER
                    },
                },
                new MapUpdate
                {
                    register_op = Serializer("5559876543"),
                    field = new MapField
                    {
                        name = Serializer(phoneNumberRegister),
                        type = MapField.MapFieldType.REGISTER
                    },
                }
            };
            var annikaInfoUpdateMapOp = new MapOp();
            annikaInfoUpdateMapOp.updates.AddRange(annikaMapUpdates);
            var annikaInfoUpdate = new MapUpdate
            {
                map_op = annikaInfoUpdateMapOp,
                field = new MapField
                {
                    name = Serializer(annikaInfoMap),
                    type = MapField.MapFieldType.MAP
                }
            };

            var updates = new List<MapUpdate> {
                firstNameRegisterMapUpdate,
                phoneNumberRegisterMapUpdate,
                enterpriseCustomerFlagUpdate,
                pageVisitsCounterUpdate,
                interestsSetUpdate,
                annikaInfoUpdate
            };
            rslt = client.DtUpdateMap(id, Serializer, rslt.Context, null, updates);
            CheckResult(rslt);
            PrintMapValues(rslt.Values);

            // Update Ahmed's set of interests to remove "opera" and add "indie pop"
            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Updating Ahmed's interests");
            var interestsRemoves = new[] { "opera" };
            interestsAdds = new[] { "indie pop" };
            setOperation = new SetOp();
            setOperation.adds.AddRange(interestsAdds.Select(i => Serializer(i)));
            setOperation.removes.AddRange(interestsRemoves.Select(i => Serializer(i)));
            interestsSetUpdate = new MapUpdate
            {
                set_op = setOperation,
                field = new MapField
                {
                    name = Serializer(interestsSet),
                    type = MapField.MapFieldType.SET
                }
            };

            updates = new List<MapUpdate> {
                interestsSetUpdate
            };
            rslt = client.DtUpdateMap(id, Serializer, rslt.Context, null, updates);
            CheckResult(rslt);
            PrintMapValues(rslt.Values);

            // Registers (and other map members) can also be removed
            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Removing Annika's first name");
            var annikaMapRemoves = new List<MapField>
            {
                new MapField
                {
                    name = Serializer(firstNameRegister),
                    type = MapField.MapFieldType.REGISTER
                },
            };
            annikaInfoUpdateMapOp = new MapOp();
            annikaInfoUpdateMapOp.removes.AddRange(annikaMapRemoves);
            annikaInfoUpdate = new MapUpdate
            {
                map_op = annikaInfoUpdateMapOp,
                field = new MapField
                {
                    name = Serializer(annikaInfoMap),
                    type = MapField.MapFieldType.MAP
                }
            };

            updates = new List<MapUpdate> {
                annikaInfoUpdate
            };
            rslt = client.DtUpdateMap(id, Serializer, rslt.Context, null, updates);
            CheckResult(rslt);
            PrintMapValues(rslt.Values);


            // Store whether Annika is subscribed to a variety of plans
            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Storing Annika's plan flags");
            const string enterprisePlanFlag = "enterprise_plan";
            const string familyPlanFlag = "family_plan";
            const string freePlanFlag = "free_plan";
            annikaMapUpdates = new List<MapUpdate>
            {
                new MapUpdate
                {
                    flag_op = MapUpdate.FlagOp.DISABLE,
                    field = new MapField
                    {
                        name = Serializer(enterprisePlanFlag),
                        type = MapField.MapFieldType.FLAG
                    },
                },
                new MapUpdate
                {
                    flag_op = MapUpdate.FlagOp.DISABLE,
                    field = new MapField
                    {
                        name = Serializer(familyPlanFlag),
                        type = MapField.MapFieldType.FLAG
                    },
                },
                new MapUpdate
                {
                    flag_op = MapUpdate.FlagOp.DISABLE,
                    field = new MapField
                    {
                        name = Serializer(freePlanFlag),
                        type = MapField.MapFieldType.FLAG
                    },
                } 
            };
            annikaInfoUpdateMapOp = new MapOp();
            annikaInfoUpdateMapOp.updates.AddRange(annikaMapUpdates);
            annikaInfoUpdate = new MapUpdate
            {
                map_op = annikaInfoUpdateMapOp,
                field = new MapField
                {
                    name = Serializer(annikaInfoMap),
                    type = MapField.MapFieldType.MAP
                }
            };

            updates = new List<MapUpdate> {
                annikaInfoUpdate
            };
            rslt = client.DtUpdateMap(id, Serializer, rslt.Context, null, updates);
            CheckResult(rslt);
            PrintMapValues(rslt.Values);

            // Store Annika's purchases in a counter
            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Adding Annika's widget_purchases counter");
            const string widgetPurchasesCounter = "widget_purchases";
            annikaMapUpdates = new List<MapUpdate>
            {
                new MapUpdate
                {
                    counter_op = new CounterOp { increment = 1 },
                    field = new MapField
                    {
                        name = Serializer(widgetPurchasesCounter),
                        type = MapField.MapFieldType.COUNTER
                    },
                }
            };
            annikaInfoUpdateMapOp = new MapOp();
            annikaInfoUpdateMapOp.updates.AddRange(annikaMapUpdates);
            annikaInfoUpdate = new MapUpdate
            {
                map_op = annikaInfoUpdateMapOp,
                field = new MapField
                {
                    name = Serializer(annikaInfoMap),
                    type = MapField.MapFieldType.MAP
                }
            };

            updates = new List<MapUpdate> {
                annikaInfoUpdate
            };
            rslt = client.DtUpdateMap(id, Serializer, rslt.Context, null, updates);
            CheckResult(rslt);
            PrintMapValues(rslt.Values);

            // Store Annika's interests in a set
            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Adding Annika's interests in a set");
            const string annikaInterestsSet = "interests";
            var annikaInterestsSetOp = new SetOp();
            annikaInterestsSetOp.adds.Add(Serializer("tango dancing"));
            annikaMapUpdates = new List<MapUpdate>
            {
                new MapUpdate
                {
                    set_op = annikaInterestsSetOp,
                    field = new MapField
                    {
                        name = Serializer(annikaInterestsSet),
                        type = MapField.MapFieldType.SET
                    },
                }
            };
            annikaInfoUpdateMapOp = new MapOp();
            annikaInfoUpdateMapOp.updates.AddRange(annikaMapUpdates);
            annikaInfoUpdate = new MapUpdate
            {
                map_op = annikaInfoUpdateMapOp,
                field = new MapField
                {
                    name = Serializer(annikaInfoMap),
                    type = MapField.MapFieldType.MAP
                }
            };

            updates = new List<MapUpdate> {
                annikaInfoUpdate
            };
            rslt = client.DtUpdateMap(id, Serializer, rslt.Context, null, updates);
            CheckResult(rslt);
            PrintMapValues(rslt.Values);

            // Remove "tango dancing" from Annika's interests
            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Remove \"tango dancing\" from Annika's interests");
            annikaInterestsSetOp = new SetOp();
            annikaInterestsSetOp.removes.Add(Serializer("tango dancing"));
            annikaMapUpdates = new List<MapUpdate>
            {
                new MapUpdate
                {
                    set_op = annikaInterestsSetOp,
                    field = new MapField
                    {
                        name = Serializer(annikaInterestsSet),
                        type = MapField.MapFieldType.SET
                    },
                }
            };
            annikaInfoUpdateMapOp = new MapOp();
            annikaInfoUpdateMapOp.updates.AddRange(annikaMapUpdates);
            annikaInfoUpdate = new MapUpdate
            {
                map_op = annikaInfoUpdateMapOp,
                field = new MapField
                {
                    name = Serializer(annikaInfoMap),
                    type = MapField.MapFieldType.MAP
                }
            };

            updates = new List<MapUpdate> {
                annikaInfoUpdate
            };
            rslt = client.DtUpdateMap(id, Serializer, rslt.Context, null, updates);
            CheckResult(rslt);
            PrintMapValues(rslt.Values);

            // Add purchase information to Annika's data
            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Add specific purchase info to Annika's data");

            const string annikaPurchaseMap = "purchase";
            const string annikaFirstPurchaseFlag = "first_purchase";
            const string annikaPurchaseAmountRegister = "amount";
            const string annikaPurchaseItemsSet = "items";

            var annikaItemsSetOp = new SetOp();
            annikaItemsSetOp.adds.Add(Serializer("large widget"));
            annikaMapUpdates = new List<MapUpdate>
            {
                new MapUpdate
                {
                    flag_op = MapUpdate.FlagOp.ENABLE,
                    field = new MapField
                    {
                        name = Serializer(annikaFirstPurchaseFlag),
                        type = MapField.MapFieldType.FLAG
                    }
                },
                new MapUpdate
                {
                    register_op = Serializer("1271"),
                    field = new MapField
                    {
                        name = Serializer(annikaPurchaseAmountRegister),
                        type = MapField.MapFieldType.REGISTER
                    }
                },
                new MapUpdate
                {
                    set_op = annikaItemsSetOp,
                    field = new MapField
                    {
                        name = Serializer(annikaPurchaseItemsSet),
                        type = MapField.MapFieldType.SET
                    },
                }
            };

            var annikaPurchaseMapOp = new MapOp();
            annikaPurchaseMapOp.updates.AddRange(annikaMapUpdates);
            var annikaPurchaseMapUpdate = new MapUpdate
            {
                map_op = annikaPurchaseMapOp,
                field = new MapField
                {
                    name = Serializer(annikaPurchaseMap),
                    type = MapField.MapFieldType.MAP
                }
            };

            annikaInfoUpdateMapOp = new MapOp();
            annikaInfoUpdateMapOp.updates.Add(annikaPurchaseMapUpdate);
            annikaInfoUpdate = new MapUpdate
            {
                map_op = annikaInfoUpdateMapOp,
                field = new MapField
                {
                    name = Serializer(annikaInfoMap),
                    type = MapField.MapFieldType.MAP
                }
            };

            updates = new List<MapUpdate> {
                annikaInfoUpdate
            };
            rslt = client.DtUpdateMap(id, Serializer, rslt.Context, null, updates);
            CheckResult(rslt);
            PrintMapValues(rslt.Values);

            rslt = client.DtFetchMap(id);
            CheckResult(rslt);
            Console.WriteLine("Context: {0}", Convert.ToBase64String(rslt.Context));
             */
        }

        private static void PrintMap(Map map)
        {
            var converter = new ByteArrayAsStringConverter();
            string msg = string.Format("Map: {0}", JsonConvert.SerializeObject(map, converter));
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
        }
    }
}