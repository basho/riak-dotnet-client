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

namespace CorrugatedIron.Collections
{
    public class ResourcePool<TResource> : IDisposable
        where TResource : class
    {
        private readonly Func<TResource> _resourceBuilder;
        private readonly Action<TResource> _resourceDestroyer;
        private readonly ConcurrentQueue<TResource> _resources;
        private int _poolSize;
        private bool _disposing;

        public ResourcePool(int poolSize, Func<TResource> resourceBuilder, Action<TResource> resourceDestroyer)
        {
            _poolSize = poolSize;
            _resourceBuilder = resourceBuilder;
            _resourceDestroyer = resourceDestroyer;
            _resources = new ConcurrentQueue<TResource>();

            for (var i = 0; i < poolSize; ++i)
            {
                _resources.Enqueue(resourceBuilder());
            }
        }

        public void AddInstance()
        {
            _resources.Enqueue(_resourceBuilder());
            Interlocked.Increment(ref _poolSize);
        }

        public Tuple<bool, TResult> Consume<TResult>(Func<TResource, TResult> consumer)
        {
            if (_disposing) return Tuple.Create(false, default(TResult));

            TResource instance = null;
            try
            {
                if (_resources.TryDequeue(out instance))
                {
                    var result = consumer(instance);
                    return Tuple.Create(true, result);
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
                    _resources.Enqueue(instance);
                }
            }

            return Tuple.Create(false, default(TResult));
        }

        public void Dispose()
        {
            if (_disposing) return;

            _disposing = true;

            TResource instance;
            while (_resources.TryDequeue(out instance))
            {
                _resourceDestroyer(instance);
            }
        }
    }
}
