// <copyright file="BlogPostRepository.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2015 - Basho Technologies, Inc.
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

namespace RiakClientExamples.Dev.DataModeling
{
    using System.Linq;
    using System.Collections.Generic;
    using RiakClient;
    using RiakClient.Messages;
    using RiakClient.Models;
    using RiakClient.Util;
    using System;

    public class UserRepository : Repository<User>
    {
        const string firstNameRegister = "first_name";
        const string lastNameRegister = "last_name";
        const string interestsSet = "interests";
        const string visitsCounter = "visits";

        private readonly User userModel;

        public UserRepository(IRiakClient client, User model)
            : base(client, model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            this.userModel = model;
        }

        public override string Save()
        {
            var mapUpdates = new List<MapUpdate>();

            mapUpdates.Add(new MapUpdate
            {
                register_op = TextSerializer(userModel.FirstName),
                field = new MapField
                {
                    name = TextSerializer(firstNameRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            mapUpdates.Add(new MapUpdate
            {
                register_op = TextSerializer(userModel.LastName),
                field = new MapField
                {
                    name = TextSerializer(lastNameRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            if (EnumerableUtil.NotNullOrEmpty(userModel.Interests))
            {
                var interestsSetOp = new SetOp();
                interestsSetOp.adds.AddRange(
                    userModel.Interests.Select(i => TextSerializer(i))
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

            // Insert without context
            var rslt = client.DtUpdateMap(
                GetRiakObjectId(), TextSerializer, null, null, mapUpdates, null);
            CheckResult(rslt.Result);

            return model.ID;
        }

        public void IncrementPageVisits()
        {
            var mapUpdates = new List<MapUpdate>();

            mapUpdates.Add(new MapUpdate
            {
                counter_op = new CounterOp { increment = 1 },
                field = new MapField
                {
                    name = TextSerializer(visitsCounter),
                    type = MapField.MapFieldType.COUNTER
                }
            });

            // Update without context
            var rslt = client.DtUpdateMap(
                GetRiakObjectId(), TextSerializer, null, null, mapUpdates, null);
            CheckResult(rslt.Result);
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
