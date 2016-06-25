namespace RiakClient.Commands.TS
{
    using System.Collections.Generic;
    using Util;

    /// <summary>
    /// Response to a <see cref="Get"/> command.
    /// </summary>
    public class GetResponse : Response<Row, IEnumerable<Row>>
    {
        private readonly IEnumerable<Column> columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResponse"/> class.
        /// </summary>
        /// <param name="key">A <see cref="Row"/> representing the key.</param>
        /// <param name="columns">The columns for the fetched TS data.</param>
        /// <param name="values">The rows for the fetched TS data.</param>
        public GetResponse(Row key, IEnumerable<Column> columns, IEnumerable<Row> values)
            : base(key, values)
        {
            this.columns = columns;
        }

        public override bool NotFound
        {
            get { return EnumerableUtil.IsNullOrEmpty(Value); }
        }

        /// <summary>
        /// Returns the columns for this response.
        /// </summary>
        public IEnumerable<Column> Columns
        {
            get { return columns; }
        }
    }
}