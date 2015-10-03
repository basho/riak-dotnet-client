namespace RiakClient.Commands.CRDT
{
    /// <summary>
    /// Response to a <see cref="FetchCounter"/> or <see cref="UpdateCounter"/> command.
    /// </summary>
    public class CounterResponse : DataTypeResponse<long>
    {
        /// <inheritdoc />
        public CounterResponse()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterResponse"/> class.
        /// </summary>
        /// <param name="key">A <see cref="RiakString"/> representing the key.</param>
        /// <param name="context">The data type context. Necessary to use this if updating a data type with removals.</param>
        /// <param name="value">The value of the fetched CRDT counter.</param>
        public CounterResponse(RiakString key, byte[] context, long value)
            : base(key, context, value)
        {
        }
    }
}
