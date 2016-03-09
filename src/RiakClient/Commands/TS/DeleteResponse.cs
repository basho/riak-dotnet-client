namespace RiakClient.Commands.TS
{
    using System.Collections.Generic;

    /// <summary>
    /// Response to a <see cref="Delete"/> command.
    /// </summary>
    public class DeleteResponse : Response
    {
        /// <inheritdoc />
        public DeleteResponse()
            : base(notFound: false)
        {
        }

        /// <inheritdoc />
        public DeleteResponse(bool notFound)
            : base(notFound)
        {
        }
    }
}