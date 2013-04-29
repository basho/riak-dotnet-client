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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorrugatedIron.Comms
{
    internal class RiakConnectionPool : IRiakConnectionManager
    {
        private readonly List<IRiakConnection> _allResources;
        private readonly ConcurrentStack<IRiakConnection> _resources;
        private bool _disposing;

        public RiakConnectionPool(IRiakNodeConfiguration nodeConfig, IRiakConnectionFactory connFactory)
        {
            var poolSize = nodeConfig.PoolSize;
            _allResources = new List<IRiakConnection>();
            _resources = new ConcurrentStack<IRiakConnection>();

            for(var i = 0; i < poolSize; ++i)
            {
                var conn = connFactory.CreateConnection(nodeConfig);
                _allResources.Add(conn);
                _resources.Push(conn);
            }
        }

        public Task<Tuple<bool, TResult>> Consume<TResult>(Func<IRiakConnection, Task<TResult>> consumer)
        {
            if(_disposing)
                return TaskResult(Tuple.Create(false, default(TResult)));

            IRiakConnection instance = null;
            if(_resources.TryPop(out instance))
            {
                return consumer(instance).ContinueWith((Task<TResult> finishedTask) => {
                    if (instance != null)
                        _resources.Push(instance);

                    // finshed task
                    if (!finishedTask.IsFaulted)
                        return Tuple.Create(true, finishedTask.Result);
                    else
                        return Tuple.Create(false, default(TResult));
                });
            }
            else
            {
                return TaskResult(Tuple.Create(false, default(TResult)));
            }
        }

        public Task<Tuple<bool, TResult>> DelayedConsume<TResult>(Func<IRiakConnection, Action, Task<TResult>> consumer)
        {
            if(_disposing)
                return TaskResult(Tuple.Create(false, default(TResult)));

            IRiakConnection instance = null;
            if(_resources.TryPop(out instance))
            {
                return consumer(instance, () => {}).ContinueWith((Task<TResult> finishedTask) => {
                    if (instance != null)
                        _resources.Push(instance);
                    
                    // finshed task
                    if (!finishedTask.IsFaulted)
                        return Tuple.Create(true, finishedTask.Result);
                    else
                        return Tuple.Create(false, default(TResult));
                });
            }
            else
            {
                return TaskResult(Tuple.Create(false, default(TResult)));
            }
        }

        public void Dispose()
        {
            if(_disposing) return;

            _disposing = true;

            foreach(var conn in _allResources)
            {
                conn.Dispose();
            }
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
