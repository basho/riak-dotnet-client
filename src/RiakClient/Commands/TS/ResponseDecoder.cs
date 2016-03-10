namespace RiakClient.Commands.TS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Util;

    internal class ResponseDecoder
    {
        private readonly IEnumerable<TsColumnDescription> tscols;
        private readonly IEnumerable<TsRow> tsrows;

        public ResponseDecoder(RpbResp response)
        {
            TsGetResp gr = response as TsGetResp;
            if (gr != null)
            {
                tscols = gr.columns;
                tsrows = gr.rows;
                return;
            }

            TsQueryResp qr = response as TsQueryResp;
            if (qr != null)
            {
                tscols = qr.columns;
                tsrows = qr.rows;
                return;
            }

            var msg = string.Format("Can't decode message of type: {0}", response.GetType().Name);
            throw new InvalidOperationException(msg);
        }

        public DecodedResponse Decode()
        {
            IEnumerable<Column> cols = Enumerable.Empty<Column>();

            if (EnumerableUtil.NotNullOrEmpty(tscols))
            {
                cols = tscols.Select(tsc =>
                    new Column(RiakString.FromBytes(tsc.name), (ColumnType)tsc.type));
            }

            IEnumerable<Row> rows = Enumerable.Empty<Row>();

            if (EnumerableUtil.NotNullOrEmpty(tsrows))
            {
                rows = tsrows.Select(tsr => new Row(tsr));
            }

            return new DecodedResponse(cols, rows);
        }
    }
}
