namespace Riak.Core
{
    using System;

    /// <summary>
    /// Represents an error that occurs when a command executes on a Connection that is mid-execution of a previous command.
    /// </summary>
    [Serializable]
    public class ConnectionInFlightException : ConnectionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionInFlightException"/> class.
        /// </summary>
        public ConnectionInFlightException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionInFlightException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public ConnectionInFlightException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionInFlightException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ConnectionInFlightException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
