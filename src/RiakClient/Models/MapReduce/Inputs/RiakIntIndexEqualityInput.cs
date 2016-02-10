namespace RiakClient.Models.MapReduce.Inputs
{
    using System;
    using System.Numerics;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an integer secondary index match query mapreduce input.
    /// </summary>
    public class RiakIntIndexEqualityInput : RiakIndexInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakIntIndexEqualityInput"/> class.
        /// </summary>
        /// <param name="bucket">The bucket that contains the <paramref name="index"/> to query.</param>
        /// <param name="index">
        /// The index to query. The output of that query will be used as input for the mapreduce job.
        /// </param>
        /// <param name="key">The index key to query for.</param>
        [Obsolete("Use the constructor that accepts a RiakIndexId instead. This will be removed in the next version.")]
        public RiakIntIndexEqualityInput(string bucket, string index, BigInteger key)
            : this(new RiakIndexId(bucket, index), key)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakIntIndexEqualityInput"/> class.
        /// </summary>
        /// <param name="indexId">
        /// The <see cref="RiakIndexId"/> that specifies which index to query.
        /// The output of that query will be used as input for the mapreduce job.
        /// </param>
        /// <param name="key">The index key to query for.</param>
        public RiakIntIndexEqualityInput(RiakIndexId indexId, BigInteger key)
            : base(indexId.ToIntIndexId())
        {
            Key = key;
        }

        // TODO: immutable

        /// <summary>
        /// The index key to query for.
        /// </summary>
        public BigInteger Key { get; set; }

        /// <inheritdoc/>
        public override JsonWriter WriteJson(JsonWriter writer)
        {
            WriteIndexHeaderJson(writer);

            writer.WritePropertyName("key");
            writer.WriteValue(Key.ToString());
            writer.WriteEndObject();

            return writer;
        }
    }
}
