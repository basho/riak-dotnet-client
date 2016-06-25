namespace RiakClient.Models.MapReduce.Inputs
{
    using System.Collections.Generic;
    using Models.MapReduce.KeyFilters;
    using Newtonsoft.Json;

    /// <summary>
    /// Interface for mapreduce phase inputs.
    /// </summary>
    public interface IRiakPhaseInput
    {
        /// <summary>
        /// Serialize the phase input to JSON and write it using the <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        /// <returns>The original JsonWriter, not modified but useful for call chaining.</returns>
        JsonWriter WriteJson(JsonWriter writer);
    }

    /// <summary>
    /// Abstract implementation of <see cref="IRiakPhaseInput"/>. 
    /// </summary>
    public abstract class RiakPhaseInput : IRiakPhaseInput
    {
        /// <summary>
        /// A list of key filters represented as <see cref="IRiakKeyFilterToken"/>s.
        /// </summary>
        public List<IRiakKeyFilterToken> Filters { get; set; }

        /// <inheritdoc/>
        public abstract JsonWriter WriteJson(JsonWriter writer);

        protected void WriteBucketKeyBucketJson(JsonWriter writer, string bucketType, string bucketName)
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
