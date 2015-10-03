namespace RiakClient.Commands.CRDT
{
    using System.Collections.Generic;
    using Extensions;
    using Util;

    /// <summary>
    /// Represents options for a <see cref="UpdateSet"/> operation.
    /// </summary>
    /// <inheritdoc />
    public class UpdateSetOptions : UpdateCommandOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSetOptions"/> class.
        /// </summary>
        /// <inheritdoc />
        public UpdateSetOptions(string bucketType, string bucket, string key)
            : base(bucketType, bucket, key)
        {
        }

        /// <summary>
        /// The <see cref="UpdateSet"/> additions.
        /// </summary>
        /// <value>The values to add via the <see cref="UpdateSet"/> command.</value>
        public ISet<byte[]> Additions
        {
            get;
            set;
        }

        /// <summary>
        /// The <see cref="UpdateSet"/> removals.
        /// </summary>
        /// <value>The values to remove via the <see cref="UpdateSet"/> command.</value>
        public ISet<byte[]> Removals
        {
            get;
            set;
        }

        /// <summary>
        /// The <see cref="UpdateSet"/> additions, as UTF8-encoded strings.
        /// </summary>
        /// <value>The values to add via the <see cref="UpdateSet"/> command.</value>
        public ISet<string> AdditionsAsStrings
        {
            get { return Additions.GetUTF8Strings(); }
            set { Additions = value.GetUTF8Bytes(); }
        }

        /// <summary>
        /// The <see cref="UpdateSet"/> removals, as UTF8-encoded strings.
        /// </summary>
        /// <value>The values to remove via the <see cref="UpdateSet"/> command.</value>
        public ISet<string> RemovalsAsStrings
        {
            get { return Removals.GetUTF8Strings(); }
            set { Removals = value.GetUTF8Bytes(); }
        }

        protected override bool GetHasRemoves()
        {
            return EnumerableUtil.NotNullOrEmpty(Removals);
        }
    }
}
