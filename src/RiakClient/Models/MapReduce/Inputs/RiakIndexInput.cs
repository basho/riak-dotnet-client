namespace RiakClient.Models.MapReduce.Inputs
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an abstract secondary index query mapreduce input.
    /// </summary>
    public abstract class RiakIndexInput : RiakPhaseInput
    {
        private readonly RiakIndexId indexId;

        protected RiakIndexInput(RiakIndexId indexId)
        {
            this.indexId = indexId;
        }

        /// <summary>
        /// The <see cref="RiakIndexId"/> of the index to query.
        /// </summary>
        public RiakIndexId IndexId
        {
            get { return indexId; }
        }

        /// <summary>
        /// The bucket containing the index to query.
        /// </summary>
        [Obsolete("Use IndexId.BucketName instead. This will be removed in the next version.")]
        public string Bucket
        {
            get { return IndexId != null ? IndexId.BucketName : null; }
        }

        /// <summary>
        /// The name of the index to query.
        /// </summary>
        [Obsolete("Use IndexId.SchemaName instead. This will be removed in the next version.")]
        public string Index
        {
            get { return IndexId != null ? IndexId.IndexName : null; }
        }

        protected void WriteIndexHeaderJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");

            writer.WriteStartObject();

            WriteBucketKeyBucketJson(writer, IndexId.BucketType, IndexId.BucketName);

            writer.WritePropertyName("index");
            writer.WriteValue(IndexId.IndexName);
        }
    }
}
