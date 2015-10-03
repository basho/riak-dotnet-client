namespace RiakClient.Models.RiakDt
{
    using Messages;

    /// <summary>
    /// Represents an operation on a Riak Map data type.
    /// </summary>
    [System.Obsolete("RiakDt is deprecated. Please use Commands/CRDT namespace.")]
    public class MapOperation : IDtOp
    {
        /// <inheritdoc/>
        public DtOp ToDtOp()
        {
            return new DtOp
            {
                map_op = new MapOp()
            };
        }
    }
}
