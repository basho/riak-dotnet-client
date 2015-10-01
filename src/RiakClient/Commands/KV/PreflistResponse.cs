namespace RiakClient.Commands.KV
{
    using System.Collections.Generic;

    /// <summary>
    /// Response to a <see cref="FetchPreflist"/> command.
    /// </summary>
    public class PreflistResponse : Response<IEnumerable<PreflistItem>>
    {
        /// <inheritdoc />
        public PreflistResponse()
            : base(notFound: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreflistResponse"/> class.
        /// </summary>
        /// <param name="key">A <see cref="RiakString"/> representing the key.</param>
        /// <param name="value">The value of the fetched preflist.</param>
        public PreflistResponse(RiakString key, IEnumerable<PreflistItem> value)
            : base(key, value)
        {
        }
    }
}
