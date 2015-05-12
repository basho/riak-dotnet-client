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

namespace RiakClientExamples.Dev.DataModeling
{
    using System.Linq;
    using RiakClient;
    using RiakClient.Commands.CRDT;

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
            FetchMap cmd = new FetchMap.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(key)
                .Build();

            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);
            MapResponse response = cmd.Response;
            Map map = response.Value;

            string firstName = map.Registers.GetValue(firstNameRegister);
            string lastName = map.Registers.GetValue(lastNameRegister);
            var interests = map.Sets.GetValue(interestsSet).ToArray();
            uint pageVisits = (uint)map.Counters.GetValue(pageVisitsCounter);

            bool accountStatus;
            map.Flags.TryGetValue(paidAccountFlag, out accountStatus);

            return new User(firstName, lastName, interests, pageVisits, accountStatus);
        }

        public override string Save(User model)
        {
            var mapOperation = new UpdateMap.MapOperation();

            mapOperation.SetRegister(firstNameRegister, model.FirstName);
            mapOperation.SetRegister(lastNameRegister, model.LastName);
            mapOperation.IncrementCounter(pageVisitsCounter, model.PageVisits);
            mapOperation.AddToSet(interestsSet, model.Interests);

            // Insert does not require context
            RiakString key = UpdateMap(model, mapOperation, fetchFirst: false);
            return (string)key;
        }

        public void AddInterest(User model, string interest)
        {
            var mapOperation = new UpdateMap.MapOperation();
            mapOperation.AddToSet(interestsSet, interest);

            // Adding to a set does not require context
            UpdateMap(model, mapOperation, fetchFirst: false);
        }

        public void RemoveInterest(User model, string interest)
        {
            var mapOperation = new UpdateMap.MapOperation();
            mapOperation.RemoveFromSet(interestsSet, interest);

            // Removing from a set requires context
            UpdateMap(model, mapOperation, fetchFirst: true);
        }

        public void IncrementPageVisits(User model)
        {
            var mapOperation = new UpdateMap.MapOperation();
            mapOperation.IncrementCounter(pageVisitsCounter, 1);

            // Update without context
            UpdateMap(model, mapOperation, fetchFirst: false);
        }

        public void UpgradeAccount(User model)
        {
            SetPaidAccount(model, true);
        }

        public void DowngradeAccount(User model)
        {
            SetPaidAccount(model, false);
        }

        protected override string BucketType
        {
            get { return "maps"; }
        }

        protected override string Bucket
        {
            get { return "users"; }
        }

        private void SetPaidAccount(User model, bool value)
        {
            var mapOperation = new UpdateMap.MapOperation();
            mapOperation.SetFlag(paidAccountFlag, value);

            // Flag update does not require context
            UpdateMap(model, mapOperation, fetchFirst: false);
        }
    }
}