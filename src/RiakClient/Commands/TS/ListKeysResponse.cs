namespace RiakClient.Commands.TS
{
    using System.Collections.Generic;

    /// <summary>
    /// Response to a <see cref="ListKeys"/> command.
    /// </summary>
    public class ListKeysResponse : Response<IEnumerable<Row>>
    {
        /// <inheritdoc />
        public ListKeysResponse()
            : base(notFound: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListKeysResponse"/> class.
        /// </summary>
        /// <param name="values">The rows for the fetched TS data.</param>
        public ListKeysResponse(IEnumerable<Row> values)
            : base(values)
        {
        }
    }
}