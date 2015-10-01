namespace RiakClient.Commands.KV
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Response to a <see cref="ListBuckets"/> command.
    /// </summary>
    public class ListBucketsResponse : Response<IReadOnlyCollection<RiakString>>
    {
        private readonly List<RiakString> buckets;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBucketsResponse"/> class.
        /// </summary>
        public ListBucketsResponse()
            : base(new List<RiakString>())
        {
            buckets = (List<RiakString>)Value;
        }

        internal void AddBuckets(IEnumerable<RiakString> buckets)
        {
            this.buckets.AddRange(buckets);
        }
    }
}
