// <copyright file="RiakConnectionPool.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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
// </copyright>

namespace RiakClient.Comms
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Config;

    internal class RiakConnectionPool : IRiakConnectionManager
    {
        private readonly ICollection<IRiakConnection> allResources = new List<IRiakConnection>();
        private readonly ConcurrentStack<IRiakConnection> resources = new ConcurrentStack<IRiakConnection>();
        private bool disposing;

        public RiakConnectionPool(
            IRiakNodeConfiguration nodeConfig,
            IRiakAuthenticationConfiguration authConfig,
            IRiakConnectionFactory connFactory)
        {
            int poolSize = nodeConfig.PoolSize;

            for (var i = 0; i < poolSize; ++i)
            {
                var conn = connFactory.CreateConnection(nodeConfig, authConfig);
                allResources.Add(conn);
                resources.Push(conn);
            }
        }

        public Tuple<bool, TResult> Consume<TResult>(Func<IRiakConnection, TResult> consumer)
        {
            if (disposing)
            {
                return Tuple.Create(false, default(TResult));
            }

            IRiakConnection instance = null;
            try
            {
                if (resources.TryPop(out instance))
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
                    resources.Push(instance);
                }
            }

            return Tuple.Create(false, default(TResult));
        }

        public Tuple<bool, TResult> DelayedConsume<TResult>(Func<IRiakConnection, Action, TResult> consumer)
        {
            if (disposing)
            {
                return Tuple.Create(false, default(TResult));
            }

            IRiakConnection instance = null;
            try
            {
                if (resources.TryPop(out instance))
                {
                    Action cleanup = () =>
                    {
                        var i = instance;
                        instance = null;
                        resources.Push(i);
                    };

                    var result = consumer(instance, cleanup);
                    return Tuple.Create(true, result);
                }
            }
            catch (Exception)
            {
                if (instance != null)
                {
                    resources.Push(instance);
                }

                return Tuple.Create(false, default(TResult));
            }

            return Tuple.Create(false, default(TResult));
        }

        public void Dispose()
        {
            if (disposing)
            {
                return;
            }

            disposing = true;

            foreach (var conn in allResources)
            {
                conn.Dispose();
            }
        }
    }
}
