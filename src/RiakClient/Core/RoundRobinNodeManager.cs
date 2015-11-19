namespace Riak.Core
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;
    using RiakClient.Commands;

    public class RoundRobinNodeManager : NodeManager, INodeManager
    {
        private static readonly ILog Log = LogManager.GetLogger<RoundRobinNodeManager>();

        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();

        private ushort nodeIndex = 0;

        public RoundRobinNodeManager() : base(Log)
        {
        }

        public async Task<ExecuteResult> ExecuteAsyncOnNode(IList<INode> nodes, IRCommand cmd, INode previous = null)
        {
            Validate(nodes, cmd);

            ushort startingIndex = nodeIndex;

            var rslt = new ExecuteResult(executed: false);
            do
            {
                INode node = null;
                try
                {
                    sync.EnterWriteLock();

                    // Check index before accessing {nodes} because elements can be removed from {nodes}.
                    if (nodeIndex >= nodes.Count)
                    {
                        nodeIndex = 0;
                    }

                    node = nodes[nodeIndex];
                    nodeIndex++;
                }
                finally
                {
                    sync.ExitWriteLock();
                }

                // don't try the same node twice in a row if we have multiple nodes
                if (nodes.Count > 1 && previous != null && previous == node)
                {
                    continue;
                }

                rslt = await ExecuteAsync(node, cmd);
                if (rslt.Executed)
                {
                    break;
                }
            }
            while (nodeIndex != startingIndex);

            return rslt;
        }
    }
}
