namespace RiakClient.Models
{
    using Extensions;

    internal class RiakBinIndexId : RiakIndexId
    {
        public RiakBinIndexId(string bucketName, string indexName)
            : base(bucketName, indexName.ToBinaryKey())
        {
        }

        public RiakBinIndexId(string bucketType, string bucketName, string indexName)
            : base(bucketType, bucketName, indexName.ToBinaryKey())
        {
        }
    }
}
