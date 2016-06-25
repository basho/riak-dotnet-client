namespace RiakClient.Comms.LoadBalancing
{
    using System.Collections.Generic;
    using System.Linq;
    using Containers;

    /// <summary>
    /// Represents a Round-Robin based load balancing strategy.
    /// </summary>
    public class RoundRobinStrategy : ILoadBalancingStrategy
    {
        private readonly object nodesLock = new object();
        private IList<IRiakNode> nodes;
        private IConcurrentEnumerator<IRiakNode> roundRobin;

        /// <inheritdoc/>
        public void Initialise(IEnumerable<IRiakNode> nodes)
        {
            this.nodes = nodes.ToList();
            this.roundRobin = new ConcurrentEnumerable<IRiakNode>(RoundRobin()).GetEnumerator();
        }

        /// <inheritdoc/>
        public IRiakNode SelectNode()
        {
            IRiakNode node = null;

            if (roundRobin.TryMoveNext(out node))
            {
                return node;
            }

            return null;
        }

        /// <inheritdoc/>
        public void RemoveNode(IRiakNode node)
        {
            lock (nodesLock)
            {
                if (nodes.Contains(node))
                {
                    nodes.Remove(node);
                }
            }
        }

        /// <inheritdoc/>
        public void AddNode(IRiakNode node)
        {
            lock (nodesLock)
            {
                if (!nodes.Contains(node))
                {
                    nodes.Add(node);
                }
            }
        }

        private IEnumerable<IRiakNode> RoundRobin()
        {
            while (true)
            {
                IList<IRiakNode> list;

                lock (nodesLock)
                {
                    list = new List<IRiakNode>(nodes);
                }

                if (list.Count > 0)
                {
                    var nodes = list.GetEnumerator();
                    while (nodes.MoveNext() && nodes.Current != null)
                    {
                        yield return nodes.Current;
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }
    }
}
