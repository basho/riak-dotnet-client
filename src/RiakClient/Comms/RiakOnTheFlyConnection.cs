namespace RiakClient.Comms
{
    using System;
    using Config;

    internal class RiakOnTheFlyConnection : IRiakConnectionManager
    {
        private readonly IRiakNodeConfiguration nodeConfig;
        private readonly IRiakAuthenticationConfiguration authConfig;
        private readonly IRiakConnectionFactory connFactory;
        private bool disposing;

        public RiakOnTheFlyConnection(
            IRiakNodeConfiguration nodeConfig,
            IRiakAuthenticationConfiguration authConfig,
            IRiakConnectionFactory connFactory)
        {
            this.nodeConfig = nodeConfig;
            this.authConfig = authConfig;
            this.connFactory = connFactory;
        }

        public Tuple<bool, TResult> Consume<TResult>(Func<IRiakConnection, TResult> consumer)
        {
            if (disposing)
            {
                return Tuple.Create(false, default(TResult));
            }

            using (var conn = connFactory.CreateConnection(nodeConfig, authConfig))
            {
                try
                {
                    var result = consumer(conn);
                    return Tuple.Create(true, result);
                }
                catch (Exception)
                {
                    return Tuple.Create(false, default(TResult));
                }
            }
        }

        public Tuple<bool, TResult> DelayedConsume<TResult>(Func<IRiakConnection, Action, TResult> consumer)
        {
            if (disposing)
            {
                return Tuple.Create(false, default(TResult));
            }

            IRiakConnection conn = null;

            try
            {
                conn = connFactory.CreateConnection(nodeConfig, authConfig);
                var result = consumer(conn, conn.Dispose);
                return Tuple.Create(true, result);
            }
            catch (Exception)
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
            if (disposing)
            {
                return;
            }

            disposing = true;
        }
    }
}
