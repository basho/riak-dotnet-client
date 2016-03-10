namespace RiakClient.Commands.TS
{
    using System;

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
        /// Set to a function to receive streamed data.
        /// </summary>
        public Action<QueryResponse> Callback
        {
            get; set;
        }
    }
}
