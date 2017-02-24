namespace RiakClient.Commands.CRDT
{
    using System.Collections.Generic;
    using Extensions;
    using Messages;
    using Util;

    /// <summary>
    /// Command used to update a Set in Riak. As a convenience, a builder method
    /// is provided as well as an object with a fluent API for constructing the
    /// update.
    /// See <see cref="UpdateGSet.Builder"/>
    /// <code>
    /// var update = new UpdateGSet.Builder()
    ///           .WithBucketType("maps")
    ///           .WithBucket("myBucket")
    ///           .WithKey("map_1")
    ///           .WithReturnBody(true)
    ///           .Build();
    /// </code>
    /// </summary>
    public class UpdateGSet : UpdateSetBase
    {
        private readonly UpdateGSetOptions gsetOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateGSet"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="UpdateGSetOptions"/></param>
        /// <inheritdoc />
        public UpdateGSet(UpdateGSetOptions options)
            : base(options)
        {
            gsetOptions = options;
        }

        protected override DtOp GetRequestOp()
        {
            var op = new DtOp();
            op.gset_op = new GSetOp();

            if (EnumerableUtil.NotNullOrEmpty(gsetOptions.Additions))
            {
                op.gset_op.adds.AddRange(gsetOptions.Additions);
            }

            return op;
        }

        public class Builder
            : UpdateCommandBuilder<Builder, UpdateGSet, UpdateGSetOptions, SetResponse>
        {
            private ISet<byte[]> additions;

            public Builder()
            {
            }

            public Builder(ISet<byte[]> additions)
            {
                this.additions = additions;
            }

            public Builder(ISet<string> additions)
            {
                this.additions = additions.GetUTF8Bytes();
            }

            public Builder(ISet<byte[]> additions, Builder source)
                : base(source)
            {
                this.additions = additions;
            }

            public Builder(ISet<string> additions, Builder source)
                : base(source)
            {
                this.additions = additions.GetUTF8Bytes();
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

            protected override void PopulateOptions(UpdateGSetOptions options)
            {
                options.Additions = additions;
            }
        }
    }
}