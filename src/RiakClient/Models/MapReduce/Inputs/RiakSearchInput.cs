namespace RiakClient.Models.MapReduce.Inputs
{
    using Models.Search;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a search based mapreduce input.
    /// </summary>
    public class RiakSearchInput : RiakPhaseInput
    {
        private readonly string index;
        private readonly string query;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakSearchInput"/> class.
        /// </summary>
        /// <param name="query">The <see cref="RiakFluentSearch"/> to run, whose results will be used as inputs for the mapreduce job. </param>
        public RiakSearchInput(RiakFluentSearch query)
            : this(query.Index, query.ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakSearchInput"/> class.
        /// </summary>
        /// <param name="index">The index to run the <paramref name="query"/> against.</param>
        /// <param name="query">The query to run, whose results will be used as inputs for the mapreduce job.</param>
        public RiakSearchInput(string index, string query)
        {
            this.index = index;
            this.query = query;
        }

        /// <inheritdoc/>
        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartObject();

            writer.WritePropertyName("index");
            writer.WriteValue(index);

            writer.WritePropertyName("query");
            writer.WriteValue(query);

            writer.WriteEndObject();

            return writer;
        }
    }
}
