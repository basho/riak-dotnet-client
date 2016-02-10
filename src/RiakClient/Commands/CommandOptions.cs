namespace RiakClient.Commands
{
    using System;

    /// <summary>
    /// Base class for all Riak command options.
    /// </summary>
    public abstract class CommandOptions : IEquatable<CommandOptions>
    {
        private readonly RiakString bucketType;
        private readonly RiakString bucket;
        private readonly RiakString key;
        private TimeSpan timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOptions"/> class.
        /// </summary>
        /// <param name="args">Arguments to this ctor. Required.</param>
        protected CommandOptions(Args args)
            : this(args, Riak.Constants.DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOptions"/> class.
        /// </summary>
        /// <param name="args">Arguments to this ctor. Required.</param>
        /// <param name="timeout">The command timeout in Riak.</param>
        protected CommandOptions(Args args, TimeSpan timeout)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            this.bucketType = args.BucketType;
            this.bucket = args.Bucket;
            this.key = args.Key;

            this.timeout = timeout;
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

        /// <summary>
        /// The timeout for this command.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public bool Equals(CommandOptions other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CommandOptions);
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
                result = (result * 397) ^ timeout.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Args"/> class.
        /// </summary>
        /// <param name="bucketType">The bucket type in Riak. Default is "default"</param>
        /// <param name="bucket">The bucket in Riak.</param>
        /// <param name="bucketRequired">Set to <b>true</b> if bucket is required.</param>
        /// <param name="key">The key in Riak.</param>
        /// <param name="keyRequired">If <b>true</b> and no key given, an exception is thrown.</param>
        protected class Args
        {
            private readonly RiakString bucketType;
            private readonly RiakString bucket;
            private readonly RiakString key;

            public Args(
                string bucketType,
                string bucket,
                bool bucketRequired,
                string key,
                bool keyRequired)
            {
                if (string.IsNullOrWhiteSpace(bucketType))
                {
                    // TODO 3.0 constant somewhere
                    this.bucketType = new RiakString("default");
                }
                else
                {
                    this.bucketType = new RiakString(bucketType);
                }

                if (bucketRequired && string.IsNullOrWhiteSpace(bucket))
                {
                    throw new ArgumentNullException("bucket");
                }

                this.bucket = new RiakString(bucket);

                if (keyRequired && string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException("key");
                }

                this.key = new RiakString(key);
            }

            public RiakString BucketType
            {
                get { return bucketType; }
            }

            public RiakString Bucket
            {
                get { return bucket; }
            }

            public RiakString Key
            {
                get { return key; }
            }
        }
    }
}
