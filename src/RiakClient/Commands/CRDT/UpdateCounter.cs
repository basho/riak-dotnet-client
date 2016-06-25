namespace RiakClient.Commands.CRDT
{
    using Messages;

    /// <summary>
    /// Command used to update a Counter in Riak. As a convenience, a builder method
    /// is provided as well as an object with a fluent API for constructing the
    /// update.
    /// See <see cref="UpdateCounter.Builder"/>
    /// <code>
    /// var update = new UpdateCounter.Builder(10)
    ///           .WithBucketType("maps")
    ///           .WithBucket("myBucket")
    ///           .WithKey("map_1")
    ///           .WithReturnBody(true)
    ///           .Build();
    /// </code>
    /// </summary>
    public class UpdateCounter : UpdateCommand<CounterResponse>
    {
        private readonly UpdateCounterOptions counterOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCounter"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="UpdateCounterOptions"/></param>
        /// <inheritdoc />
        public UpdateCounter(UpdateCounterOptions options)
            : base(options)
        {
            this.counterOptions = options;
        }

        protected override DtOp GetRequestOp()
        {
            var op = new DtOp();
            op.counter_op = new CounterOp();
            op.counter_op.increment = counterOptions.Increment;
            return op;
        }

        protected override CounterResponse CreateResponse(DtUpdateResp response)
        {
            RiakString key = GetKey(CommandOptions.Key, response);
            return new CounterResponse(key, response.context, response.counter_value);
        }

        public class Builder :
            UpdateCommandBuilder<Builder, UpdateCounter, UpdateCounterOptions, CounterResponse>
        {
            private long increment;

            public Builder()
            {
            }

            public Builder(long increment)
            {
                this.increment = increment;
            }

            public Builder(long increment, Builder source)
                : base(source)
            {
                this.increment = increment;
            }

            public Builder WithIncrement(long increment)
            {
                this.increment = increment;
                return this;
            }

            protected override void PopulateOptions(UpdateCounterOptions options)
            {
                options.Increment = increment;
            }
        }
    }
}
