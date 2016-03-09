namespace RiakClient.Commands.TS
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents options for a <see cref="Put"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class PutOptions : TimeseriesCommandOptions
    {
        private IEnumerable<Row> rows;

        /// <inheritdoc/>
        public PutOptions(string table)
            : base(table)
        {
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
