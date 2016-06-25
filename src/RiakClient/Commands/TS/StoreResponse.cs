namespace RiakClient.Commands.TS
{
    /// <summary>
    /// Response to a <see cref="Store"/> command.
    /// </summary>
    public class StoreResponse : Response
    {
        /// <inheritdoc />
        public StoreResponse()
            : base(false)
        {
        }
    }
}