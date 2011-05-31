using CorrugatedIron.Config;

namespace CorrugatedIron.Comms
{
    public interface IRiakNodeFactory
    {
        IRiakNode CreateNode(IRiakNodeConfiguration nodeConfiguration, IRiakConnectionFactory connectionFactory);
    }

    public class RiakNodeFactory : IRiakNodeFactory
    {
        public IRiakNode CreateNode(IRiakNodeConfiguration nodeConfiguration, IRiakConnectionFactory connectionFactory)
        {
            return new RiakNode(nodeConfiguration, connectionFactory);
        }
    }
}
