namespace RiakClient.Commands.TS
{
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Util;

    internal class ResponseDecoder
    {
        private readonly IEnumerable<TsColumnDescription> tscols;
        private readonly IEnumerable<TsRow> tsrows;
        
        public ResponseDecoder(TsQueryResp response)
            : this(response.columns, response.rows)
        {
        }

        public ResponseDecoder(TsGetResp response)
            : this(response.columns, response.rows)
        {
        }

        private ResponseDecoder(
            IEnumerable<TsColumnDescription> tscols,
            IEnumerable<TsRow> tsrows)
        {
            this.tscols = tscols;
            this.tsrows = tsrows;
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
