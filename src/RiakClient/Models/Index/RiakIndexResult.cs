namespace RiakClient.Models.Index
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Messages;

    /// <summary>
    /// Represents a result to an index query.
    /// </summary>
    public class RiakIndexResult : IRiakIndexResult
    {
        private readonly IEnumerable<RiakIndexKeyTerm> indexKeyTerms;

        internal RiakIndexResult(bool includeTerms, RiakResult<RpbIndexResp> response)
        {
            if (includeTerms)
            {
                indexKeyTerms = response.Value.results.Select(
                    pair => new RiakIndexKeyTerm(pair.value.FromRiakString(), pair.key.FromRiakString()));
            }
            else
            {
                indexKeyTerms = response.Value.keys.Select(key => new RiakIndexKeyTerm(key.FromRiakString()));
            }
        }

        /// <inheritdoc/>
        public IEnumerable<RiakIndexKeyTerm> IndexKeyTerms
        {
            get { return indexKeyTerms; }
        }
    }
}
