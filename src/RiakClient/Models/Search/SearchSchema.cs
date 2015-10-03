namespace RiakClient.Models.Search
{
    using Extensions;
    using Messages;

    /// <summary>
    /// Represents a Lucene search schema.
    /// </summary>
    public class SearchSchema
    {
        private readonly string name;
        private readonly string content;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchSchema"/> class.
        /// </summary>
        /// <param name="name">The name of the schema.</param>
        /// <param name="content">A string containing the schema.</param>
        public SearchSchema(string name, string content)
        {
            this.name = name;
            this.content = content;
        }

        internal SearchSchema(RpbYokozunaSchema schema)
        {
            this.name = schema.name.FromRiakString();
            this.content = schema.content.FromRiakString();
        }

        /// <summary>
        /// The name of the search schema.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// A string containing the schema.
        /// </summary>
        public string Content
        {
            get { return content; }
        }

        internal RpbYokozunaSchema ToMessage()
        {
            return new RpbYokozunaSchema
            {
                name = this.name.ToRiakString(),
                content = this.content.ToRiakString()
            };
        }
    }
}
