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

using CorrugatedIron.Config;

namespace CorrugatedIron.Comms
{
    public interface IRiakConnectionFactory
    {
        IRiakConnection CreateConnection(IRiakNodeConfiguration nodeConfiguration);
    }

    public class RiakConnectionFactory : IRiakConnectionFactory
    {
        public IRiakConnection CreateConnection(IRiakNodeConfiguration nodeConfiguration)
        {
            // As pointless as this seems, it serves the purpose of decoupling the
            // creation of the connections to the node itself. Also means we can
            // pull it apart to test it
            return new RiakConnection(nodeConfiguration);
        }
    }
}
