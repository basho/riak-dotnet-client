namespace Riak.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Logging;
    using RiakClient.Commands;
    using RiakClient.Util;

    public abstract class NodeManager
    {
        private readonly ILog log;

        public NodeManager(ILog log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            this.log = log;
        }

        protected void Validate(IList<INode> nodes, IRCommand cmd)
        {
            if (EnumerableUtil.IsNullOrEmpty(nodes))
            {
                var msg = string.Format(Properties.Resources.Riak_Core_NodeManagerZeroLengthNodes_fmt, cmd.Name);
                log.Error(msg);
                throw new InvalidOperationException(msg);
            }

            if (cmd == null)
            {
                var msg = string.Format(Properties.Resources.Riak_Core_NodeManagerCommandRequiredException, cmd.Name);
                log.Error(msg);
                throw new InvalidOperationException(msg);
            }
        }

        protected async Task<ExecuteResult> ExecuteAsync(INode node, IRCommand cmd)
        {
            var rslt = await node.ExecuteAsync(cmd);
            if (rslt.Executed)
            {
                log.DebugFormat(Properties.Resources.Riak_Core_NodeManager_ExecutedCommand_fmt, cmd, node);
            }

            return rslt;
        }
    }
}
