namespace RiakClient.Exceptions
{
    using System;

    /// <summary>
    /// Represents an error that is raised when conflicting Riak features are used together.
    /// </summary>
    [Serializable]
    public class RiakUnsupportedException : RiakException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakUnsupportedException"/> class.
        /// </summary>
        public RiakUnsupportedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakUnsupportedException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public RiakUnsupportedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakUnsupportedException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public RiakUnsupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
