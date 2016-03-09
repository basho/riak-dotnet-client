namespace RiakClient.Commands.TS
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents options for a <see cref="Store"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class StoreOptions : TimeseriesCommandOptions
    {
        private IEnumerable<Column> columns;
        private IEnumerable<Row> rows;

        /// <inheritdoc/>
        public StoreOptions(string table)
            : base(table)
        {
        }

        /// <summary>
        /// The columns corresponding to the rows
        /// </summary>
        public IEnumerable<Column> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        /// <summary>
        /// The rows to store in Riak TS
        /// </summary>
        public IEnumerable<Row> Rows
        {
            get { return rows; }
            set { rows = value; }
        }
    }
}
