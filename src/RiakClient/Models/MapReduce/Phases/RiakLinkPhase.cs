namespace RiakClient.Models.MapReduce.Phases
{
    using Extensions;
    using Newtonsoft.Json;

    internal class RiakLinkPhase : RiakPhase
    {
        private string bucket;
        private string tag;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakLinkPhase"/> class.
        /// </summary>
        public RiakLinkPhase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakLinkPhase"/> class.
        /// </summary>
        /// <param name="riakLink">
        /// The <see cref="RiakLink"/> containing a bucket and tag to limit linkwalking to.
        /// If the <see cref="RiakLink"/>'s Bucket, Key, and Tag are all null or whitespace, then this phase will follow all links.
        /// </param>
        public RiakLinkPhase(RiakLink riakLink)
        {
            if (string.IsNullOrWhiteSpace(riakLink.Bucket)
                && string.IsNullOrWhiteSpace(riakLink.Key)
                && string.IsNullOrWhiteSpace(riakLink.Tag))
            {
                AllLinks();
            }
            else
            {
                bucket = riakLink.Bucket;
                tag = riakLink.Tag;
            }
        }

        /// <inheritdoc/>
        public override string PhaseType
        {
            get { return "link"; }
        }

        /// <summary>
        /// Configure this Link phase to follow only links with buckets and tags that match those in <paramref name="link"/>.
        /// </summary>
        /// <param name="riakLink">
        /// The <see cref="RiakLink"/> containing a bucket and tag to limit linkwalking to.
        /// If the <see cref="RiakLink"/>'s Bucket, Key, and Tag are all null or whitespace, then this phase will follow all links.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakLinkPhase FromRiakLink(RiakLink riakLink)
        {
            if (string.IsNullOrWhiteSpace(riakLink.Bucket)
                && string.IsNullOrWhiteSpace(riakLink.Key)
                && string.IsNullOrWhiteSpace(riakLink.Tag))
            {
                AllLinks();
            }
            else
            {
                bucket = riakLink.Bucket;
                tag = riakLink.Tag;
            }

            return this;
        }

        /// <summary>
        /// Configure this Link phase to follow only links that go to a certain <paramref name="bucket"/>.
        /// </summary>
        /// <param name="bucket">The bucket to limit linkwalking to.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakLinkPhase Bucket(string bucket)
        {
            this.bucket = bucket;
            return this;
        }

        /// <summary>
        /// Configure this Link phase to follow only links with a certain <paramref name="tag"/>.
        /// </summary>
        /// <param name="tag">The tag to limit linkwalking to.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakLinkPhase Tag(string tag)
        {
            this.tag = tag;
            return this;
        }

        /// <summary>
        /// Configure this Link phase to follow all Links found on input objects.
        /// </summary>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakLinkPhase AllLinks()
        {
            bucket = default(string);
            tag = default(string);
            Keep(false);
            return this;
        }

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteSpecifiedProperty("bucket", bucket)
                .WriteSpecifiedProperty("tag", tag);
        }
    }
}
