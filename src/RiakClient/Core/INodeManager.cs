namespace Riak.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RiakClient.Commands;

    public interface INodeManager
    {
        Task<ExecuteResult> ExecuteAsyncOnNode(IList<INode> nodes, IRCommand cmd, INode previous = null);
    }
}