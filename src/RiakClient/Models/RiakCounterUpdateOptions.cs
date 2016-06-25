namespace RiakClient.Models
{
    using Messages;

    /// <summary>
    /// A collection of options that can be modified when updating a 1.4-style counter in Riak.
    /// </summary>
    public class RiakCounterUpdateOptions : RiakOptions<RiakCounterUpdateOptions>
    {
        /// <summary>
        /// Whether or not the updated value should be returned from the counter
        /// </summary>
        public bool? ReturnValue { get; private set; }

        /// <summary>
        /// A fluent setter for the <see cref="ReturnValue"/> property.
        /// Sets whether or not the updated value should be returned from the counter.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public RiakCounterUpdateOptions SetReturnValue(bool value)
        {
            ReturnValue = value;
            return this;
        }

        internal void Populate(RpbCounterUpdateReq request)
        {
            if (W != null)
            {
                request.w = W;
            }

            if (Dw != null)
            {
                request.dw = Dw;
            }

            if (Pw != null)
            {
                request.pw = Pw;
            }

            if (ReturnValue.HasValue)
            {
                request.returnvalue = ReturnValue.Value;
            }
        }
    }
}
