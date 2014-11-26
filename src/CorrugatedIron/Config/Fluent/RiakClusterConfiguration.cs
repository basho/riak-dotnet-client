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

using System;
using System.Collections.Generic;

namespace CorrugatedIron.Config.Fluent
{
    public class RiakClusterConfiguration : IRiakClusterConfiguration
    {
        private readonly List<IRiakNodeConfiguration> _nodes;
        private int _nodePollTime = 5000;
        private int _defaultRetryWaitTime = 200;
        private int _defaultRetryCount = 3;

        public IList<IRiakNodeConfiguration> RiakNodes
        {
            get { return _nodes; }
        }

        public int NodePollTime
        {
            get { return _nodePollTime; }
        }

        public int DefaultRetryWaitTime
        {
            get { return _defaultRetryWaitTime; }
        }

        public int DefaultRetryCount
        {
            get { return _defaultRetryCount; }
        }

        public RiakClusterConfiguration()
        {
            _nodes = new List<IRiakNodeConfiguration>();
        }

        public RiakClusterConfiguration SetNodePollTime(int nodePollTime)
        {
            _nodePollTime = nodePollTime;
            return this;
        }

        public RiakClusterConfiguration SetDefaultRetryWaitTime(int defaultRetryWaitTime)
        {
            _defaultRetryWaitTime = defaultRetryWaitTime;
            return this;
        }

        public RiakClusterConfiguration SetDefaultRetryCount(int defaultRetryCount)
        {
            _defaultRetryCount = defaultRetryCount;
            return this;
        }

        public RiakClusterConfiguration AddNode(Action<RiakNodeConfiguration> nodeSetup)
        {
            var node = new RiakNodeConfiguration();
            nodeSetup(node);
            _nodes.Add(node);
            return this;
        }
    }
}
