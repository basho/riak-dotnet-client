namespace RiakClient.Models.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Messages;

    /// <summary>
    /// An enumeration of Riak Search result sort orders
    /// </summary>
    public enum PreSort
    {
        /// <summary>
        /// Sort the results by bucket key.
        /// </summary>
        Key,

        /// <summary>
        /// Sort the results by search score.
        /// </summary>
        Score
    }

    /// <summary>
    /// An enumeration of different default search query operators.
    /// </summary>
    public enum DefaultOperation
    {
        /// <summary>
        /// The and operator.
        /// </summary>
        And,

        /// <summary>
        /// The or operator.
        /// </summary>
        Or
    }

    /// <summary>
    /// Represents a Riak Search request.
    /// </summary>
    public class RiakSearchRequest : IEquatable<RiakSearchRequest>
    {
        private readonly string solrIndex;
        private readonly string solrQuery;
        private readonly string solrFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakSearchRequest"/> class.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="Query"/> and <see cref="Filter"/> members for specifying
        /// Solr query and filter.
        /// </remarks>
        public RiakSearchRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakSearchRequest"/> class.
        /// </summary>
        /// <param name="solrIndex">The name of the Solr index to query.</param>
        /// <param name="solrQuery">The Solr query.</param>
        /// <param name="solrFilter">The Solr filter to use. Defaults to <c>null</c>.</param>
        /// <exception cref="ArgumentException"><paramref name="solrIndex"/> cannot be null, zero length, or whitespace</exception>
        /// <exception cref="ArgumentException"><paramref name="solrQuery"/> cannot be null, zero length, or whitespace</exception>
        public RiakSearchRequest(string solrIndex, string solrQuery, string solrFilter = null)
        {
            if (string.IsNullOrWhiteSpace(solrIndex))
            {
                throw new ArgumentNullException("solrIndex");
            }

            this.solrIndex = solrIndex;

            if (string.IsNullOrWhiteSpace(solrQuery))
            {
                throw new ArgumentNullException("solrQuery");
            }

            this.solrQuery = solrQuery;
            this.solrFilter = solrFilter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakSearchRequest"/> class.
        /// </summary>
        /// <param name="solrQuery">The Solr query as expressed by an instance of <see cref="RiakFluentSearch"/>.</param>
        /// <param name="solrFilter">The Solr filter as expressed by an instance of <see cref="RiakFluentSearch"/>. Defaults to <c>null</c>.</param>
        /// <exception cref="ArgumentException"><paramref name="solrIndex"/> cannot be null, zero length, or whitespace</exception>
        /// <exception cref="ArgumentException"><paramref name="solrQuery"/> cannot be null, zero length, or whitespace</exception>
        public RiakSearchRequest(
            RiakFluentSearch solrQuery,
            RiakFluentSearch solrFilter = null)
        {
            if (solrQuery == null)
            {
                throw new ArgumentNullException("solrQuery");
            }

            this.Query = solrQuery;
            this.Filter = solrFilter;
        }

        /// <summary>
        /// The query to run for the search.
        /// </summary>
        public RiakFluentSearch Query { get; set; }

        /// <summary>
        /// The filter to use for the search.
        /// </summary>
        public RiakFluentSearch Filter { get; set; }

        /// <summary>
        /// The maximum number of rows to return.
        /// </summary>
        /// <remarks>
        /// Combine with <see cref="Start"/> to implement paging.
        /// Distributed pagination in Riak Search cannot be used reliably when sorting on fields 
        /// that can have different values per replica of the same object, namely score and _yz_id. 
        /// In the case of sorting by these fields, you may receive redundant objects. 
        /// In the case of score, the top-N can return different results over multiple runs.
        /// </remarks>
        public long Rows { get; set; }

        /// <summary>
        /// The starting row to return.
        /// </summary>
        /// <remarks>
        /// Combine with <see cref="Rows"/> to implement paging.
        /// Distributed pagination in Riak Search cannot be used reliably when sorting on fields 
        /// that can have different values per replica of the same object, namely score and _yz_id. 
        /// In the case of sorting by these fields, you may receive redundant objects. 
        /// In the case of score, the top-N can return different results over multiple runs.
        /// </remarks>
        public long Start { get; set; }

        /// <summary>
        /// A <see cref="RiakFluentSearch"/> "filter" to run on the query.
        /// </summary>
        /// <summary>
        /// The field to sort on.
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// Presort the results by Key or Score.
        /// </summary>
        public PreSort? PreSort { get; set; }

        /// <summary>
        /// The default operator for parsing queries.
        /// </summary>
        /// <remarks>Defaults to <see cref="DefaultOperation"/>.And if not specified.</remarks>
        public DefaultOperation? DefaultOperation { get; set; }

        /// <summary>
        /// The list of fields that should be returned for each record in the result list.
        /// </summary>
        /// <remarks>
        /// The 'id' field is always returned, even if not specified in this list.
        /// </remarks>
        public List<string> ReturnFields { get; set; }

        /// <summary>
        /// Compares two <see cref="RiakSearchRequest"/> objects for equality.
        /// </summary>
        /// <param name="other">The instance of <see cref="RiakSearchRequest"/> with which to compare equality</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public bool Equals(RiakSearchRequest other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as RiakSearchRequest);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            string filterString = GetFilterString();

            unchecked
            {
                int idxHashCode = GetIndexString().GetHashCode();
                int queryHashCode = GetQueryString().GetHashCode();
                int filterHashCode = filterString == null ? 0 : filterString.GetHashCode();
                return (idxHashCode * 397) ^ (queryHashCode * 397) ^ (filterHashCode * 397);
            }
        }

        internal RpbSearchQueryReq ToMessage()
        {
            var msg = new RpbSearchQueryReq
            {
                index = GetIndexString().ToRiakString(),
                q = GetQueryString().ToRiakString(),
                rows = (uint)Rows,
                start = (uint)Start,
                sort = Sort.ToRiakString(),
                filter = GetFilterString().ToRiakString(),
                presort = PreSort.HasValue ? PreSort.Value.ToString().ToLower().ToRiakString() : null,
                op = DefaultOperation.HasValue ? DefaultOperation.Value.ToString().ToLower().ToRiakString() : null
            };

            if (ReturnFields != null)
            {
                msg.fl.AddRange(ReturnFields.Select(x => x.ToRiakString()));
            }

            return msg;
        }

        private string GetIndexString()
        {
            string indexString = null;

            if (solrIndex != null)
            {
                indexString = solrIndex;
            }
            else
            {
                indexString = Query.Index;
            }

            return indexString;
        }

        private string GetQueryString()
        {
            string queryString = null;

            if (solrQuery != null)
            {
                queryString = solrQuery;
            }
            else
            {
                queryString = Query.ToString();
            }

            return queryString;
        }

        private string GetFilterString()
        {
            string filterString = null;

            if (solrFilter != null)
            {
                filterString = solrFilter;
            }
            else if (Filter != null)
            {
                filterString = Filter.ToString();
            }

            return filterString;
        }
    }
}
