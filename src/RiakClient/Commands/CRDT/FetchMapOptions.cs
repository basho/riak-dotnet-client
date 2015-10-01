namespace RiakClient.Commands.CRDT
{
    /// <summary>
    /// Represents options for a <see cref="FetchMap"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class FetchMapOptions : FetchCommandOptions
    {
        /// <inheritdoc/>
        public FetchMapOptions(string bucketType, string bucket, string key)
            : base(bucketType, bucket, key)
        {
        }
    }
}
