namespace Riak.Core
{
    using System.Threading.Tasks;
    using RiakClient.Commands;

    public interface INode
    {
        Task<ExecuteResult> ExecuteAsync(IRCommand cmd);
    }
}