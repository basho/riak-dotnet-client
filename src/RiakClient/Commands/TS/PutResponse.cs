namespace RiakClient.Commands.TS
{
    /// <summary>
    /// Response to a <see cref="Put"/> command.
    /// </summary>
    public class PutResponse : Response<bool>
    {
        /// <inheritdoc />
        public PutResponse() : base(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PutResponse"/> class.
        /// </summary>
        /// <param name="success"><b>true</b> if successful</param>
        public PutResponse(bool success)
            : base(success)
        {
        }
    }
}