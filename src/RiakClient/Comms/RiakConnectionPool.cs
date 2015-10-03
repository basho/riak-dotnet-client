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
