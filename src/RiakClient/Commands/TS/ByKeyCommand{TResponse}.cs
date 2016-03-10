namespace RiakClient.Commands.TS
{
    using System;
    using Messages;
    using Util;

    /// <summary>
    /// Operates on Riak TS data by key
    /// </summary>
    /// <typeparam name="TResponse">The type of the response data from Riak.</typeparam>
    [CLSCompliant(false)]
    public abstract class ByKeyCommand<TResponse> : Command<ByKeyOptions, TResponse>
        where TResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByKeyCommand{TResponse}"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="ByKeyOptions"/></param>
        public ByKeyCommand(ByKeyOptions options)
            : base(options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (options.Key == null)
            {
                throw new ArgumentNullException("options.Key", "options.Key can not be null");
            }

            if (EnumerableUtil.IsNullOrEmpty(options.Key.Cells))
            {
                throw new ArgumentNullException("options.Key.Cells", "options.Key.Cells can not be null or empty");
            }
        }

        public override RpbReq ConstructPbRequest()
        {
            ITsByKeyReq req = GetByKeyReq();

            req.table = CommandOptions.Table;

            req.timeoutSpecified = false;
            if (CommandOptions.Timeout.HasValue)
            {
                req.timeout = (uint)CommandOptions.Timeout;
            }

            req.key.AddRange(CommandOptions.Key.ToTsCells());

            return (RpbReq)req;
        }

        protected abstract ITsByKeyReq GetByKeyReq();

        /// <inheritdoc />
        public abstract class Builder<TCommand>
            : TimeseriesCommandBuilder<Builder<TCommand>, TCommand, ByKeyOptions>
        {
            private Row key;

            public Builder<TCommand> WithKey(Row key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key", "key is required");
                }

                this.key = key;
                return this;
            }

            protected override void PopulateOptions(ByKeyOptions options)
            {
                options.Key = key;
                options.Timeout = timeout;
            }
        }
    }
}
