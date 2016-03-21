namespace RiakClient.Commands.CRDT
{
    /// <summary>
    /// Represents options for a <see cref="UpdateCounter"/> operation.
    /// </summary>
    /// <inheritdoc />
    public class UpdateCounterOptions : UpdateCommandOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCounterOptions"/> class.
        /// </summary>
        /// <inheritdoc />
        public UpdateCounterOptions(string bucketType, string bucket, string key)
            : base(bucketType, bucket, key)
        {
        }

        /// <summary>
        /// The <see cref="UpdateCounter"/> increment value.
        /// </summary>
        /// <value>The increment value for the <see cref="UpdateCounter"/> command.</value>
        public long Increment
        {
            get;
            set;
        }

        protected override bool GetHasRemoves()
        {
            return false;
        }
    }
}
