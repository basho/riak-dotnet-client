namespace RiakClient.Commands
{
    /// <summary>
    /// Response to a Riak command.
    /// </summary>
    public class Response
    {
        protected bool isNotFound;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class representing "Not Found".
        /// </summary>
        /// <param name="notFound">Set to <b>true</b> to indicate the item was not found.</param>
        public Response(bool notFound)
        {
            isNotFound = notFound;
        }

        /// <summary>
        /// Will be set to <b>true</b> when the object does not exist in Riak.
        /// </summary>
        /// <value><b>false</b> when the object exists in Riak, <b>true</b> if the object does NOT exist.</value>
        public bool NotFound
        {
            get { return isNotFound; }
        }
    }
}
