namespace RiakClient.Commands.TS
{
    using System;

    /// <inheritdoc/>
    public class ListKeysOptions : TimeseriesCommandOptions
    {
        /// <inheritdoc/>
        public ListKeysOptions(string table)
            : base(table)
        {
        }

        /// <summary>
        /// Set to a function to receive streamed data.
        /// </summary>
        public Action<ListKeysResponse> Callback
        {
            get; set;
        }
    }
}
