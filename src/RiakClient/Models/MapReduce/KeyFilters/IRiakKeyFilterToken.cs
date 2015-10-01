namespace RiakClient.Models.MapReduce.KeyFilters
{
    /// <summary>
    /// Represents the interface for Riak Key Filter tokens.
    /// </summary>
    public interface IRiakKeyFilterToken
    {
        /// <summary>
        /// Converts the Key Filter Token to a Json-encoded string.
        /// </summary>
        /// <returns>A Json string.</returns>
        string ToJsonString();
    }
}
