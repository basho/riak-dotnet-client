// <copyright file="User.cs" company="Basho Technologies, Inc.">
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
    using System;
    using System.Collections.Generic;

    public class User : IModel
    {
        private readonly string firstName;
        private readonly string lastName;
        private readonly IEnumerable<string> interests;
        private uint pageVisits = 0;

        public User(
            string firstName,
            string lastName,
            IEnumerable<string> interests,
            uint pageVisits = 0)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException("firstName", "firstName is required");
            }

            this.firstName = firstName;

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException("lastName", "lastName is required");
            }

            this.lastName = lastName;
            this.interests = interests;
            this.pageVisits = pageVisits;
        }

        public string ID
        {
            get
            {
                return string.Format("{0}_{1}",
                    firstName.ToLowerInvariant(), lastName.ToLowerInvariant());
            }
        }

        public string FirstName
        {
            get { return firstName; }
        }

        public string LastName
        {
            get { return lastName; }
        }

        public IEnumerable<string> Interests
        {
            get { return interests; }
        }

        public uint PageVisits
        {
            get { return pageVisits; }
        }
    }
}
