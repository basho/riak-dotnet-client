namespace RiakClient.Commands.CRDT
{
    using System.Collections.Generic;
    using Extensions;

    /// <summary>
    /// Response to a <see cref="FetchSet"/> command.
    /// </summary>
    public class SetResponse : DataTypeResponse<ISet<byte[]>>
    {
        /// <inheritdoc />
        public SetResponse()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetResponse"/> class.
        /// </summary>
        /// <param name="key">A <see cref="RiakString"/> representing the key.</param>
        /// <param name="context">The data type context. Necessary to use this if updating a data type with removals.</param>
        /// <param name="value">The value of the fetched CRDT counter.</param>
        public SetResponse(RiakString key, byte[] context, ISet<byte[]> value)
            : base(key, context, value)
        {
        }

        public ISet<string> AsStrings
        {
            get { return Value.GetUTF8Strings(); }
        }
    }
}
