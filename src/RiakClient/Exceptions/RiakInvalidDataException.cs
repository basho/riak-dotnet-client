namespace RiakClient.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// Represents an error that occurs when an invalid message code is read from a Riak response.
    /// </summary>
    [Serializable]
    public class RiakInvalidDataException : RiakException
    {
        private readonly byte messageCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakInvalidDataException"/> class.
        /// </summary>
        public RiakInvalidDataException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakInvalidDataException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public RiakInvalidDataException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakInvalidDataException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public RiakInvalidDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakInvalidDataException"/> class.
        /// </summary>
        /// <param name="messageCode">The invalid message code that was read from the Riak response.</param>
        public RiakInvalidDataException(byte messageCode)
            : this(string.Format("Unexpected message code: {0}", messageCode))
        {
            this.messageCode = messageCode;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected RiakInvalidDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.messageCode = info.GetByte("MessageCode");
        }

        /// <summary>
        /// The invalid message code that was read from the Riak response.
        /// </summary>
        public byte MessageCode
        {
            get
            {
                return messageCode;
            }
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

            info.AddValue("MessageCode", messageCode);

            base.GetObjectData(info, context);
        }
    }
}
