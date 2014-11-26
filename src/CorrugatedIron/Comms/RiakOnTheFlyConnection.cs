// Copyright (c) 2013 - OJ Reeves & Jeremiah Peschka
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
using System;

namespace CorrugatedIron.Comms
{
    internal class RiakOnTheFlyConnection : IRiakConnectionManager
    {
        private readonly IRiakNodeConfiguration _nodeConfig;
        private readonly IRiakConnectionFactory _connFactory;
        private bool _disposing;

        public RiakOnTheFlyConnection(IRiakNodeConfiguration nodeConfig, IRiakConnectionFactory connFactory)
        {
            _nodeConfig = nodeConfig;
            _connFactory = connFactory;
        }

        public Tuple<bool, TResult> Consume<TResult>(Func<IRiakConnection, TResult> consumer)
        {
            if(_disposing) return Tuple.Create(false, default(TResult));

            using (var conn = _connFactory.CreateConnection(_nodeConfig))
            {
                try
                {
                    var result = consumer(conn);
                    return Tuple.Create(true, result);
                }
                catch(Exception)
                {
                    return Tuple.Create(false, default(TResult));
                }
            }
        }

        public Tuple<bool, TResult> DelayedConsume<TResult>(Func<IRiakConnection, Action, TResult> consumer)
        {
            if(_disposing) return Tuple.Create(false, default(TResult));

            IRiakConnection conn = null;

            try
            {
                conn = _connFactory.CreateConnection(_nodeConfig);
                var result = consumer(conn, conn.Dispose);
                return Tuple.Create(true, result);
            }
            catch(Exception)
            {
                if (conn != null)
                {
                    conn.Dispose();
                }
                return Tuple.Create(false, default(TResult));
            }
        }

        public void Dispose()
        {
            if(_disposing) return;

            _disposing = true;
        }
    }
}
