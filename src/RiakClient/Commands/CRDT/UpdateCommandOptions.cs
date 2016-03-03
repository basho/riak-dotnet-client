namespace RiakClient.Commands.CRDT
{
    /// <summary>
    /// Represents options for a <see cref="UpdateCommand{TResponse}"/> operation.
    /// </summary>
    public abstract class UpdateCommandOptions : KvCommandOptions
    {
        private bool returnBody = true;
        private bool includeContext = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandOptions"/> class.
        /// </summary>
        /// <param name="bucketType">The bucket type in Riak. Required.</param>
        /// <param name="bucket">The bucket in Riak. Required.</param>
        /// <param name="key">The key in Riak. If <b>null</b>, Riak will generate a key.</param>
        public UpdateCommandOptions(string bucketType, string bucket, string key)
            : base(bucketType, bucket, key, false)
        {
        }

        /// <summary>
        /// The W (write) value to use.
        /// </summary>
        public Quorum W { get; set; }

        /// <summary>
        /// The PW (primary vnode write) value to use.
        /// </summary>
        public Quorum PW { get; set; }

        /// <summary>
        /// The DW (durable write) value to use.
        /// </summary>
        public Quorum DW { get; set; }

        /// <summary>
        /// If true, returns the updated CRDT.
        /// </summary>
        public bool ReturnBody
        {
            get { return returnBody; }
            set { returnBody = value; }
        }

        /// <summary>
        /// The context from a previous fetch. Required for remove operations. 
        /// </summary>
        public byte[] Context { get; set; }

        /// <summary>
        /// Set to <b>false</b> to not return context. Default (and recommended value) is <b>true</b>.
        /// </summary>
        public bool IncludeContext
        {
            get { return includeContext; }
            set { includeContext = value; }
        }

        /// <summary>
        /// Returns to <b>true</b> if this command has removals.
        /// </summary>
        public bool HasRemoves
        {
            get { return GetHasRemoves(); }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ (W != null ? W.GetHashCode() : 0);
                result = (result * 397) ^ (PW != null ? PW.GetHashCode() : 0);
                result = (result * 397) ^ (DW != null ? DW.GetHashCode() : 0);
                result = (result * 397) ^ ReturnBody.GetHashCode();
                result = (result * 397) ^ Context.GetHashCode();
                result = (result * 397) ^ IncludeContext.GetHashCode();
                return result;
            }
        }

        protected abstract bool GetHasRemoves();
    }
}