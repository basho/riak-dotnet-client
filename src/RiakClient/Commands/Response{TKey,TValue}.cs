namespace RiakClient.Commands
{
    /// <summary>
    /// Response to a Riak command with a return value.
    /// </summary>
    /// <typeparam name="TKey">The type of the Riak request key.</typeparam>
    /// <typeparam name="TValue">The type of the Riak response.</typeparam>
    public abstract class Response<TKey, TValue> : Response<TValue>
    {
        private readonly TKey key;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="notFound">Set to <b>true</b> to indicate the item was not found.</param>
        public Response(bool notFound)
            : base(notFound)
        {
            key = default(TKey);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="key">The key corresponding to the request.</param>
        /// <param name="value">The response data.</param>
        public Response(TKey key, TValue value)
            : base(value)
        {
            this.key = key;
        }

        /// <summary>
        /// The key corresponding to the request.
        /// </summary>
        /// <value>The key.</value>
        public TKey Key
        {
            get { return key; }
        }
    }
}
