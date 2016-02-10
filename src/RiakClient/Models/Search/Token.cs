namespace RiakClient.Models.Search
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a Lucene search token.
    /// </summary>
    public class Token
    {
        private static readonly Regex EncodeRegex = new Regex(@"(["" \\'\(\)\[\]\\:\+\-\/\?])");

        private readonly string value;
        private readonly string suffix;

        internal Token(string value)
            : this(value, null)
        {
        }

        internal Token(string value, string suffix)
        {
            this.value = value;
            this.suffix = suffix;
        }

        /// <summary>
        /// Create a token for searching for field values that exactly match the parameter <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>A newly initialized and configured <see cref="Token"/>.</returns>
        public static Token Is(string value)
        {
            return new Token(value);
        }

        /// <summary>
        /// Create a token for searching for field values that start with the parameter <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>A newly initialized and configured <see cref="Token"/>.</returns>
        public static Token StartsWith(string value)
        {
            return new Token(value, "*");
        }

        /// <summary>
        /// Returns the token in a format acceptable for Lucene query strings.
        /// </summary>
        /// <returns>A string that represents the token.</returns>
        public override string ToString()
        {
            return value != null ? EncodeRegex.Replace(value, m => "\\" + m.Value) + suffix : string.Empty;
        }
    }
}
