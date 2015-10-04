namespace Riak
{
    using System;

    /// <summary>
    /// Represents a result of an operation against Riak.
    /// </summary>
    public class ExecuteResult
    {
        private readonly bool executed;
        private readonly RError error;
        private readonly Exception exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteResult"/> class.
        /// </summary>
        /// <param name="executed"><b>true</b> if the result represents an operation that executed on Riak, <b>false</b> otherwise. Defaults to <b>true</b>.</param>
        public ExecuteResult(bool executed = true)
        {
            this.executed = executed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteResult"/> class.
        /// </summary>
        /// <param name="error">The <see cref="RError"/> object, if any. Defaults to <b>null</b>.</param>
        public ExecuteResult(RError error)
            : this(true)
        {
            this.error = error;
            if (this.error == null)
            {
                throw new ArgumentNullException("error");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteResult"/> class.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> object.</param>
        public ExecuteResult(Exception exception)
            : this(false) // TODO 3.0 is this always false in this Exception case?
        {
            this.exception = exception;
            if (this.exception == null)
            {
                throw new ArgumentNullException("exception");
            }
        }

        /// <summary>
        /// <b>true</b> if the operation executed on Riak, otherwise <b>false</b>.
        /// </summary>
        public bool Executed
        {
            get { return executed; }
        }

        /// <summary>
        /// The error returned from the Riak operation, in the case that the operation was not a success.
        /// </summary>
        public RError Error
        {
            get { return error; }
        }

        /// <summary>
        /// The <see cref="Exception"/> returned from the Riak operation, in the case that the operation was not a success.
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
        }
    }
}
