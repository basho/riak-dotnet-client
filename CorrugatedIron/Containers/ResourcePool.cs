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

namespace CorrugatedIron.Containers
{
    public class ResourcePool<TResource> : IDisposable
        where TResource : class
    {
        private readonly int _resourceWaitTimeout;
        private readonly Action<TResource> _resourceDestroyer;
        private readonly ConcurrentStack<TResource> _resources;
        private readonly Semaphore _resourceLock;
        private bool _disposing;

        public ResourcePool(int poolSize, int resourceWaitTimeout, Func<TResource> resourceBuilder, Action<TResource> resourceDestroyer)
        {
            _resourceWaitTimeout = resourceWaitTimeout;
            _resourceDestroyer = resourceDestroyer;
            _resources = new ConcurrentStack<TResource>();

            _resourceLock = new Semaphore(0, poolSize);

            for (var i = 0; i < poolSize; ++i)
            {
                _resources.Push(resourceBuilder());
            }

            _resourceLock.Release(poolSize);
        }

        public Tuple<bool, TResult> Consume<TResult>(Func<TResource, TResult> consumer)
        {
            if (_disposing) return Tuple.Create(false, default(TResult));

            TResource instance = null;
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

        public Tuple<bool, TResult> StreamConsume<TResult>(Func<TResource, Action, TResult> consumer)
        {
            if (_disposing) return Tuple.Create(false, default(TResult));

            TResource instance = null;
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

            TResource instance;
            while (_resources.TryPop(out instance))
            {
                _resourceDestroyer(instance);
            }
        }
    }
}
