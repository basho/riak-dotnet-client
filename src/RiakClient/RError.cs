namespace Riak
{
    using System;
    using RiakClient;
    using RiakClient.Messages;

    /// <summary>
    /// Represents a result of an operation against Riak that returned an <see cref="RpbErrorResp"/>
    /// </summary>
    public class RError
    {
        private readonly RiakString message;
        private readonly int code;

        /// <summary>
        /// Initializes a new instance of the <see cref="RError"/> class.
        /// </summary>
        /// <param name="error">The PB error from which to create this <see cref="RError"/> instance.</param>
        [CLSCompliant(false)]
        public RError(RpbErrorResp error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            this.message = new RiakString(error.errmsg);
            this.code = (int)error.errcode;
        }

        /// <summary>
        /// Get the error message.
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Get the error code.
        /// </summary>
        public int Code
        {
            get { return code; }
        }
    }
}
