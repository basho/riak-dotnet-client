namespace CorrugatedIron.Models
{
    public class RiakObjectId
    {
        public string Bucket { get; set; }
        public string Key { get; set; }

        public RiakObjectId(string bucket, string key)
        {
            Bucket = bucket;
            Key = key;
        }

        internal RiakLink ToRiakLink(string tag)
        {
            return new RiakLink(Bucket, Key, tag);
        }
    }
}
