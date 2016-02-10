namespace RiakClient.Models.Search
{
    /// <summary>
    /// Represents a Lucene "range" search term.
    /// </summary>
    public class RangeTerm : Term
    {
        private readonly Token from;
        private readonly Token to;
        private readonly bool inclusive;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeTerm"/> class.
        /// </summary>
        /// <param name="search">The fluent search to add this term to.</param>
        /// <param name="field">The field to search.</param>
        /// <param name="from">The lower bound of values to search the <paramref name="field"/> for.</param>
        /// <param name="to">The upper bound of values to search the <paramref name="field"/> for.</param>
        /// <param name="inclusive">The option to include the bounds in the range or not.</param>
        internal RangeTerm(RiakFluentSearch search, string field, Token from, Token to, bool inclusive)
            : base(search, field)
        {
            this.from = from;
            this.to = to;
            this.inclusive = inclusive;
        }

        /// <summary>
        /// Returns the term in a Lucene query string format.
        /// </summary>
        /// <returns>A string that represents the query term.</returns>
        public override string ToString()
        {
            var brackets = inclusive ? new[] { "[", "]" } : new[] { "{", "}" };
            return Prefix() + Field() + brackets[0] + from + " TO " + to + brackets[1];
        }
    }
}
