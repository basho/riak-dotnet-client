namespace RiakClient.Commands
{
    using System;

    /// <summary>
    /// Base class for all Riak KV command options.
    /// </summary>
    public abstract class KvCommandOptions : CommandOptions, IEquatable<KvCommandOptions>
    {
        private readonly RiakString bucketType;
        private readonly RiakString bucket;
        private readonly RiakString key;

        /// <summary>
        /// Initializes a new instance of the <see cref="KvCommandOptions"/> class.
        /// </summary>
        /// <param name="args">The command args</param>
        public KvCommandOptions(Args args)
            : this(args, Riak.Constants.DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KvCommandOptions"/> class.
        /// </summary>
        /// <param name="args">The command args</param>
        /// <param name="timeout">The command's timeout</param>
        public KvCommandOptions(Args args, TimeSpan timeout)
            : base(timeout)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            bucketType = args.BucketType;
            bucket = args.Bucket;
            key = args.Key;
        }

        /// <summary>
        /// The bucket type
        /// </summary>
        /// <value>A <see cref="RiakString"/> representing the bucket type.</value>
        public RiakString BucketType
        {
            get { return bucketType; }
        }

        /// <summary>
        /// The bucket
        /// </summary>
        /// <value>A <see cref="RiakString"/> representing the bucket.</value>
        public RiakString Bucket
        {
            get { return bucket; }
        }

        /// <summary>
        /// The key
        /// </summary>
        /// <value>The <see cref="RiakString"/> representing the key.</value>
        public RiakString Key
        {
            get { return key; }
        }

        public bool Equals(KvCommandOptions other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as KvCommandOptions);
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses a combination of the public properties to generate a unique hash code.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = bucketType.GetHashCode();
                result = (result * 397) ^ bucket.GetHashCode();
                result = (result * 397) ^ (key != null ? key.GetHashCode() : 0);
                result = (result * 397) ^ base.GetHashCode();
                return result;
            }
        }
    }
}
