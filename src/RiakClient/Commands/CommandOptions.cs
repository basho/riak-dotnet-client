namespace RiakClient.Commands
{
    using System;

    /// <summary>
    /// Base class for all Riak command options.
    /// </summary>
    public abstract class CommandOptions : IEquatable<CommandOptions>
    {
        private TimeSpan timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOptions"/> class.
        /// </summary>
        protected CommandOptions()
            : this(default(TimeSpan))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOptions"/> class.
        /// </summary>
        /// <param name="timeout">The command timeout in Riak.</param>
        protected CommandOptions(TimeSpan timeout)
        {
            this.timeout = timeout;
        }

        /// <summary>
        /// The timeout for this command.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return timeout; }
            set { timeout = value; } // TODO 3.0 allow setter?
        }

        public bool Equals(CommandOptions other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return GetHashCode() == other.GetHashCode();
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
            return timeout.GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Args"/> class.
        /// </summary>
        /// <param name="bucketType">The bucket type in Riak. Default is "default"</param>
        /// <param name="bucket">The bucket in Riak.</param>
        /// <param name="bucketRequired">Set to <b>true</b> if bucket is required.</param>
        /// <param name="key">The key in Riak.</param>
        /// <param name="keyRequired">If <b>true</b> and no key given, an exception is thrown.</param>
        public class Args
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
