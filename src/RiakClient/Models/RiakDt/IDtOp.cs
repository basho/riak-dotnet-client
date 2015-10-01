namespace RiakClient.Models.RiakDt
{
    using Messages;

    /// <summary>
    /// An interface representing a Riak datatype operation.
    /// </summary>
    [System.Obsolete("RiakDt is deprecated. Please use Commands/CRDT namespace.")]
    public interface IDtOp
    {
        /// <summary>
        /// Convert the current instance to a <see cref="DtOp"/>.
        /// </summary>
        /// <returns>A new <see cref="DtOp"/>.</returns>
        DtOp ToDtOp();
    }
}
