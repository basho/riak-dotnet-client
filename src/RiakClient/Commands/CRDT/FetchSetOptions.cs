namespace RiakClient.Commands.CRDT
{
    /// <summary>
    /// Represents options for a <see cref="FetchSet"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class FetchSetOptions : FetchCommandOptions
    {
        /// <inheritdoc/>
        public FetchSetOptions(string bucketType, string bucket, string key)
            : base(bucketType, bucket, key)
        {
        }
    }
}
