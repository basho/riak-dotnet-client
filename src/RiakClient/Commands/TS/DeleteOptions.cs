namespace RiakClient.Commands.TS
{
    /// <summary>
    /// Represents options for a <see cref="Delete"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class DeleteOptions : TimeseriesCommandOptions
    {
        /// <inheritdoc/>
        public DeleteOptions(string table)
            : base(table)
        {
        }

        /// <summary>
        /// The key to delete from Riak TS
        /// </summary>
        public Row Key
        {
            get; set;
        }
    }
}
