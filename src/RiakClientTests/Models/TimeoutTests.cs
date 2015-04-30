// <copyright file="TimeoutTests.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.Models
{
    using System;
    using NUnit.Framework;
    using RiakClient;

    [TestFixture]
    public class TimeoutTests
    {
        [Test]
        public void WhenUsingInvalidTimeout_ThrowsArgumentException()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() => new Timeout(-1));
        }

        [Test]
        public void OtherNegValues_AreInvalidTimeouts()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() => new Timeout(-32));
            Assert.Catch<ArgumentOutOfRangeException>(() => new Timeout(-1024));
        }

        [Test]
        public void ToString_ReturnsMilliseconds()
        {
            var t = new Timeout(123456);
            Assert.AreEqual("123456", t.ToString());
            Assert.AreEqual("123456", (string)t);
        }

        [Test]
        public void WhenUsingValidTimeoutMillis_ResultsInValidTimeoutInstance()
        {
            var r = new Random();

            for (ushort i = 0; i < ushort.MaxValue; ++i)
            {
                var millis = r.Next(int.MaxValue);
                var t = new Timeout(millis);
                Assert.AreEqual(millis, (int)t);
                Assert.AreEqual(millis, (uint)t);

                Assert.AreEqual(TimeSpan.FromMilliseconds(millis), (TimeSpan)t);

                var t2 = new Timeout(millis);
                Assert.AreEqual(t2, t);
            }
        }
    }
}