namespace RiakClient.Models.MapReduce.Inputs
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a string secondary index match query mapreduce input.
    /// </summary>
    public class RiakBinIndexEqualityInput : RiakIndexInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakBinIndexEqualityInput"/> class.
        /// </summary>
        /// <param name="bucket">The bucket that contains the <paramref name="index"/> to query.</param>
        /// <param name="index">
        /// The index to query. The output of that query will be used as input for the mapreduce job.
        /// </param>
        /// <param name="key">The index key to query for.</param>
        [Obsolete("Use the constructor that accepts a RiakIndexId instead. This will be removed in the next version.")]
        public RiakBinIndexEqualityInput(string bucket, string index, string key)
            : this(new RiakIndexId(bucket, index), key)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakBinIndexEqualityInput"/> class.
        /// </summary>
        /// <param name="indexId">
        /// The <see cref="RiakIndexId"/> that specifies which index to query.
        /// The output of that query will be used as input for the mapreduce job.
        /// </param>
        /// <param name="key">The index key to query for.</param>
        public RiakBinIndexEqualityInput(RiakIndexId indexId, string key)
            : base(indexId.ToBinIndexId())
        {
            Key = key;
        }

        /// <summary>
        /// The index key to query for.
        /// </summary>
        public string Key { get; set; }

        /// <inheritdoc/>
        public override JsonWriter WriteJson(JsonWriter writer)
        {
            WriteIndexHeaderJson(writer);

            writer.WritePropertyName("key");
            writer.WriteValue(Key);
            writer.WriteEndObject();

            return writer;
        }
    }
}
