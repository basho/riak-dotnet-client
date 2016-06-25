namespace RiakClient.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    // http://stackoverflow.com/questions/94488/what-is-the-correct-way-to-make-a-custom-net-exception-serializable

    /// <summary>
    /// Represents an error that occurred during execution.
    /// </summary>
    [Serializable]
    public class RiakException : Exception
    {
        private readonly int errorCode;
        private readonly bool nodeOffline;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakException"/> class.
        /// </summary>
        public RiakException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public RiakException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public RiakException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakException"/> class.
        /// </summary>
        /// <param name="errorCode">An error code returned from a Riak operation.</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="nodeOffline">A flag to mark if the node was offline or unreachable at the time of the error.</param>
        public RiakException(int errorCode, string message, bool nodeOffline)
            : this(string.Format("Riak returned an error. Code '{0}'. Message: '{1}'", errorCode, message))
        {
            this.nodeOffline = nodeOffline;
            this.errorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="nodeOffline">A flag to mark if the node was offline or unreachable at the time of the error.</param>
        public RiakException(string message, bool nodeOffline)
            : this(message)
        {
            this.nodeOffline = nodeOffline;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected RiakException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.errorCode = info.GetInt32("ErrorCode");
            this.nodeOffline = info.GetBoolean("NodeOffline");
        }

        /// <summary>
        /// An error code returned from a Riak operation.
        /// </summary>
        public int ErrorCode
        {
            get { return errorCode; }
        }

        /// <summary>
        /// A flag to mark if the node was offline or unreachable at the time of the error.
        /// </summary>
        public bool NodeOffline
        {
            get { return nodeOffline; }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="info"/>' cannot be null. </exception>
        /// <exception cref="SerializationException">A value has already been associated with '<paramref name="name" />'.</exception>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("ErrorCode", errorCode);
            info.AddValue("NodeOffline", nodeOffline);

            base.GetObjectData(info, context);
        }
    }
}
