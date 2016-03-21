namespace RiakClient.Commands
{
    /// <summary>
    /// Response to a <see cref="FetchServerInfo"/> command.
    /// </summary>
    public class ServerInfoResponse : Response<ServerInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfoResponse"/> class.
        /// </summary>
        public ServerInfoResponse()
            : base(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfoResponse"/> class.
        /// </summary>
        /// <param name="value">The fetched server information.</param>
        public ServerInfoResponse(ServerInfo value)
            : base(value)
        {
        }
    }
}
