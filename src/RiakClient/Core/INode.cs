namespace Riak.Core
{
    using System.Threading.Tasks;
    using RiakClient.Commands;

    public interface INode
    {
        int ExecuteCount
        {
            get;
        }

        Task<ExecuteResult> ExecuteAsync(IRCommand cmd);
    }
}