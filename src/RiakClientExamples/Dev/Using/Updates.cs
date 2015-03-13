// <copyright file="Updates.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientExamples.Dev.Using
{
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;

    /*
     * http://docs.basho.com/riak/latest/dev/using/updates/
     */
    public sealed class Updates : ExampleBase
    {
        [Test]
        public void ExamplePut()
        {
            id = PutCoach("seahawks", "Pete Carroll");
        }

        [Test]
        public void UpdateCoachExample()
        {
            id = PutCoach("packers", "Old Coach");
            UpdateCoach("packers", "Vince Lombardi");

            id = new RiakObjectId("siblings", "coaches", "packers");
            var getResult = client.Get(id);


            RiakObject packers = getResult.Value;
            Assert.AreEqual("Vince Lombardi", Encoding.UTF8.GetString(packers.Value));
            Assert.AreEqual(0, packers.Siblings.Count);

            rslt = getResult;
        }

        private RiakObjectId PutCoach(string team, string coach)
        {
            var id = new RiakObjectId("siblings", "coaches", team);
            var obj = new RiakObject(id, coach,
                RiakConstants.ContentTypes.TextPlain);
            var rslt = client.Put(obj);
            CheckResult(rslt);

            return id;
        }

        private void UpdateCoach(string team, string newCoach)
        {
            var id = new RiakObjectId("siblings", "coaches", team);
            var getResult = client.Get(id);
            CheckResult(getResult);

            RiakObject obj = getResult.Value;
            obj.SetObject<string>(newCoach, RiakConstants.ContentTypes.TextPlain);
            var rslt = client.Put(obj);
            CheckResult(rslt);
        }
    }
}
