using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakBucketInput : IRiakPhaseInput
    {
        private readonly string _bucket;

        public RiakBucketInput(string bucket)
        {
            _bucket = bucket;
        }

        public virtual JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WriteValue(_bucket);
            return writer;
        }

        public static implicit operator RiakBucketInput(string bucket)
        {
            return new RiakBucketInput(bucket);
        }
    }
}
