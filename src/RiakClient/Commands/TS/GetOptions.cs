namespace RiakClient.Commands.TS
{
    /// <summary>
    /// Represents options for a <see cref="Get"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class GetOptions : TimeseriesCommandOptions
    {
        /// <inheritdoc/>
        public GetOptions(string table)
            : base(table)
        {
        }

        /// <summary>
        /// The key to fetch from Riak TS
        /// </summary>
        public Row Key
        {
            get; set;
        }
    }
}
