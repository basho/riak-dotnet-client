namespace RiakClient.Commands.KV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Util;

    /// <summary>
    /// Fetches a Map from Riak
    /// </summary>
    public class FetchPreflist : Command<FetchPreflistOptions, PreflistResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchPreflist"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchPreflistOptions"/></param>
        public FetchPreflist(FetchPreflistOptions options)
            : base(options)
        {
        }

        public override MessageCode ExpectedCode
        {
            get { return MessageCode.RpbGetBucketKeyPreflistResp; }
        }

        public override RiakReq ConstructRequest()
        {
            var req = new RpbGetBucketKeyPreflistReq();

            req.type = CommandOptions.BucketType;
            req.bucket = CommandOptions.Bucket;
            req.key = CommandOptions.Key;

            return req;
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = new PreflistResponse();
            }
            else
            {
                RpbGetBucketKeyPreflistResp resp = (RpbGetBucketKeyPreflistResp)response;

                IEnumerable<PreflistItem> preflistItems = Enumerable.Empty<PreflistItem>();

                if (EnumerableUtil.NotNullOrEmpty(resp.preflist))
                {
                    preflistItems = resp.preflist.Select(i =>
                        new PreflistItem(RiakString.FromBytes(i.node), i.partition, i.primary));
                }

                Response = new PreflistResponse(Options.Key, preflistItems);
            }
        }

        /// <inheritdoc />
        public class Builder
            : KvCommandBuilder<Builder, FetchPreflist, FetchPreflistOptions>
        {
            public override FetchPreflist Build()
            {
                Options = BuildOptions();
                return new FetchPreflist(Options);
            }
        }
    }
}