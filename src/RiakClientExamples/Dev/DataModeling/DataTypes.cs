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

namespace RiakClientExamples.Dev.DataModeling
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;

    /*
     * http://docs.basho.com/riak/latest/dev/data-modeling/data-types/
     */
    public sealed class DataTypes : ExampleBase
    {
        [Test]
        public void UserExample()
        {
            var interests = new HashSet<string> { "distributed systems", "Erlang" };
            var joe = new User("Joe", "Armstrong", interests);

            var entityManager = new EntityManager(client);
            entityManager.Add(joe);
            var repo = new UserRepository(client);
            repo.Save(joe);

            joe.VisitPage();

            joe.AddInterest("riak");

            repo.UpgradeAccount(joe);

            var joeFetched = repo.Get(joe.ID);

            Assert.GreaterOrEqual(joe.PageVisits, 0);
            Assert.Contains("riak", joeFetched.Interests.ToArray());

            PrintObject(joeFetched);

            repo.DowngradeAccount(joe);

            joeFetched = repo.Get(joe.ID);
            PrintObject(joeFetched);
        }
    }
}