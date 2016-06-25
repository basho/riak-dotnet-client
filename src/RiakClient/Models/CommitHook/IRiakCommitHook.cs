namespace RiakClient.Models.CommitHook
{
    using Messages;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an interface for commit hooks.
    /// </summary>
    public interface IRiakCommitHook
    {
        /// <summary>
        /// Convert the commit hook to a JSON-encoded string.
        /// </summary>
        /// <returns>
        /// A JSON-encoded string.
        /// </returns>
        string ToJsonString();

        /// <summary>
        /// Converts the commit hook to a protobuf message.
        /// </summary>
        /// <returns>
        /// A new instance of a <see cref="RpbCommitHook"/> class.
        /// </returns>
        RpbCommitHook ToRpbCommitHook();

        /// <summary>
        /// Write the commit hook to a <see cref="JsonWriter"/>.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        void WriteJson(JsonWriter writer);
    }
}
