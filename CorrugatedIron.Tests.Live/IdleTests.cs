// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Comms;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.IdleTests
{
    [TestFixture]
    [Ignore("Idling is undergoing rework, so these tests are currently invalid")]
    public class WhenConnectionGoesIdle : LiveRiakConnectionTestBase
    {
        public WhenConnectionGoesIdle()
            : base("riak1NodeConfiguration")
        {
        }

        private IRiakConnection GetIdleConnection()
        {
            var result = Cluster.UseConnection(RiakResult<IRiakConnection>.Success, 1);
            //System.Threading.Thread.Sleep(ClusterConfig.RiakNodes[0].IdleTimeout + 1000);
            return result.Value;
        }

        [Test]
        public void IsIdleFlagIsSet()
        {
            var conn = GetIdleConnection();
            conn.IsIdle.ShouldBeTrue();
        }

        [Test]
        public void ConnectionIsRestoredOnNextUse()
        {
            GetIdleConnection();
            var result = Client.Ping();
            result.IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void IdleFlagIsUnsetOnNextUse()
        {
            var conn = GetIdleConnection();
            Client.Ping();
            conn.IsIdle.ShouldBeFalse();
        }
    }
}
