namespace RiakClient.Commands.TS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Erlang;
    using Messages;
    using Util;

    /// <summary>
    /// Fetches timeseries data from Riak
    /// </summary>
    public class Store : Command<StoreOptions, StoreResponse>, IRiakTtbCommand
    {
        private const string TsPutReqAtom = "tsputreq";

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

        public override MessageCode ExpectedCode
        {
            get { return MessageCode.TsPutResp; }
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

        public Action<MemoryStream> GetSerializer()
        {
            string tableName = CommandOptions.Table;
            ICollection<Row> rows = CommandOptions.Rows;

            return (ms) =>
            {
                using (var os = new OtpOutputStream())
                {
                    os.WriteByte(OtpExternal.VersionTag);

                    // {tsputreq, tableName, emptyList, rows}
                    os.WriteTupleHead(4);
                    os.WriteAtom(TsPutReqAtom);
                    os.WriteStringAsBinary(tableName);
                    os.WriteNil();

                    if (rows.Count > 0)
                    {
                        os.WriteListHead(rows.Count);

                        foreach (Row r in CommandOptions.Rows)
                        {
                            os.WriteTupleHead(r.Cells.Count);
                            foreach (Cell c in r.Cells)
                            {
                                c.ToTtbCell(os);
                            }
                        }

                        os.WriteNil();
                    }
                    else
                    {
                        os.WriteNil();
                    }

                    os.WriteTo(ms);
                }
            };
        }

        public override void OnSuccess(RpbResp response)
        {
            var ttbresp = response as TsTtbResp;
            if (ttbresp != null)
            {
                // TODO check for true?
                // Bert rsp = ttbresp.Response;
            }

            Response = new StoreResponse();
        }

        /// <inheritdoc />
        public class Builder
            : TimeseriesCommandBuilder<Builder, Store, StoreOptions>
        {
            private ICollection<Column> columns;
            private ICollection<Row> rows;

            public Builder WithColumns(ICollection<Column> columns)
            {
                if (EnumerableUtil.IsNullOrEmpty(columns))
                {
                    throw new ArgumentNullException("columns", "columns are required");
                }

                this.columns = columns;
                return this;
            }

            public Builder WithRows(ICollection<Row> rows)
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
