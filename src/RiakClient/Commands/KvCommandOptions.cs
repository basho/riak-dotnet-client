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
        /// <param name="bucketType">The bucket type in Riak. Required.</param>
        /// <param name="bucket">The bucket in Riak. Required.</param>
        /// <param name="key">The key in Riak.</param>
        /// <param name="keyIsRequired">If <b>true</b> and no key given, an exception is thrown.</param>
        public KvCommandOptions(
            string bucketType,
            string bucket,
            string key,
            bool keyIsRequired)
            : this(bucketType, bucket, key, keyIsRequired, CommandDefaults.Timeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KvCommandOptions"/> class.
        /// </summary>
        /// <param name="bucketType">The bucket type in Riak. Required.</param>
        /// <param name="bucket">The bucket in Riak. Required.</param>
        /// <param name="key">The key in Riak.</param>
        /// <param name="keyIsRequired">If <b>true</b> and no key given, an exception is thrown.</param>
        /// <param name="timeout">The command timeout in Riak. Default is <b>60 seconds</b></param>
        public KvCommandOptions(
            string bucketType,
            string bucket,
            string key,
            bool keyIsRequired,
            Timeout timeout)
            : base(timeout)
        {
            if (string.IsNullOrEmpty(bucketType))
            {
                throw new ArgumentNullException("bucketType");
            }
            else
            {
                this.bucketType = bucketType;
            }

            if (string.IsNullOrEmpty(bucket))
            {
                throw new ArgumentNullException("bucket");
            }
            else
            {
                this.bucket = bucket;
            }

            if (keyIsRequired && string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            else
            {
                this.key = key;
            }
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
