namespace RiakClient.Commands.KV
{
    using System;

    /// <summary>
    /// Part of the <see cref="PreflistResponse"/> class.
    /// </summary>
    public class PreflistItem
    {
        private readonly RiakString node;
        private readonly long id;
        private readonly bool primary;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreflistItem"/> class.
        /// </summary>
        /// <param name="node">A <see cref="RiakString"/> representing the node owning the partition.</param>
        /// <param name="id">The partition ID.</param>
        /// <param name="primary">Will be <b>true</b> if this is a primary node for the partition.</param>
        public PreflistItem(RiakString node, long id, bool primary)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            this.node = node;
            this.id = id;
            this.primary = primary;
        }

        public RiakString Node
        {
            get { return node; }
        }

        public long ID
        {
            get { return id; }
        }

        public bool Primary
        {
            get { return primary; }
        }
    }
}
