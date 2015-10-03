namespace RiakClient.Models.Search
{
    /// <summary>
    /// Represents a Lucene "unary" search term.
    /// </summary>
    public class UnaryTerm : Term
    {
        private readonly Token value;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryTerm"/> class.
        /// </summary>
        /// <param name="search">The fluent search to add this term to.</param>
        /// <param name="field">The field to search.</param>
        /// <param name="value">The value to search the <paramref name="field"/> for.</param>
        internal UnaryTerm(RiakFluentSearch search, string field, string value)
            : this(search, field, Token.Is(value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryTerm"/> class.
        /// </summary>
        /// <param name="search">The fluent search to add this term to.</param>
        /// <param name="field">The field to search.</param>
        /// <param name="value">The <see cref="Token"/> to search the <paramref name="field"/> for.</param>
        internal UnaryTerm(RiakFluentSearch search, string field, Token value)
            : base(search, field)
        {
            this.value = value;
        }

        /// <summary>
        /// Returns the term in a Lucene query string format.
        /// </summary>
        /// <returns>
        /// A string that represents the query term.</returns>
        public override string ToString()
        {
            return Prefix() + Field() + value + Suffix();
        }
    }
}
