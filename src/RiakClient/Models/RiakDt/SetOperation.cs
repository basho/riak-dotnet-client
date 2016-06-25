namespace RiakClient.Models.RiakDt
{
    using Messages;

    /// <summary>
    /// Represents an operation on a Riak Set data type.
    /// </summary>
    [System.Obsolete("RiakDt is deprecated. Please use Commands/CRDT namespace.")]
    public class SetOperation : IDtOp
    {
        /// <inheritdoc/>
        public DtOp ToDtOp()
        {
            return new DtOp
            {
                set_op = new SetOp()
            };
        }
    }
}
