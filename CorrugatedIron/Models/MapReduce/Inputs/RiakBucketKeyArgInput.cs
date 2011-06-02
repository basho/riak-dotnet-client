using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakBucketKeyArgInput : RiakBucketKeyInput
    {
        private readonly string _arg;

        public RiakBucketKeyArgInput(string bucket, string key, string arg)
            : base(bucket, key)
        {
            _arg = arg;
        }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            base.WriteJson(writer);
            writer.WriteValue(_arg);
            return writer;
        }
    }
}
