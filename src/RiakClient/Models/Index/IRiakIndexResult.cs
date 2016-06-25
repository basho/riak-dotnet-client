namespace RiakClient.Models.Index
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the interface to an index query result.
    /// </summary>
    public interface IRiakIndexResult
    {
        /// <summary>
        /// The collection of result <see cref="RiakIndexKeyTerm"/>s.
        /// </summary>
        IEnumerable<RiakIndexKeyTerm> IndexKeyTerms { get; }
    }
}
