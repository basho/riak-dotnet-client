namespace CorrugatedIron
{
    public sealed class RiakClusterManager : IRiakClusterManager
    {
        private RiakClusterManager()
        {
            EndPoint = RiakCluster.FromConfig("riakConfig");
        }

        public static IRiakClusterManager Instance
        {
            get
            {
                return Nested.Instance;
            }
        }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly IRiakClusterManager Instance = new RiakClusterManager();
        }

        public IRiakEndPoint EndPoint { get; set; }
    }
}
