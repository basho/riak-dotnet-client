namespace RiakClient
{
    using System;

    /// <summary>
    /// Represents the collection of result information for a Riak operation that has no specific return value.
    /// </summary>
    public class RiakResult
    {
        private readonly bool isSuccess;

        private readonly string errorMessage;
        private readonly Exception exception;

        private readonly ResultCode resultCode;
        private readonly bool nodeOffline = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakResult"/> class.
        /// </summary>
        /// <param name="isSuccess"><b>true</b> if the result represents Success, <b>false</b> otherwise. Defaults to <b>true</b>.</param>
        /// <param name="errorMessage">The error message, if any. Defaults to <b>null</b>.</param>
        /// <param name="resultCode">The <see cref="ResultCode"/>. Defaults to <b>ResultCode.Success</b>.</param>
        public RiakResult(bool isSuccess = true, string errorMessage = null, ResultCode resultCode = ResultCode.Success)
            : this(isSuccess, resultCode, null, errorMessage, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakResult"/> class.
        /// </summary>
        /// <param name="resultCode">The <see cref="ResultCode"/>.</param>
        /// <param name="exception">The <see cref="System.Exception"/>. Required.</param>
        public RiakResult(ResultCode resultCode, Exception exception)
            : this(false, resultCode, exception, null, false)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
        }

        protected RiakResult(bool isSuccess, ResultCode resultCode, Exception exception, string errorMessage, bool nodeOffline)
        {
            this.isSuccess = isSuccess;
            this.resultCode = resultCode;
            this.exception = exception;
            this.errorMessage = errorMessage;
            this.nodeOffline = nodeOffline;

            if (string.IsNullOrWhiteSpace(this.errorMessage) &&
                exception != null &&
                !string.IsNullOrWhiteSpace(exception.Message))
            {
                this.errorMessage = exception.Message;
            }
        }

        /// <summary>
        /// <b>true</b> if the Riak operation was a success, otherwise, <b>false</b>.
        /// </summary>
        public bool IsSuccess
        {
            get { return isSuccess; }
        }

        /// <summary>
        /// The error message returned from the Riak operation, in the case that the operation was not a success.
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        /// <summary>
        /// The <see cref="System.Exception"/> returned from the Riak operation.
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
        }

        /// <summary>
        /// The <see cref="ResultCode"/> returned from the operation.
        /// </summary>
        public ResultCode ResultCode
        {
            get { return resultCode; }
        }

        internal bool NodeOffline
        {
            get { return nodeOffline; }
        }

        internal static RiakResult Success()
        {
            return new RiakResult();
        }

        internal static RiakResult FromError(ResultCode code, string message, bool nodeOffline)
        {
            return new RiakResult(false, code, null, message, nodeOffline);
        }

        internal static RiakResult FromException(ResultCode code, Exception ex, bool nodeOffline)
        {
            return new RiakResult(false, code, ex, null, nodeOffline);
        }
    }
}
