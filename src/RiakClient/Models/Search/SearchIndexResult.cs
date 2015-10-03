namespace RiakClient.Models.Search
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Messages;

    /// <summary>
    /// Represents the result value for fetching a Riak search index.
    /// </summary>
    public class SearchIndexResult
    {
        private readonly ReadOnlyCollection<SearchIndex> indexes;

        internal SearchIndexResult(RpbYokozunaIndexGetResp getResponse)
        {
            var searchIndexes = getResponse.index.Select(i => new SearchIndex(i));
            this.indexes = new ReadOnlyCollection<SearchIndex>(searchIndexes.ToList());
        }

        /// <summary>
        /// The collection of matching search indexes.
        /// </summary>
        public ReadOnlyCollection<SearchIndex> Indexes
        {
            get { return indexes; }
        }
    }
}
