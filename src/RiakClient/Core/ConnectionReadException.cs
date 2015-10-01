namespace Riak.Core
{
    using System;

    /// <summary>
    /// Represents an error when a connection can't read from it's underlying NetworkStream / Socket
    /// </summary>
    [Serializable]
    public class ConnectionReadException : ConnectionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionReadException"/> class.
        /// </summary>
        public ConnectionReadException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionReadException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public ConnectionReadException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionReadException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ConnectionReadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}