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
    public class Store : Command<StoreOptions, StoreResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Store"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="StoreOptions"/></param>
        public Store(StoreOptions options)
            : base(options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (EnumerableUtil.IsNullOrEmpty(options.Rows))
            {
                throw new ArgumentNullException("Rows", "Rows can not be null or empty");
            }
        }

        public override MessageCode RequestCode
        {
            get { return MessageCode.TsPutReq; }
        }

        public override MessageCode ResponseCode
        {
            get { return MessageCode.TsPutResp; }
        }

        public override Type ResponseType
        {
            get { return typeof(TsPutResp); }
        }

        public override RpbReq ConstructPbRequest()
        {
            var req = new TsPutReq();

            req.table = CommandOptions.Table;

            if (EnumerableUtil.NotNullOrEmpty(CommandOptions.Columns))
            {
                req.columns.AddRange(CommandOptions.Columns.Select(c => c.ToTsColumn()));
            }

            req.rows.AddRange(CommandOptions.Rows.Select(r => r.ToTsRow()));

            return req;
        }

        public override void OnSuccess(RpbResp response)
        {
            Response = new StoreResponse();
        }

        /// <inheritdoc />
        public class Builder
            : TimeseriesCommandBuilder<Builder, Store, StoreOptions>
        {
            private IEnumerable<Column> columns;
            private IEnumerable<Row> rows;

            public Builder WithColumns(IEnumerable<Column> columns)
            {
                if (EnumerableUtil.IsNullOrEmpty(columns))
                {
                    throw new ArgumentNullException("columns", "columns are required");
                }

                this.columns = columns;
                return this;
            }

            public Builder WithRows(IEnumerable<Row> rows)
            {
                if (EnumerableUtil.IsNullOrEmpty(rows))
                {
                    throw new ArgumentNullException("rows", "rows are required");
                }

                this.rows = rows;
                return this;
            }

            public Builder WithRow(Row row)
            {
                if (row == null)
                {
                    throw new ArgumentNullException("row", "row is required");
                }

                rows = new[] { row };
                return this;
            }

            protected override void PopulateOptions(StoreOptions options)
            {
                options.Columns = columns;
                options.Rows = rows;
            }
        }
    }
}
