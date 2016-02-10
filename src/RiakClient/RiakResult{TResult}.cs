namespace RiakClient
{
    using System;

    /// <summary>
    /// Represents the collection of result information for a Riak operation that 
    /// returns a <typeparamref name="TResult"/>-typed value.     
    /// </summary>
    /// <typeparam name="TResult">The type of the Riak operation's return value.</typeparam>
    public class RiakResult<TResult> : RiakResult
    {
        private readonly TResult value;

        public RiakResult(TResult value, bool isSuccess, string errorMessage = null, ResultCode resultCode = ResultCode.Success)
            : base(isSuccess, resultCode, null, errorMessage)
        {
            this.value = value;
        }

        public RiakResult(Exception exception, ResultCode resultCode)
            : base(false, resultCode, exception, null)
        {
            this.value = default(TResult);
        }

        public RiakResult(RiakResult result)
            : base(result.IsSuccess, result.ResultCode, result.Exception, result.ErrorMessage)
        {
            this.value = default(TResult);
        }

        /// <summary>
        /// The return value from the Riak operation.
        /// </summary>
        public TResult Value
        {
            get { return value; }
        }

        /// <summary>Is the current paginated query / streaming query done?</summary>
        /// <remarks>Valid for Riak 1.4+ only.</remarks>
        public bool? Done { get; protected set; }

        /// <summary>
        /// An opaque continuation returned if there are still additional 
        /// results to be returned in a paginated query. This value should
        /// be supplied to the next query issued to Riak.
        /// </summary>
        /// <remarks>Valid for Riak 1.4 and newer only.</remarks>
        public string Continuation { get; protected set; }

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses a combination of the public properties to calculate the hash.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = Value != null ? Value.GetHashCode() : 0;
                result = (result * 397) ^ IsSuccess.GetHashCode();
                result = (result * 397) ^ ResultCode.GetHashCode();
                result = (result * 397) ^ NodeOffline.GetHashCode();
                result = (result * 397) ^ Done.GetHashCode();
                result = (result * 397) ^ Continuation.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as RiakResult<TResult>);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public bool Equals(RiakResult<TResult> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.Value, Value)
                && Equals(other.IsSuccess, IsSuccess)
                && Equals(other.ResultCode, ResultCode)
                && Equals(other.NodeOffline, NodeOffline)
                && Equals(other.Continuation, Continuation)
                && (other.Done.HasValue
                    && Done.HasValue
                    && Equals(other.Done.Value && Done.Value));
        }

        internal static RiakResult<TResult> Success(TResult value)
        {
            return new RiakResult<TResult>(value, true);
        }

        internal static new RiakResult<TResult> FromError(ResultCode code, string message, bool nodeOffline)
        {
            var riakResult = new RiakResult<TResult>(default(TResult), false, message, code);
            riakResult.NodeOffline = nodeOffline;
            return riakResult;
        }

        internal static new RiakResult<TResult> FromException(ResultCode code, Exception exception, bool nodeOffline)
        {
            var riakResult = new RiakResult<TResult>(exception, code);
            riakResult.NodeOffline = nodeOffline;
            return riakResult;
        }

        internal RiakResult SetDone(bool? value)
        {
            Done = value;
            return this;
        }

        internal RiakResult SetContinuation(string value)
        {
            Continuation = value;
            return this;
        }
    }
}
