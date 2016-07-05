namespace RiakClient.Commands.TS
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents options for a <see cref="Store"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class StoreOptions : TimeseriesCommandOptions
    {
        private ICollection<Column> columns;
        private ICollection<Row> rows;

        /// <inheritdoc/>
        public StoreOptions(string table)
            : base(table)
        {
        }

        /// <summary>
        /// The columns corresponding to the rows
        /// </summary>
        public ICollection<Column> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        /// <summary>
        /// The rows to store in Riak TS
        /// </summary>
        public ICollection<Row> Rows
        {
            get { return rows; }
            set { rows = value; }
        }
    }
}
