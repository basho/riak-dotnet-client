namespace Riak.Core
{
    using System;

    /// <summary>
    /// Represents an error when a connection can't write to its underlying NetworkStream / Socket
    /// </summary>
    [Serializable]
    public class ConnectionWriteException : ConnectionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionWriteException"/> class.
        /// </summary>
        public ConnectionWriteException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionWriteException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public ConnectionWriteException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionWriteException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ConnectionWriteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
