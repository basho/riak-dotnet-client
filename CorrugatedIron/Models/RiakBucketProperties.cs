using CorrugatedIron.Messages;

namespace CorrugatedIron.Models
{
    public class RiakBucketProperties
    {
        public bool AllowMultiple { get; set; }
        public uint NVal { get; set; }

        public RiakBucketProperties()
        {
        }

        public RiakBucketProperties(RpbBucketProps bucketProps)
        {
            AllowMultiple = bucketProps.AllowMultiple;
            NVal = bucketProps.NVal;
        }
    }
}
