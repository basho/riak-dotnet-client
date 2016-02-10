namespace RiakClient.Models.Search
{
    /// <summary>
    /// Represents a Lucene "binary" search term, such as AND &amp; OR.
    /// </summary>
    public class BinaryTerm : Term
    {
        private readonly Term left;
        private readonly Term right;
        private readonly Op op;
        private readonly Token value;

        internal BinaryTerm(RiakFluentSearch search, string field, Op op, Term left, string value)
            : this(search, field, op, left, Token.Is(value))
        {
        }

        internal BinaryTerm(RiakFluentSearch search, string field, Op op, Term left, Token value)
            : this(search, field, op, left)
        {
            this.value = value;
        }

        internal BinaryTerm(RiakFluentSearch search, string field, Op op, Term left, Term right)
            : this(search, field, op, left)
        {
            this.right = right;
        }

        private BinaryTerm(RiakFluentSearch search, string field, Op op, Term left)
            : base(search, field)
        {
            this.op = op;
            this.left = left;
            left.Owner = this;
        }

        internal enum Op
        {
            And,
            Or
        }

        /// <summary>
        /// Returns the term in a Lucene query string format.
        /// </summary>
        /// <returns>
        /// A string that represents the query term.</returns>
        public override string ToString()
        {
            return left + " " + op.ToString().ToUpper() + " "
                + Prefix()
                + (right != null ? right.ToString() : Field() + value) + Suffix();
        }
    }
}
