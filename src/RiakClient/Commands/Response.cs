namespace RiakClient.Commands
{
    /// <summary>
    /// Response to a Riak command.
    /// </summary>
    public class Response
    {
        private readonly bool notFound = false; // TODO 3.0 should only be responses for commands where not found is possible?? maybe?

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        public Response()
        {
            notFound = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="notFound">Set to <b>true</b> to indicate the item was not found.</param>
        public Response(bool notFound)
        {
            this.notFound = notFound;
        }

        /// <summary>
        /// Will be set to <b>true</b> when the object does not exist in Riak.
        /// </summary>
        /// <value><b>false</b> when the object exists in Riak, <b>true</b> if the object does NOT exist.</value>
        public virtual bool NotFound
        {
            get { return notFound; }
        }
    }
}
