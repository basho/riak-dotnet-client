namespace Riak
{
    using System;

    /// <summary>
    /// Represents a result of an operation against Riak.
    /// </summary>
    public class Result
    {
        private readonly bool success;
        private readonly RError error;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="success"><b>true</b> if the result represents Success, <b>false</b> otherwise. Defaults to <b>true</b>.</param>
        public Result(bool success = true)
        {
            this.success = success;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="error">The <see cref="RError"/> object, if any. Defaults to <b>null</b>.</param>
        public Result(RError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            this.error = error;
            this.success = false;
        }

        /// <summary>
        /// <b>true</b> if the Riak operation was a success, otherwise, <b>false</b>.
        /// </summary>
        public bool Success
        {
            get { return success; }
        }

        /// <summary>
        /// The error returned from the Riak operation, in the case that the operation was not a success.
        /// </summary>
        public RError Error
        {
            get { return error; }
        }
    }
}
