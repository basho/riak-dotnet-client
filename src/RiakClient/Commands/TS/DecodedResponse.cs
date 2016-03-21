namespace RiakClient.Commands.TS
{
    using System.Collections.Generic;

    internal class DecodedResponse
    {
        private readonly IEnumerable<Column> columns;
        private readonly IEnumerable<Row> rows;

        public DecodedResponse(IEnumerable<Column> columns, IEnumerable<Row> rows)
        {
            this.columns = columns;
            this.rows = rows;
        }

        public IEnumerable<Column> Columns
        {
            get { return columns; }
        }

        public IEnumerable<Row> Rows
        {
            get { return rows; }
        }
    }
}
