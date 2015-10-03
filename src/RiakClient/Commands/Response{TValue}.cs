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
        /// <param name="value">The response data.</param>
        public Response(TValue value)
            : this(false, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TValue}"/> class.
        /// </summary>
        /// <param name="notFound">Set to <b>true</b> to indicate the item was not found.</param>
        public Response(bool notFound)
            : base(notFound)
        {
            this.value = default(TValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TValue}"/> class.
        /// </summary>
        /// <param name="notFound">Set to <b>true</b> to indicate the item was not found.</param>
        /// <param name="value">The response data.</param>
        public Response(bool notFound, TValue value)
            : base(notFound)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TValue}"/> class.
        /// </summary>
        /// <param name="key">A <see cref="RiakString"/> representing the key.</param>
        /// <param name="value">The response data.</param>
        public Response(RiakString key, TValue value)
            : base(key)
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
