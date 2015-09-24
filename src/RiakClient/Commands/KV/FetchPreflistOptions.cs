namespace RiakClient.Commands.KV
{
    /// <summary>
    /// Represents options for a <see cref="FetchPreflist"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class FetchPreflistOptions : CommandOptions
    {
        /// <inheritdoc/>
        public FetchPreflistOptions(string bucketType, string bucket, string key)
            : base(new Args(bucketType, bucket, true, key, true))
        {
        }
    }
}