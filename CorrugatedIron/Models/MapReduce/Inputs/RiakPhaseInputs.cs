using System.Collections.Generic;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakPhaseInputs
    {
        private readonly List<RiakBucketKeyInput> _bucketKeyInputs;
        private readonly List<RiakBucketKeyArgInput> _bucketKeyArgInputs;
        private readonly RiakBucketInput _bucketInput;

        public RiakPhaseInputs(RiakBucketInput bucketInput)
        {
            _bucketInput = bucketInput;
        }

        public RiakPhaseInputs(List<RiakBucketKeyInput> bucketKeyInputs)
        {
            _bucketKeyInputs = bucketKeyInputs;
        }

        public RiakPhaseInputs(List<RiakBucketKeyArgInput> bucketKeyArgInputs)
        {
            _bucketKeyArgInputs = bucketKeyArgInputs;
        }

        public JsonWriter WriteJson(JsonWriter writer)
        {
            if (_bucketInput != null)
            {
                return _bucketInput.WriteJson(writer);
            }

            writer.WriteStartArray();
            if (_bucketKeyInputs != null)
            {
                _bucketKeyInputs.ForEach(i =>
                    {
                        writer.WriteStartArray();
                        i.WriteJson(writer);
                        writer.WriteEndArray();
                    });
            }
            else
            {
                _bucketKeyArgInputs.ForEach(i =>
                    {
                        writer.WriteStartArray();
                        i.WriteJson(writer);
                        writer.WriteEndArray();
                    });
            }
            writer.WriteEndArray();

            return writer;
        }

        public static implicit operator RiakPhaseInputs(RiakBucketInput input)
        {
            return new RiakPhaseInputs(input);
        }

        public static implicit operator RiakPhaseInputs(List<RiakBucketKeyInput> input)
        {
            return new RiakPhaseInputs(input);
        }

        public static implicit operator RiakPhaseInputs(List<RiakBucketKeyArgInput> input)
        {
            return new RiakPhaseInputs(input);
        }
    }
}
