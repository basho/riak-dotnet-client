// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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

using System;
using System.Threading;
using CorrugatedIron.Messages;

namespace CorrugatedIron.Comms
{
    public class RiakConnectionUsageManager : IDisposable
    {
        private static int _nextClientId = 1;
        private readonly IRiakConnection _connection;

        public RiakConnectionUsageManager(IRiakConnection connection, bool setClientId)
        {
            _connection = connection;
            _connection.EndIdle();

            if (setClientId)
            {
                // For now, we'll set the client ID to something different each time we
                // dish the connection out. This is because the client ID has an impact
                // on the vclock stuff behind the scenes. Ideally, the PB client
                // interface will be updated such that the client ID can be posted
                // along with the messages rather than be tied to a given connection.
                // In the mean time, we'll just do this to make sure we don't have
                // client ID contention issues and risk having vclock issues.
                SetClientId(connection);
            }
        }

        public void Dispose()
        {
            _connection.BeginIdle();
        }

        private static void SetClientId(IRiakConnection connection)
        {
            connection.WriteRead<RpbSetClientIdReq, RpbSetClientIdResp>(new RpbSetClientIdReq { ClientId = GetNextClientId() });
        }

        private static byte[] GetNextClientId()
        {
            var clientId = Interlocked.Increment(ref _nextClientId);
            return BitConverter.GetBytes(clientId);
        }
    }
}
