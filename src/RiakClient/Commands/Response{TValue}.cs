namespace RiakClient.Commands
{
    /// <summary>
    /// Response to a Riak command with a return value.
    /// </summary>
    /// <typeparam name="TValue">The type of the Riak response.</typeparam>
    public class Response<TValue> : Response
    {
        private readonly TValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TValue}"/> class.
        /// </summary>
        /// <param name="notFound">Set to <b>true</b> to indicate the item was not found.</param>
        public Response(bool notFound)
            : base(notFound)
        {
            value = default(TValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TValue}"/> class.
        /// </summary>
        /// <param name="value">The response data.</param>
        public Response(TValue value)
        {
            this.value = value;
        }

        /// <summary>
        /// The value returned from Riak
        /// </summary>
        /// <value>The value returned from Riak, deserialized.</value>
        public TValue Value
        {
            get { return value; }
        }
    }
}
