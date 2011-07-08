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
using System.Collections.Concurrent;
using System.Threading;
using CorrugatedIron.Config;

namespace CorrugatedIron.Comms
{
    internal class RiakConnectionPool : IDisposable
    {
        private readonly int _resourceWaitTimeout;
        private readonly ConcurrentStack<IRiakConnection> _resources;
        private readonly Semaphore _resourceLock;
        private bool _disposing;

        public RiakConnectionPool(IRiakNodeConfiguration nodeConfig, IRiakConnectionFactory connFactory)
        {
            var poolSize = nodeConfig.PoolSize;
            _resourceWaitTimeout = nodeConfig.AcquireTimeout;
            _resources = new ConcurrentStack<IRiakConnection>();

            _resourceLock = new Semaphore(0, poolSize);

            for (var i = 0; i < poolSize; ++i)
            {
                _resources.Push(connFactory.CreateConnection(nodeConfig));
            }

            _resourceLock.Release(poolSize);
        }

        public Tuple<bool, TResult> Consume<TResult>(Func<IRiakConnection, TResult> consumer)
        {
            if (_disposing) return Tuple.Create(false, default(TResult));

            IRiakConnection instance = null;
            try
            {
                if (_resourceLock.WaitOne(_resourceWaitTimeout))
                {
                    if (_resources.TryPop(out instance))
                    {
                        var result = consumer(instance);
                        return Tuple.Create(true, result);
                    }
                }
            }
            catch (Exception)
            {
                return Tuple.Create(false, default(TResult));
            }
            finally
            {
                if (instance != null)
                {
                    _resources.Push(instance);
                    _resourceLock.Release();
                }
            }

            return Tuple.Create(false, default(TResult));
        }

        public Tuple<bool, TResult> DelayedConsume<TResult>(Func<IRiakConnection, Action, TResult> consumer)
        {
            if (_disposing) return Tuple.Create(false, default(TResult));

            IRiakConnection instance = null;
            try
            {
                if (_resourceLock.WaitOne(_resourceWaitTimeout))
                {
                    if (_resources.TryPop(out instance))
                    {
                        Action cleanup = () =>
                            {
                                var i = instance;
                                instance = null;
                                _resources.Push(i);
                                _resourceLock.Release();
                            };
                        var result = consumer(instance, cleanup);
                        return Tuple.Create(true, result);
                    }
                }
            }
            catch (Exception)
            {
                if (instance != null)
                {
                    _resources.Push(instance);
                    _resourceLock.Release();
                }
                return Tuple.Create(false, default(TResult));
            }

            return Tuple.Create(false, default(TResult));
        }

        public void Dispose()
        {
            if (_disposing) return;

            _disposing = true;

            // TODO: make sure we clean up all the connections
            // ie. do tracking of released resources

            IRiakConnection instance;
            while (_resources.TryPop(out instance))
            {
                instance.Dispose();
            }
        }
    }
}
