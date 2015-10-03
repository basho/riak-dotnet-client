namespace RiakClient.Commands.KV
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents options for a <see cref="ListBuckets"/> operation.
    /// </summary>
    /// <inheritdoc/>
    public class ListBucketsOptions : CommandOptions
    {
        private readonly bool stream;
        private readonly Action<IEnumerable<RiakString>> callback;

        /// <inheritdoc/>
        public ListBucketsOptions(string bucketType, bool stream, Action<IEnumerable<RiakString>> callback, TimeSpan timeout)
            : base(new Args(bucketType, null, false, null, false), timeout)
        {
            if (stream && callback == null)
            {
                throw new ArgumentNullException("callback", Riak.Properties.Resources.Riak_Commands_StreamingCommandRequiresCallbackException);
            }

            this.stream = stream;
            this.callback = callback;
        }

        public bool Stream
        {
            get { return stream; }
        }

        public Action<IEnumerable<RiakString>> Callback
        {
            get { return callback; }
        }
    }
}
