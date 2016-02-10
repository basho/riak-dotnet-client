namespace RiakClient.Commands
{
    using System;

    /// <summary>
    /// Response to a Riak command.
    /// </summary>
    public class Response
    {
        private readonly RiakString key;
        private readonly bool notFound; // TODO 3.0 should only be responses for commands where not found is possible?? maybe?

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class representing "Not Found".
        /// </summary>
        /// <param name="notFound">Set to <b>true</b> to indicate the item was not found.</param>
        public Response(bool notFound)
        {
            this.notFound = notFound;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="key">A <see cref="RiakString"/> representing the key.</param>
        public Response(RiakString key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key", "key is required!");
            }
            else
            {
                this.key = key;
            }

            this.notFound = false;
        }

        /// <summary>
        /// Will be set to <b>true</b> when the object does not exist in Riak.
        /// </summary>
        /// <value><b>false</b> when the object exists in Riak, <b>true</b> if the object does NOT exist.</value>
        public bool NotFound
        {
            get { return notFound; }
        }

        /// <summary>
        /// Returns the object's key in Riak. If Riak generates a key for you, it will be here.
        /// </summary>
        /// <value>A <see cref="RiakString"/> representing the key of the object in Riak.</value>
        public RiakString Key
        {
            get { return key; }
        }
    }
}
