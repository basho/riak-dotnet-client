using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public abstract class RiakIndexInput : RiakPhaseInput
    {
        public RiakIndexId IndexId { get; protected set; }
        
        protected void WriteIndexHeaderJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");

            writer.WriteStartObject();

            WriteBucketKeyBucketJson(writer, IndexId.BucketType, IndexId.BucketName);

            writer.WritePropertyName("index");
            writer.WriteValue(IndexId.IndexName);
        }

        private void WriteBucketKeyBucketJson(JsonWriter writer, string bucketType, string bucketName)
        {
            writer.WritePropertyName("bucket");

            if (string.IsNullOrEmpty(bucketType))
            {
                writer.WriteValue(bucketName);
            }
            else
            {
                writer.WriteStartArray();
                writer.WriteValue(bucketType);
                writer.WriteValue(bucketName);
                writer.WriteEndArray();
            }
        }
    }
}