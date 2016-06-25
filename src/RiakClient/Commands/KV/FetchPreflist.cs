namespace RiakClient.Commands.KV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Util;

    /// <summary>
    /// Fetches an object's preflist from Riak
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

        public override MessageCode RequestCode
        {
            get { return MessageCode.RpbGetBucketKeyPreflistReq; }
        }

        public override MessageCode ResponseCode
        {
            get { return MessageCode.RpbGetBucketKeyPreflistResp; }
        }

        public override Type ResponseType
        {
            get { return typeof(RpbGetBucketKeyPreflistResp); }
        }

        public override RpbReq ConstructPbRequest()
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
            : CommandBuilder<Builder, FetchPreflist, FetchPreflistOptions>
        {
            // TODO 3.0 KvCommandBuilder?
            private string bucketType;
            private string bucket;
            private string key;

            public override IRCommand Build()
            {
                Options = BuildOptions();
                return new FetchPreflist(Options);
            }

            public Builder WithBucketType(string bucketType)
            {
                if (string.IsNullOrWhiteSpace(bucketType))
                {
                    throw new ArgumentNullException("bucketType");
                }

                this.bucketType = bucketType;
                return this;
            }

            public Builder WithBucket(string bucket)
            {
                if (string.IsNullOrWhiteSpace(bucket))
                {
                    throw new ArgumentNullException("bucket");
                }

                this.bucket = bucket;
                return this;
            }

            public Builder WithKey(string key)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException("key");
                }

                this.key = key;
                return this;
            }

            protected override FetchPreflistOptions BuildOptions()
            {
                return new FetchPreflistOptions(bucketType, bucket, key);
            }
        }
    }
}
