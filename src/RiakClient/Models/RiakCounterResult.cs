namespace RiakClient.Models
{
    /// <summary>
    /// Represents the data returned from a counter operation.
    /// </summary>
    public class RiakCounterResult
    {
        private readonly RiakResult<RiakObject> result;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakCounterResult"/> class.
        /// </summary>
        /// <param name="result">The <see cref="RiakResult{T}"/> to populate the new <see cref="RiakCounterResult"/> from.</param>
        public RiakCounterResult(RiakResult<RiakObject> result)
        {
            this.result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakCounterResult"/> class.
        /// </summary>
        /// <param name="result">The <see cref="RiakResult{T}"/> to populate the new <see cref="RiakCounterResult"/> from.</param>
        /// <param name="value">The current value of the counter.</param>
        public RiakCounterResult(RiakResult<RiakObject> result, long? value) : this(result)
        {
            this.Value = value;
        }

        /// <summary>
        /// The <see cref="RiakResult{T}"/> of the counter operation.
        /// </summary>
        public RiakResult<RiakObject> Result
        {
            get { return result; }
        }

        /// <summary>
        /// The value of the counter (if the operation returned it).
        /// </summary>
        public long? Value
        {
            get; set;
        }
    }
}
