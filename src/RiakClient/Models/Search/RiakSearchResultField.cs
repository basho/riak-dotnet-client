namespace RiakClient.Models.Search
{
    using Extensions;
    using Messages;

    /// <summary>
    /// Represents a single Riak Search result field.
    /// </summary>
    public class RiakSearchResultField
    {
        private readonly string key;
        private readonly string value;

        internal RiakSearchResultField(RpbPair field)
        {
            this.key = field.key.FromRiakString();
            this.value = field.value.FromRiakString();
        }

        /// <summary>
        /// The key of the result field.
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// The value of the result field.
        /// </summary>
        public string Value
        {
            get { return value; }
        }
    }
}
