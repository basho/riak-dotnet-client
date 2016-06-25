namespace RiakClient.Comms
{
    using Riak.Config;

    /// <summary>
    /// An interface for factories that create <see cref="IRiakConnection"/>s from <see cref="INodeConfiguration"/>s.
    /// </summary>
    public interface IRiakConnectionFactory
    {
        /// <summary>
        /// Create a <see cref="IRiakConnection"/> from a <see cref="INodeConfiguration"/>.
        /// </summary>
        /// <param name="nodeConfig">The node configuration to base the connection on.</param>
        /// <param name="authConfig">The authentication configuration to use with the connection.</param>
        /// <returns>A new instance of a <see cref="IRiakConnection"/> type.</returns>
        IRiakConnection CreateConnection(INodeConfiguration nodeConfig, IAuthenticationConfiguration authConfig);
    }
}
