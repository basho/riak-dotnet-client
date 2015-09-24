namespace Riak
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    // http://stackoverflow.com/questions/94488/what-is-the-correct-way-to-make-a-custom-net-exception-serializable

    /// <summary>
    /// Represents an error that occurred during execution.
    /// </summary>
    [Serializable]
    public class RException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RException"/> class.
        /// </summary>
        public RException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public RException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public RException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected RException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}