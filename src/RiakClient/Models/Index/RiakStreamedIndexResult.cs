namespace RiakClient.Models.Index
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Messages;

    /// <summary>
    /// Represents a result to a streaming index query.
    /// </summary>
    public class RiakStreamedIndexResult : IRiakIndexResult
    {
        private readonly IEnumerable<RiakResult<RpbIndexResp>> responseReader;
        private readonly bool includeTerms;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakStreamedIndexResult"/> class.
        /// </summary>
        /// <param name="includeTerms">The option to include the terms in the results.</param>
        /// <param name="responseReader">The <see cref="IEnumerable{T}"/> to read results from.</param>
        public RiakStreamedIndexResult(bool includeTerms, IEnumerable<RiakResult<RpbIndexResp>> responseReader)
        {
            this.responseReader = responseReader;
            this.includeTerms = includeTerms;
        }

        /// <inheritdoc/>
        public IEnumerable<RiakIndexKeyTerm> IndexKeyTerms
        {
            get
            {
                return responseReader.SelectMany(item => GetIndexKeyTerm(item.Value));
            }
        }

        private IEnumerable<RiakIndexKeyTerm> GetIndexKeyTerm(RpbIndexResp response)
        {
            IEnumerable<RiakIndexKeyTerm> indexKeyTerms = null;

            if (includeTerms)
            {
                indexKeyTerms = response.results.Select(
                    pair => new RiakIndexKeyTerm(pair.value.FromRiakString(), pair.key.FromRiakString()));
            }
            else
            {
                indexKeyTerms = response.keys.Select(key => new RiakIndexKeyTerm(key.FromRiakString()));
            }

            return indexKeyTerms;
        }
    }
}
