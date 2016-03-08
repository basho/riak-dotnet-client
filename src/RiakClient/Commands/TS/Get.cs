namespace RiakClient.Commands.TS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;

    /// <summary>
    /// Fetches timeseries data from Riak
    /// </summary>
    public class Get : Command<GetOptions, GetResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Get"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="GetOptions"/></param>
        public Get(GetOptions options)
            : base(options)
        {
        }

        public override MessageCode ExpectedCode
        {
            get { return MessageCode.TsGetResp; }
        }

        public override RpbReq ConstructPbRequest()
        {
            var req = new TsGetReq();

            req.table = CommandOptions.Table;

            req.timeoutSpecified = false;
            if (CommandOptions.Timeout.HasValue)
            {
                req.timeout = (uint)CommandOptions.Timeout;
            }

            req.key.AddRange(CommandOptions.Key.ToTsCells());

            return req;
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = new GetResponse();
            }
            else
            {
                TsGetResp resp = (TsGetResp)response;

                IEnumerable<Row> rows = Enumerable.Empty<Row>();

                /*
                if (EnumerableUtil.NotNullOrEmpty(resp.preflist))
                {
                    preflistItems = resp.preflist.Select(i =>
                        new PreflistItem(RiakString.FromBytes(i.node), i.partition, i.primary));
                }
                */

                Response = new GetResponse(CommandOptions.Key, rows);
            }
        }

        /// <inheritdoc />
        public class Builder
            : TimeseriesCommandBuilder<Builder, Get, GetOptions>
        {
            private Row key;

            public Builder WithKey(Row key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key", "key is required");
                }

                this.key = key;
                return this;
            }

            protected override void PopulateOptions(GetOptions options)
            {
                options.Key = key;
                options.Timeout = timeout;
            }
        }
    }
}
