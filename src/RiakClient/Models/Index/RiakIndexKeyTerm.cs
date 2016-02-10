namespace RiakClient.Models.Index
{
    /// <summary>
    /// Represents an index result Key-Term pair.
    /// </summary>
    public class RiakIndexKeyTerm
    {
        private readonly string key;
        private readonly string term;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakIndexKeyTerm" /> class.
        /// </summary>
        /// <param name="key">The result key to use.</param>
        public RiakIndexKeyTerm(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakIndexKeyTerm" /> class.
        /// </summary>
        /// <param name="key">The result key to use.</param>
        /// <param name="term">The matchint term to use.</param>
        public RiakIndexKeyTerm(string key, string term)
            : this(key)
        {
            this.term = term;
        }

        /// <summary>
        /// The result key.
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// The matching term.
        /// </summary>
        public string Term
        {
            get { return term; }
        }
    }
}
