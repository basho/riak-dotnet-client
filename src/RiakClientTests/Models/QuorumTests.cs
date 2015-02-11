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
            Assert.Catch<ArgumentNullException>(() => new Quorum(string.Empty));
            Assert.Catch<ArgumentOutOfRangeException>(() => new Quorum("frazzle"));
        }

        [Test]
        public void Zero_IsValidQuorum()
        {
            var q = new Quorum(0);
            Assert.IsNotNull(q);
        }

        [Test]
        public void NegOne_IsInvalidQuorum()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() => new Quorum(-1));
        }

        [Test]
        public void NegTwoThroughNegFive_AreValidQuorums()
        {
            for (int i = -2; i >= -5; --i)
            {
                var q = new Quorum(i);
                Assert.AreEqual(i, q);
            }
        }

        [Test]
        public void OtherNegValues_AreInvalidQuorums()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() => new Quorum(-32));
            Assert.Catch<ArgumentOutOfRangeException>(() => new Quorum(-1024));
        }

        [Test]
        public void WhenUsingValidQuorumString_ResultsInValidQuorumValue()
        {
            var validQuorumData = new Dictionary<string, int[]>
            {
                { "one", new[] { 1, -2 } },
                { "One", new[] { 1, -2 } },
                { "ONE", new[] { 1, -2 } },
                { "onE", new[] { 1, -2 } },
                { "quorum", new[] { 2, -3 } },
                { "Quorum", new[] { 2, -3 } },
                { "QUORUM", new[] { 2, -3 } },
                { "quOrUm", new[] { 2, -3 } },
                { "all", new[] { 3, -4 } },
                { "All", new[] { 3, -4 } },
                { "ALL", new[] { 3, -4 } },
                { "alL", new[] { 3, -4 } },
                { "default", new[] { 4, -5 } },
                { "Default", new[] { 4, -5 } },
                { "DEFAULT", new[] { 4, -5 } },
                { "deFaulT", new[] { 4, -5 } }
            };

            foreach (var vqd in validQuorumData)
            {
                string quorum_str = vqd.Key;
                int quorum_int_base = vqd.Value[0];
                int quorum_as_int = vqd.Value[1];
                uint quorum_uint = uint.MaxValue - (uint)quorum_int_base;

                var quorum = new Quorum(quorum_str);
                Assert.AreEqual(quorum_as_int, (int)quorum);
                Assert.AreEqual(quorum_uint, (uint)quorum);
                Assert.AreEqual(quorum_str.ToLowerInvariant(), (string)quorum);
                Assert.AreEqual(quorum_str.ToLowerInvariant(), quorum.ToString());

                var quorum_from_int = new Quorum(quorum_as_int);
                Assert.AreEqual(quorum_from_int, quorum);
                Assert.True(quorum_from_int.Equals(quorum));
            }
        }
    }
}