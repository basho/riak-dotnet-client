namespace RiakClient.Models.Search
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Messages;

    /// <summary>
    /// Represents the result of a Riak Search operation.
    /// </summary>
    public class RiakSearchResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakSearchResult"/> class.
        /// </summary>
        /// <param name="maxScore">The maximum score out of all the documents found.</param>
        /// <param name="numFound">The total number of documents found.</param>
        /// <param name="documents">The documents found.</param>
        /// <exception cref="ArgumentNullException">The value of 'documents' cannot be null. </exception>
        public RiakSearchResult(float maxScore, long numFound, IEnumerable<RiakSearchResultDocument> documents)
        {
            if (documents == null)
            {
                throw new ArgumentNullException("documents", "The documents parameter cannot be null.");
            }

            MaxScore = maxScore;
            NumFound = numFound;
            Documents = new ReadOnlyCollection<RiakSearchResultDocument>(documents.ToList());
        }

        internal RiakSearchResult(RpbSearchQueryResp response)
        {
            MaxScore = response.max_score;
            NumFound = response.num_found;

            var docs = response.docs.Select(d => new RiakSearchResultDocument(d));
            Documents = new ReadOnlyCollection<RiakSearchResultDocument>(docs.ToList());
        }

        /// <summary>
        /// The maximum score of all the matching documents.
        /// </summary>
        public float MaxScore
        {
            get;
            private set;
        }

        /// <summary>
        /// The total number of matching documents found.
        /// </summary>
        public long NumFound
        {
            get;
            private set;
        }

        /// <summary>
        /// The collection of matching documents, represented as a collection of <see cref="RiakSearchResultDocument"/>s.
        /// </summary>
        public ReadOnlyCollection<RiakSearchResultDocument> Documents
        {
            get;
            private set;
        }
    }
}
