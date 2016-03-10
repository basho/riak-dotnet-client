namespace RiakClient.Commands.TS
{
    /// <inheritdoc/>
    public class QueryOptions : TimeseriesCommandOptions
    {
        /// <inheritdoc/>
        public QueryOptions(string table)
            : base(table)
        {
        }

        /// <summary>
        /// The query for Riak TS
        /// </summary>
        public RiakString Query
        {
            get; set;
        }
    }
}
