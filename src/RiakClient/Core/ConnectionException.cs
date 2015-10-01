namespace Riak.Core
{
    using System;

    /// <summary>
    /// Represents an exception related to a Connection
    /// TODO 3.0 - temporary vs permanent exceptions?
    /// </summary>
    [Serializable]
    public class ConnectionException : RException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class.
        /// </summary>
        public ConnectionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public ConnectionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
