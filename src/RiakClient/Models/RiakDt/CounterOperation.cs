namespace RiakClient.Models.RiakDt
{
    using Messages;

    /// <summary>
    /// Represents an operation on a Riak Counter data type.
    /// </summary>
    [System.Obsolete("RiakDt is deprecated. Please use Commands/CRDT namespace.")]
    public class CounterOperation : IDtOp
    {
        private readonly long value;

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterOperation"/> class.
        /// </summary>
        /// <param name="value">The value to initialize the counter to.</param>
        public CounterOperation(long value)
        {
            this.value = value;
        }

        /// <summary>
        /// The value of the counter.
        /// </summary>
        public long Value
        {
            get { return value; }
        }

        /// <inheritdoc/>
        public DtOp ToDtOp()
        {
            return new DtOp
                {
                    counter_op = new CounterOp { increment = value }
                };
        }
    }
}
