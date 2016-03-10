namespace RiakClient.Commands.TS
{
    using System.Collections.Generic;
    using Util;

    /// <summary>
    /// Response to a <see cref="Query"/> command.
    /// </summary>
    public class QueryResponse : Response<string, IEnumerable<Row>>
    {
        private readonly IEnumerable<Column> columns;

        /// <inheritdoc />
        public QueryResponse()
            : base(notFound: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResponse"/> class.
        /// </summary>
        /// <param name="query">A <see cref="string"/> representing query.</param>
        /// <param name="columns">The columns for the fetched TS data.</param>
        /// <param name="values">The rows for the fetched TS data.</param>
        public QueryResponse(string query, IEnumerable<Column> columns, IEnumerable<Row> values)
            : base(query, values)
        {
            if (EnumerableUtil.IsNullOrEmpty(values))
            {
                isNotFound = true;
            }

            this.columns = columns;
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