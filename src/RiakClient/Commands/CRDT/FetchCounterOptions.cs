namespace RiakClient.Commands.CRDT
{
    /// <summary>
    /// Represents options for a <see cref="FetchCounter"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class FetchCounterOptions : FetchCommandOptions
    {
        /// <inheritdoc/>
        public FetchCounterOptions(string bucketType, string bucket, string key)
            : base(bucketType, bucket, key)
        {
        }
    }
}
