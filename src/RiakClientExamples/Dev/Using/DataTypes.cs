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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Messages;
    using RiakClient.Models;
    using RiakClient.Models.RiakDt;

    /*
     * http://docs.basho.com/riak/latest/dev/using/data-types/
     */
    public sealed class DataTypes : ExampleBase
    {
        [Test]
        public void CountersIdAndFetch()
        {
            id = new RiakObjectId("counters", "counters", "<insert_key_here>");
            Assert.AreEqual("counters", id.BucketType);
            Assert.AreEqual("counters", id.Bucket);
            Assert.AreEqual("<insert_key_here>", id.Key);

            var rslt = client.DtFetchCounter(id);
            CheckResult(rslt);
        }

        // Note: decrement example as well
        [Test]
        public void TrafficTicketsCounterFetchAndUpdate()
        {
            id = new RiakObjectId("counters", "counters", "traffic_tickets");
            var rslt = client.DtFetchCounter(id);
            CheckResult(rslt);

            rslt = client.DtUpdateCounter(id, 1);
            CheckResult(rslt);
            Assert.AreEqual(1, rslt.Value);

            rslt = client.DtUpdateCounter(id, -1);
            CheckResult(rslt);
            Assert.AreEqual(0, rslt.Value);
        }

        // Note: decrement example as well
        [Test]
        public void WhoopsIGotFiveTrafficTickets()
        {
            id = new RiakObjectId("counters", "counters", "traffic_tickets");
            var rslt = client.DtFetchCounter(id);
            CheckResult(rslt);

            rslt = client.DtUpdateCounter(id, 5);
            CheckResult(rslt);
            Assert.AreEqual(5, rslt.Value);

            rslt = client.DtUpdateCounter(id, -5);
            CheckResult(rslt);
            Assert.AreEqual(0, rslt.Value);
        }

        [Test]
        public void InitialSetIsEmpty()
        {
            id = new RiakObjectId("sets", "travel", "cities");
            var rslt = client.DtFetchSet(id);
            CheckResult(rslt);

            int setSize = rslt.Values.Count;
            Assert.AreEqual(0, setSize);
        }

        [Test]
        public void CitiesSetAddRemoveAndView()
        {
            id = new RiakObjectId("sets", "travel", "cities");
            var citiesSet = client.DtFetchSet(id);
            CheckResult(citiesSet);

            var adds = new List<string> { "Toronto", "Montreal" };
            citiesSet = client.DtUpdateSet(id, Serializer, citiesSet.Context, adds);
            CheckResult(citiesSet);

            var removes = new List<string> { "Montreal" };
            adds = new List<string> { "Hamilton", "Ottawa" };
            citiesSet = client.DtUpdateSet(id, Serializer, citiesSet.Context, adds, removes);
            CheckResult(citiesSet);

            foreach (var value in citiesSet.Values)
            {
                string city = Encoding.UTF8.GetString(value);
                var args = new[] { city };
                Console.WriteLine("Cities Set Value: {0}", args);
            }

            Console.WriteLine("Cities Set Size: {0}", citiesSet.Values.Count);
        }

        [Test]
        public void Maps()
        {
            const string firstNameRegister = "first_name";
            const string lastNameRegister = "last_name";
            const string phoneNumberRegister = "phone_number";
            const string enterpriseCustomerFlag = "enterprise_customer";
            const string pageVisitsCounter = "page_visits";
            const string interestsSet = "interests";
            const string annikaInfoMap = "annika_info";

            id = new RiakObjectId("maps", "customers", "ahmed_info");
            var rslt = client.DtFetchMap(id);
            CheckResult(rslt);

            Console.WriteLine("------------------------------------------------------------------------\n");
            Console.WriteLine("Initial import of Ahmed's data and contact");

            // Ahmed's first name
            var firstNameRegisterMapUpdate = new MapUpdate
            {
                register_op = Serializer("Ahmed"),
                field = new MapField
                {
                    name = Serializer(firstNameRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            };

            // Ahmed's phone number
            var phoneNumberRegisterMapUpdate = new MapUpdate
            {
                register_op = Serializer("5551234567"),
                field = new MapField
                {
                    name = Serializer(phoneNumberRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            };

            // Set Ahmed to NOT be an enterprise customer right now
            var enterpriseCustomerFlagUpdate = new MapUpdate
            {
                flag_op = MapUpdate.FlagOp.DISABLE,
                field = new MapField
                {
                    name = Serializer(enterpriseCustomerFlag),
                    type = MapField.MapFieldType.FLAG
                }
            };

            var pageVisitsCounterUpdate = new MapUpdate
            {
                counter_op = new CounterOp { increment = 1 },
                field = new MapField
                {
                    name = Serializer(pageVisitsCounter),
                    type = MapField.MapFieldType.COUNTER
                }
            };

            // Interests Set
            var interestsAdds = new[] { "robots", "opera", "motorcycles" };
            var setOperation = new SetOp();
            setOperation.adds.AddRange(interestsAdds.Select(i => Serializer(i)));
            var interestsSetUpdate = new MapUpdate
            {
                set_op = setOperation,
                field = new MapField
                {
                    name = Serializer(interestsSet),
                    type = MapField.MapFieldType.SET
                }
            };

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
        }

        private void CheckResult(RiakDtMapResult rslt)
        {
            CheckResult(rslt.Result);
        }

        private void CheckResult(RiakDtSetResult rslt)
        {
            CheckResult(rslt.Result);
        }

        private void CheckResult(RiakCounterResult rslt)
        {
            CheckResult(rslt.Result);
        }
    }
}
