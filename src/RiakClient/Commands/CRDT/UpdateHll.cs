namespace RiakClient.Commands.CRDT
{
    using System.Collections.Generic;
    using Extensions;
    using Messages;
    using Util;

    /// <summary>
    /// Command used to update a Hyperloglog in Riak. As a convenience, a builder method
    /// is provided as well as an object with a fluent API for constructing the
    /// update.
    /// See <see cref="UpdateHll.Builder"/>
    /// <code>
    /// var update = new UpdateHll.Builder()
    ///           .WithBucketType("hlls")
    ///           .WithBucket("myBucket")
    ///           .WithKey("hll_1")
    ///           .WithReturnBody(true)
    ///           .Build();
    /// </code>
    /// </summary>
    public class UpdateHll : UpdateCommand<HllResponse>
    {
        private readonly UpdateHllOptions hllOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateHll"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="UpdateHllOptions"/></param>
        /// <inheritdoc />
        public UpdateHll(UpdateHllOptions options)
            : base(options)
        {
            this.hllOptions = options;
        }

        protected override DtOp GetRequestOp()
        {
            var op = new DtOp();
            op.hll_op = new HllOp();

            if (EnumerableUtil.NotNullOrEmpty(hllOptions.Additions))
            {
                op.hll_op.adds.AddRange(hllOptions.Additions);
            }

            return op;
        }

        protected override HllResponse CreateResponse(DtUpdateResp response)
        {
            RiakString key = GetKey(CommandOptions.Key, response);
            return new HllResponse(key, response.hll_value);
        }

        public class Builder
            : UpdateCommandBuilder<UpdateHll.Builder, UpdateHll, UpdateHllOptions, HllResponse>
        {
            private ISet<byte[]> additions;

            public Builder()
            {
                WithIncludeContext(false);
            }

            public Builder(ISet<byte[]> additions) : this()
            {
                this.additions = additions;
            }

            public Builder(ISet<string> additions) : this()
            {
                this.additions = additions.GetUTF8Bytes();
            }

            public Builder(ISet<byte[]> additions, Builder source)
                : base(source)
            {
                this.additions = additions;
                WithIncludeContext(false);
            }

            public Builder(ISet<string> additions, Builder source)
                : base(source)
            {
                this.additions = additions.GetUTF8Bytes();
                WithIncludeContext(false);
            }

            public Builder WithAdditions(ISet<byte[]> additions)
            {
                this.additions = additions;
                return this;
            }

            public Builder WithAdditions(ISet<string> additions)
            {
                this.additions = additions.GetUTF8Bytes();
                return this;
            }

            protected override void PopulateOptions(UpdateHllOptions options)
            {
                options.Additions = additions;
            }
        }
    }
}