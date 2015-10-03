namespace RiakClient
{
    /// <summary>
    /// Riak operation result codes.
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// The operation was successful.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Cluster was shutting down.
        /// </summary>
        ShuttingDown,

        /// <summary>
        /// The requested riak object was not found.
        /// </summary>
        NotFound,

        /// <summary>
        /// Communication error with the cluster.
        /// </summary>
        CommunicationError,

        /// <summary>
        /// An invalid response was received.
        /// </summary>
        InvalidResponse,

        /// <summary>
        /// The cluster is offline.
        /// </summary>
        ClusterOffline,

        /// <summary>
        /// No available connections to make a request.
        /// </summary>
        NoConnections,

        /// <summary>
        /// An exception occurred during a batch operation.
        /// </summary>
        BatchException,

        /// <summary>
        /// The client ran out of retry attempts while trying to process the request.
        /// </summary>
        NoRetries,

        /// <summary>
        /// An HTTP error occurred.
        /// </summary>
        HttpError,

        /// <summary>
        /// An invalid request was performed.
        /// </summary>
        InvalidRequest
    }
}
