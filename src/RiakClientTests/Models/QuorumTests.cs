// <copyright file="QuorumTests.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.Models
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RiakClient.Models;

    [TestFixture]
    public class QuorumTests
    {
        [Test]
        public void WhenUsingInvalidQuorumString_ThrowsArgumentException()
        {
            Assert.Catch<ArgumentNullException>(() => new Quorum(string.Empty), "Expected ArgumentNullException");
            Assert.Catch<ArgumentOutOfRangeException>(() => new Quorum("frazzle"), "Expected ArgumentOutOfRangeException");
            Assert.Catch<ArgumentOutOfRangeException>(() => new Quorum(-32), "Expected ArgumentOutOfRangeException");
        }

        [Test]
        public void Zero_IsValidQuorum()
        {
            var q = new Quorum(0);
            Assert.IsNotNull(q);
            Assert.AreEqual(0, (int)q);
        }

        [Test]
        public void WhenUsingValidQuorumString_ResultsInValidQuorumValue()
        {
            var validQuorumData = new Dictionary<string, int>
            {
                { "one", 1 },
                { "One", 1 },
                { "ONE", 1 },
                { "onE", 1 },
                { "quorum", 2 },
                { "Quorum", 2 },
                { "QUORUM", 2 },
                { "quOrUm", 2 },
                { "all", 3 },
                { "All", 3 },
                { "ALL", 3 },
                { "alL", 3 },
                { "default", 4 },
                { "Default", 4 },
                { "DEFAULT", 4 },
                { "deFaulT", 4 }
            };

            foreach (var vqd in validQuorumData)
            {
                string quorum_str = vqd.Key;
                int quorum_int = vqd.Value;
                uint quorum_uint = uint.MaxValue - (uint)quorum_int;

                var quorum = new Quorum(quorum_str);
                Assert.AreEqual(quorum_int, (int)quorum);
                Assert.AreEqual(quorum_uint, (uint)quorum);
                Assert.AreEqual(quorum_str.ToLowerInvariant(), (string)quorum);
                Assert.AreEqual(quorum_str.ToLowerInvariant(), quorum.ToString());

                var quorum_from_int = new Quorum(quorum_int);
                Assert.AreEqual(quorum_from_int, quorum);
            }
        }
    }
}