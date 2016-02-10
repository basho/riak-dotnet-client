namespace RiakClient.Models
{
    using Extensions;

    internal class RiakIntIndexId : RiakIndexId
    {
        public RiakIntIndexId(string bucketName, string indexName)
            : base(bucketName, indexName.ToIntegerKey())
        {
        }

        public RiakIntIndexId(string bucketType, string bucketName, string indexName)
            : base(bucketType, bucketName, indexName.ToIntegerKey())
        {
        }
    }
}
