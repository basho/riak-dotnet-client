namespace RiakClient.Comms
{
    using System;
    using System.Collections.Generic;
    using Riak.Config;

    internal class RiakNode : IRiakNode
    {
        private readonly IRiakConnectionManager connections;
        private readonly bool externalLoadBalancer = false;
        private bool disposing;

        public RiakNode(
            INodeConfiguration nodeConfig,
            IAuthenticationConfiguration authConfig,
            IRiakConnectionFactory connectionFactory)
        {
            externalLoadBalancer = nodeConfig.ExternalLoadBalancer;

            // assume that if the node has a pool size of 0 then the intent is to have the connections
            // made on the fly
            if (nodeConfig.PoolSize == 0)
            {
                connections = new RiakOnTheFlyConnection(nodeConfig, authConfig, connectionFactory);
            }
            else
            {
                connections = new RiakConnectionPool(nodeConfig, authConfig, connectionFactory);
            }
        }

        public bool CanMarkOffline
        {
            get { return externalLoadBalancer == false; }
        }

        public RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun)
        {
            return UseConnection(useFun, RiakResult.FromError);
        }

        public RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun)
        {
            return UseConnection(useFun, RiakResult<TResult>.FromError);
        }

        public RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun)
            where TResult : RiakResult
        {
            if (disposing)
            {
                return RiakResult<IEnumerable<TResult>>.FromError(ResultCode.ShuttingDown, "Connection is shutting down", true);
            }

            var response = connections.DelayedConsume(useFun);
            if (response.Item1)
            {
                return response.Item2;
            }

            return RiakResult<IEnumerable<TResult>>.FromError(ResultCode.NoConnections, "Unable to acquire connection", true);
        }

        public void Dispose()
        {
            disposing = true;
            Dispose(disposing);
            GC.SuppressFinalize(disposing);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                connections.Dispose();
            }
        }

        private TRiakResult UseConnection<TRiakResult>(Func<IRiakConnection, TRiakResult> useFun, Func<ResultCode, string, bool, TRiakResult> onError)
            where TRiakResult : RiakResult
        {
            if (disposing)
            {
                return onError(ResultCode.ShuttingDown, "Connection is shutting down", true);
            }

            var response = connections.Consume(useFun);
            if (response.Item1)
            {
                return response.Item2;
            }

            return onError(ResultCode.NoConnections, "Unable to acquire connection", true);
        }
    }
}
