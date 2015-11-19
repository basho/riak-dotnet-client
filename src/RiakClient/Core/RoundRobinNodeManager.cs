namespace Riak.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;
    using RiakClient.Commands;

    public class RoundRobinNodeManager : NodeManager, INodeManager, IDisposable
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

            bool first = true;
            ushort startingIndex = 0;

            sync.EnterReadLock();
            try
            {
                startingIndex = nodeIndex;
            }
            finally
            {
                sync.ExitReadLock();
            }

            var rslt = new ExecuteResult(executed: false);
            for (;;)
            {
                INode node = null;
                sync.EnterUpgradeableReadLock();
                try
                {
                    // Check index before accessing {nodes} because elements can be removed from {nodes}.
                    if (nodeIndex >= nodes.Count)
                    {
                        nodeIndex = 0;
                    }

                    if (!first && nodeIndex == startingIndex)
                    {
                        break;
                    }

                    first = false;
                    node = nodes[nodeIndex];

                    sync.EnterWriteLock();
                    try
                    {
                        nodeIndex++;
                    }
                    finally
                    {
                        sync.ExitWriteLock();
                    }
                }
                finally
                {
                    sync.ExitUpgradeableReadLock();
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

            return rslt;
        }

        public void Dispose()
        {
            sync.Dispose();
        }
    }
}
