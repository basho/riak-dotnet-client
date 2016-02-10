namespace RiakClient.Commands.KV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Util;

    /// <summary>
    /// Fetches an list of buckets from a bucket type.
    /// </summary>
    public class ListBuckets : Command<ListBucketsOptions, ListBucketsResponse>, IStreamingCommand
    {
        private bool done = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBuckets"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="ListBucketsOptions"/></param>
        public ListBuckets(ListBucketsOptions options)
            : base(options)
        {
        }

        public bool Done
        {
            get
            {
                if (CommandOptions.Stream)
                {
                    return done;
                }

                return true;
            }
        }

        public override MessageCode RequestCode
        {
            get { return MessageCode.RpbListBucketsReq; }
        }

        public override MessageCode ResponseCode
        {
            get { return MessageCode.RpbListBucketsResp; }
        }

        public override Type ResponseType
        {
            get { return typeof(RpbListBucketsResp); }
        }

        public override RpbReq ConstructPbRequest()
        {
            var req = new RpbListBucketsReq();

            req.type = CommandOptions.BucketType;
            req.stream = CommandOptions.Stream;
            req.timeout = (uint)CommandOptions.Timeout.TotalMilliseconds;

            return req;
        }

        public override void OnSuccess(RpbResp response)
        {
            IEnumerable<RiakString> buckets = Enumerable.Empty<RiakString>();

            if (Response == null)
            {
                Response = new ListBucketsResponse();
            }

            if (response == null)
            {
                done = true;
            }
            else
            {
                var resp = (RpbListBucketsResp)response;

                done = resp.done;

                if (EnumerableUtil.NotNullOrEmpty(resp.buckets))
                {
                    buckets = resp.buckets.Select(i => RiakString.FromBytes(i));
                }

                if (CommandOptions.Stream)
                {
                    CommandOptions.Callback(buckets);
                }
                else
                {
                    Response.AddBuckets(buckets);
                }
            }
        }

        /// <inheritdoc />
        public class Builder
            : CommandBuilder<ListBuckets.Builder, ListBuckets, ListBucketsOptions>
        {
            public override ListBuckets Build()
            {
                Options = BuildOptions();
                return new ListBuckets(Options);
            }
        }
    }
}
