using CorrugatedIron.Comms;

namespace CorrugatedIron
{
    public class RiakEndPointContext : IRiakEndPointContext
    {
        public IRiakNode Node { get; set; }
        public RiakPbcSocket Socket { get; set; }

        public void Dispose()
        {
            if (Socket != null)
            {
                Socket.Disconnect().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            if (Node != null && Socket != null)
            {
                Node.Release(Socket);
            }

            if (Socket != null)
            {
                Socket = null;
            }

            if (Node != null)
            {
                Node = null;
            }
        }
    }
}