namespace RiakClient.Commands.TS
{
    using System.Collections.Generic;

    /// <summary>
    /// Response to a <see cref="Get"/> command.
    /// </summary>
    public class GetResponse : Response<Row, IEnumerable<Row>>
    {
        /// <inheritdoc />
        public GetResponse()
            : base(notFound: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResponse"/> class.
        /// </summary>
        /// <param name="key">A <see cref="Row"/> representing the key.</param>
        /// <param name="values">The values for the fetched TS key.</param>
        public GetResponse(Row key, IEnumerable<Row> values)
            : base(key, values)
        {
        }
    }
}