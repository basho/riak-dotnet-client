using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakBucketKeyInput : RiakBucketInput
    {
        private readonly string _key;

        public RiakBucketKeyInput(string bucket, string key)
            : base(bucket)
        {
            _key = key;
        }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            base.WriteJson(writer);
            writer.WriteValue(_key);
            return writer;
        }
    }
}
