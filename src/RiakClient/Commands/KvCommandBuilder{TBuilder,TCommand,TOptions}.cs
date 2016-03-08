namespace RiakClient.Commands
{
    using System;

    /// <summary>
    /// Base class for all Riak KV command builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder. Allows chaining.</typeparam>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TOptions">The type of the options for this command.</typeparam>
    public abstract class KvCommandBuilder<TBuilder, TCommand, TOptions>
        : CommandBuilder<TBuilder, TCommand, TOptions>
        where TBuilder : KvCommandBuilder<TBuilder, TCommand, TOptions>
        where TOptions : KvCommandOptions
    {
        protected string bucketType;
        protected string bucket;
        protected string key;

        public KvCommandBuilder()
        {
        }

        public KvCommandBuilder(KvCommandBuilder<TBuilder, TCommand, TOptions> source)
            : base(source)
        {
            this.bucketType = source.bucketType;
            this.bucket = source.bucket;
            this.key = source.key;
        }

        public TBuilder WithBucketType(string bucketType)
        {
            if (string.IsNullOrWhiteSpace(bucketType))
            {
                throw new ArgumentNullException("bucketType", "bucketType may not be null, empty or whitespace");
            }

            this.bucketType = bucketType;
            return (TBuilder)this;
        }

        public TBuilder WithBucket(string bucket)
        {
            if (string.IsNullOrWhiteSpace(bucket))
            {
                throw new ArgumentNullException("bucket", "bucket may not be null, empty or whitespace");
            }

            this.bucket = bucket;
            return (TBuilder)this;
        }

        public TBuilder WithKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", "key may not be null, empty or whitespace");
            }

            this.key = key;
            return (TBuilder)this;
        }

        protected override TOptions BuildOptions()
        {
            return (TOptions)Activator.CreateInstance(typeof(TOptions), bucketType, bucket, key);
        }
    }
}
