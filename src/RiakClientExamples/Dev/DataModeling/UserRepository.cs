// <copyright file="BlogPostRepository.cs" company="Basho Technologies, Inc.">
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

#pragma warning disable 618

namespace RiakClientExamples.Dev.DataModeling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RiakClient;
    using RiakClient.Messages;
    using RiakClient.Models.RiakDt;
    using RiakClient.Util;

    public class UserRepository : Repository<User>
    {
        const string firstNameRegister = "first_name";
        const string lastNameRegister = "last_name";
        const string interestsSet = "interests";
        const string pageVisitsCounter = "visits";
        const string paidAccountFlag = "paid_account";

        public UserRepository(IRiakClient client)
            : base(client)
        {
        }

        public override User Get(string key, bool notFoundOK = false)
        {
            var fetchRslt = client.DtFetchMap(GetRiakObjectId(key));
            CheckResult(fetchRslt.Result);

            string firstName = null;
            string lastName = null;
            var interests = new List<string>();
            uint pageVisits = 0;

            foreach (var value in fetchRslt.Values)
            {
                RiakDtMapField mapField = value.Field;
                switch (mapField.Name)
                {
                    case firstNameRegister:
                        if (mapField.Type != RiakDtMapField.RiakDtMapFieldType.Register)
                        {
                            throw new InvalidCastException("expected Register type");
                        }
                        firstName = TextDeserializer(value.RegisterValue);
                        break;
                    case lastNameRegister:
                        if (mapField.Type != RiakDtMapField.RiakDtMapFieldType.Register)
                        {
                            throw new InvalidCastException("expected Register type");
                        }
                        lastName = TextDeserializer(value.RegisterValue);
                        break;
                    case interestsSet:
                        if (mapField.Type != RiakDtMapField.RiakDtMapFieldType.Set)
                        {
                            throw new InvalidCastException("expected Set type");
                        }
                        interests.AddRange(value.SetValue.Select(v => TextDeserializer(v)));
                        break;
                    case pageVisitsCounter:
                        if (mapField.Type != RiakDtMapField.RiakDtMapFieldType.Counter)
                        {
                            throw new InvalidCastException("expected Counter type");
                        }
                        pageVisits = (uint)value.Counter.Value;
                        break;
                    /*
                     * Note: can do additional checks here in default case
                     */
                }
            }

            return new User(firstName, lastName, interests, pageVisits);
        }

        public override string Save(User model)
        {
            var mapUpdates = new List<MapUpdate>();

            mapUpdates.Add(new MapUpdate
            {
                register_op = TextSerializer(model.FirstName),
                field = new MapField
                {
                    name = TextSerializer(firstNameRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            mapUpdates.Add(new MapUpdate
            {
                register_op = TextSerializer(model.LastName),
                field = new MapField
                {
                    name = TextSerializer(lastNameRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            if (EnumerableUtil.NotNullOrEmpty(model.Interests))
            {
                var interestsSetOp = new SetOp();
                interestsSetOp.adds.AddRange(
                    model.Interests.Select(i => TextSerializer(i))
                );
                mapUpdates.Add(new MapUpdate
                {
                    set_op = interestsSetOp,
                    field = new MapField
                    {
                        name = TextSerializer(interestsSet),
                        type = MapField.MapFieldType.SET
                    }
                });
            }

            // Update without context
            UpdateMap(model, mapUpdates);

            return model.ID;
        }

        public void AddInterest(User model, string interest)
        {
            var interestsSetOp = new SetOp();
            interestsSetOp.adds.Add(TextSerializer(interest));

            var mapUpdates = new List<MapUpdate>
            {
                new MapUpdate
                {
                    set_op = interestsSetOp,
                    field = new MapField
                    {
                        name = TextSerializer(interestsSet),
                        type = MapField.MapFieldType.SET
                    }
                }
            };

            UpdateMap(model, mapUpdates, fetchFirst: true);
        }

        public void RemoveInterest(User model, string interest)
        {
            var interestsSetOp = new SetOp();
            interestsSetOp.removes.Add(TextSerializer(interest));

            var mapUpdates = new List<MapUpdate>
            {
                new MapUpdate
                {
                    set_op = interestsSetOp,
                    field = new MapField
                    {
                        name = TextSerializer(interestsSet),
                        type = MapField.MapFieldType.SET
                    }
                }
            };

            UpdateMap(model, mapUpdates, fetchFirst: true);
        }

        public void IncrementPageVisits(User model)
        {
            var mapUpdates = new List<MapUpdate>();

            mapUpdates.Add(new MapUpdate
            {
                counter_op = new CounterOp { increment = 1 },
                field = new MapField
                {
                    name = TextSerializer(pageVisitsCounter),
                    type = MapField.MapFieldType.COUNTER
                }
            });

            // Update without context
            UpdateMap(model, mapUpdates);
        }

        public void UpgradeAccount(User model)
        {
            var mapUpdates = new List<MapUpdate>();

            mapUpdates.Add(new MapUpdate
            {
                flag_op = MapUpdate.FlagOp.ENABLE,
                field = new MapField
                {
                    name = TextSerializer(paidAccountFlag),
                    type = MapField.MapFieldType.FLAG
                }
            });

            UpdateMap(model, mapUpdates, fetchFirst: true);
        }

        public void DowngradeAccount(User model)
        {
            var mapUpdates = new List<MapUpdate>();

            mapUpdates.Add(new MapUpdate
            {
                flag_op = MapUpdate.FlagOp.DISABLE,
                field = new MapField
                {
                    name = TextSerializer(paidAccountFlag),
                    type = MapField.MapFieldType.FLAG
                }
            });

            UpdateMap(model, mapUpdates, fetchFirst: true);
        }

        protected override string BucketType
        {
            get { return "maps"; }
        }

        protected override string BucketName
        {
            get { return "users"; }
        }
    }
}

#pragma warning restore 618