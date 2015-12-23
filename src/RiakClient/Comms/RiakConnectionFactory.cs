namespace RiakClient.Comms
{
    using Riak.Config;

    /// <summary>
    /// A factory that creates <see cref="IRiakConnection"/>s from <see cref="INodeConfiguration"/>s.
    /// </summary>
    public class RiakConnectionFactory : IRiakConnectionFactory
    {
        /// <inheritdoc/>
        public IRiakConnection CreateConnection(INodeConfiguration nodeConfig, IAuthenticationConfiguration authConfig)
        {
            // As pointless as this seems, it serves the purpose of decoupling the
            // creation of the connections to the node itself. Also means we can
            // pull it apart to test it
            return new RiakConnection(nodeConfig, authConfig);
        }
    }
}
