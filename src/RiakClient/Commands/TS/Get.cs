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
    [CLSCompliant(false)]
    public class Get : ByKeyCommand<GetResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Get"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="ByKeyOptions"/></param>
        public Get(ByKeyOptions options)
            : base(options)
        {
        }

        public override MessageCode ExpectedCode
        {
            get { return MessageCode.TsGetResp; }
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

        protected override ITsByKeyReq GetByKeyReq()
        {
            return new TsGetReq();
        }

        /// <inheritdoc />
        public class Builder
            : Builder<Get>
        {
        }
    }
}
