namespace RiakClient.Models.Search
{
    /// <summary>
    /// Represents a Lucene grouped search term.
    /// </summary>
    public class GroupTerm : Term
    {
        private readonly Term term;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupTerm"/> class.
        /// </summary>
        /// <param name="search">The fluent search to add this term to.</param>
        /// <param name="field">The field to search.</param>
        /// <param name="term">The <see cref="Term"/> group to search the <paramref name="field"/> for.</param>
        internal GroupTerm(RiakFluentSearch search, string field, Term term)
            : base(search, field)
        {
            this.term = term;
        }

        /// <summary>
        /// Returns the term in a Lucene query string format.
        /// </summary>
        /// <returns>
        /// A string that represents the query term.</returns>
        public override string ToString()
        {
            Term tmpTerm = term;

            while (tmpTerm.Owner != null)
            {
                tmpTerm = tmpTerm.Owner;
            }

            return Prefix() + "(" + tmpTerm + ")" + Suffix();
        }
    }
}
