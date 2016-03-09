namespace RiakClient.Commands.TS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Util;

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
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (options.Key == null)
            {
                throw new ArgumentNullException("options.Key", "options.Key can not be null");
            }

            if (EnumerableUtil.IsNullOrEmpty(options.Key.Cells))
            {
                throw new ArgumentNullException("options.Key.Cells", "options.Key.Cells can not be null or empty");
            }
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

                IEnumerable<Column> cols = Enumerable.Empty<Column>();

                if (EnumerableUtil.NotNullOrEmpty(resp.columns))
                {
                    cols = resp.columns.Select(tsc =>
                        new Column(RiakString.FromBytes(tsc.name), (ColumnType)tsc.type));
                }

                IEnumerable<Row> rows = Enumerable.Empty<Row>();

                if (EnumerableUtil.NotNullOrEmpty(resp.rows))
                {
                    rows = resp.rows.Select(tsr => new Row(tsr));
                }

                Response = new GetResponse(CommandOptions.Key, cols, rows);
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
