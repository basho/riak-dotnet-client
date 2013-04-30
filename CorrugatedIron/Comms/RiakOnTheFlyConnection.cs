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
using System.Threading.Tasks;

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

        public Task<Tuple<bool, TResult>> Consume<TResult>(Func<IRiakConnection, Task<TResult>> consumer)
        {
            if(_disposing)
                return TaskResult(Tuple.Create(false, default(TResult)));

            // connection on the fly
            var conn = _connFactory.CreateConnection(_nodeConfig);
            return consumer(conn).ContinueWith((Task<TResult> finishedTask) => {
                if (conn != null)
                    conn.Dispose();

                if (!finishedTask.IsFaulted)
                    return Tuple.Create(true, finishedTask.Result);
                else
                    return Tuple.Create(false, default(TResult));
            });
        }

        // consume delayed
        public Task<Tuple<bool, TResult>> DelayedConsume<TResult>(Func<IRiakConnection, Action, Task<TResult>> consumer)
        {
            if(_disposing)
                return TaskResult(Tuple.Create(false, default(TResult)));

            // connection on the fly
            var conn = _connFactory.CreateConnection(_nodeConfig);
            Action cleanup = (() => {
                if (conn != null)
                    conn.Dispose();
            });
            return consumer(conn, cleanup).ContinueWith((Task<TResult> finishedTask) => {
                if (!finishedTask.IsFaulted)
                    return Tuple.Create(true, finishedTask.Result);
                else
                    return Tuple.Create(false, default(TResult));
            });
        }

        public void Dispose()
        {
            if(_disposing) return;

            _disposing = true;
        }

        // wrap a task result
        private Task<T> TaskResult<T>(T result)
        {
            var source = new TaskCompletionSource<T>();
            source.SetResult(result);
            return source.Task;
        }
    }
}
