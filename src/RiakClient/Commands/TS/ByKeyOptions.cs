namespace RiakClient.Commands.TS
{
    /// <inheritdoc/>
    public class ByKeyOptions : TimeseriesCommandOptions
    {
        /// <inheritdoc/>
        public ByKeyOptions(string table)
            : base(table)
        {
        }

        /// <summary>
        /// The key in Riak TS
        /// </summary>
        public Row Key
        {
            get; set;
        }
    }
}
