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

        /// <summary>
        /// Set to <b>true</b> to stream the response.
        /// </summary>
        public bool Streaming
        {
            get; set;
        }
    }
}
