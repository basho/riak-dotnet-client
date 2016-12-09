namespace RiakClient.Commands.TS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Erlang;
    using Messages;
    using Util;

    internal class ResponseDecoder
    {
        private const string TsGetRespAtom = "tsgetresp";
        private const string TsQueryRespAtom = "tsqueryresp";
        private readonly DecodedResponse decodedResponse;

        public ResponseDecoder(TsQueryResp response)
            : this(response.columns, response.rows)
        {
        }

        public ResponseDecoder(TsGetResp response)
            : this(response.columns, response.rows)
        {
        }

        public ResponseDecoder(TsTtbResp response)
        {
            using (var s = new OtpInputStream(response.Response))
            {
                byte tag = s.Read1();
                if (tag != OtpExternal.VersionTag)
                {
                    string msg = string.Format(
                        "Expected OTP input stream to start with {0}, got {1}",
                        OtpExternal.VersionTag,
                        tag);
                    throw new InvalidOperationException(msg);
                }

                tag = s.Peek();
                switch (tag)
                {
                    case OtpExternal.AtomTag:
                        string atom = s.ReadAtom();
                        decodedResponse = ParseAtomResult(atom);
                        break;
                    case OtpExternal.SmallTupleTag:
                    case OtpExternal.LargeTupleTag:
                        decodedResponse = ParseTupleResult(s);
                        break;
                    default:
                        throw new InvalidOperationException("Expected an atom or tuple.");
                }
            }
        }

        private ResponseDecoder(
            IEnumerable<TsColumnDescription> tscols,
            IEnumerable<TsRow> tsrows)
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
                rows = tsrows.Select(tsr => new Row(tsr, tscols.ToArray()));
            }

            decodedResponse = new DecodedResponse(cols, rows);
        }

        public DecodedResponse Decode()
        {
            return decodedResponse;
        }

        private static DecodedResponse ParseAtomResult(string atom)
        {
            if (atom.Equals(TsQueryRespAtom) == false)
            {
                throw new InvalidOperationException("Expected tsqueryresp atom.");
            }

            var cols = Enumerable.Empty<Column>();
            var rows = Enumerable.Empty<Row>();
            return new DecodedResponse(cols, rows);
        }

        private static DecodedResponse ParseTupleResult(OtpInputStream s)
        {
            // Response is:
            // {'tsgetresp', {ColNames, ColTypes, Rows}}
            // {'tsqueryresp', {ColNames, ColTypes, Rows}}
            int arity = s.ReadTupleHead();
            if (arity != 2)
            {
                throw new InvalidOperationException("Expected response to be a 2-tuple");
            }

            string msg;
            string atom = s.ReadAtom();
            switch (atom)
            {
                case TsGetRespAtom:
                case TsQueryRespAtom:
                    arity = s.ReadTupleHead();
                    if (arity != 3)
                    {
                        msg = string.Format(
                            "Second item in {0} response tuple must be a 3-tuple.",
                            atom);
                        throw new InvalidOperationException(msg);
                    }

                    Column[] cols = ParseColumns(s);
                    Row[] rows = ParseRows(s, cols);
                    return new DecodedResponse(cols, rows);
                default:
                    msg = string.Format(
                        "Expected tsgetresp or tsqueryresp atom, got {0}",
                        atom);
                    throw new InvalidOperationException(msg);
            }
        }

        private static Column[] ParseColumns(OtpInputStream s)
        {
            int colNameCount = s.ReadListHead();
            string[] columnNames = new string[colNameCount];
            for (int i = 0; i < colNameCount; i++)
            {
                columnNames[i] = s.ReadBinaryAsString(); 
            }

            if (colNameCount > 0)
            {
                s.ReadNil();
            }

            int colTypeCount = s.ReadListHead();
            ColumnType[] columnTypes = new ColumnType[colTypeCount];
            for (int i = 0; i < colTypeCount; i++)
            {
                string a = s.ReadAtom();
                columnTypes[i] = (ColumnType)Enum.Parse(typeof(ColumnType), a, true);
            }

            if (colTypeCount > 0)
            {
                s.ReadNil();
            }

            return columnNames.Zip(columnTypes, (cname, ctype) => new Column(cname, ctype)).ToArray();
        }

        private static Row[] ParseRows(OtpInputStream s, Column[] cols)
        {
            int rowCount = s.ReadListHead();
            Row[] rows = new Row[rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                rows[i] = ParseRow(s, cols);
            }

            if (rowCount > 0)
            {
                s.ReadNil();
            }

            return rows;
        }

        private static Row ParseRow(OtpInputStream s, Column[] cols)
        {
            int cellCount = s.ReadTupleHead();
            if (cellCount != cols.Length)
            {
                string msg = string.Format(
                    "Expected cell count {0} to equal column count {1}",
                    cellCount,
                    cols.Length);
                throw new InvalidOperationException(msg);
            }

            Cell[] cells = new Cell[cellCount];
            for (int i = 0; i < cellCount; i++)
            {
                cells[i] = ParseCell(s, i, cols);
            }

            return new Row(cells);
        }

        private static Cell ParseCell(OtpInputStream s, int i, Column[] cols)
        {
            string msg;
            Column col = cols[i];
            byte tag = s.Peek();
            switch (tag)
            {
                case OtpExternal.NilTag:
                    tag = s.Read1(); // NB: actually consume the byte
                    return new Cell();

                case OtpExternal.BinTag:
                    if (col.Type == ColumnType.Varchar || col.Type == ColumnType.Blob)
                    {
                        return new Cell(s.ReadBinaryAsString());
                    }
                    else
                    {
                        throw OnBadTag(tag, col, ColumnType.Varchar, ColumnType.Blob);
                    }

                case OtpExternal.SmallIntTag:
                case OtpExternal.IntTag:
                case OtpExternal.SmallBigTag:
                case OtpExternal.LargeBigTag:
                    long val = s.ReadLong();
                    switch (col.Type)
                    {
                        case ColumnType.SInt64:
                            return new Cell(val, isUnixTimestamp: false);
                        case ColumnType.Timestamp:
                            return new Cell(val, isUnixTimestamp: true);
                        default:
                            throw OnBadTag(tag, col, ColumnType.SInt64, ColumnType.Timestamp);
                    }

                case OtpExternal.FloatTag:
                case OtpExternal.NewFloatTag:
                    if (col.Type != ColumnType.Double)
                    {
                        throw OnBadTag(tag, col, ColumnType.Double);
                    }

                    return new Cell(s.ReadDouble());

                case OtpExternal.AtomTag:
                    if (col.Type != ColumnType.Boolean)
                    {
                        throw OnBadTag(tag, col, ColumnType.Boolean);
                    }

                    return new Cell(s.ReadBoolean());

                case OtpExternal.ListTag:
                    int arity = s.ReadNil();
                    if (arity == 0)
                    {
                        // null cell
                        return new Cell();
                    }
                    else
                    {
                        msg = string.Format(
                            "Expected nil list, got one with arity {0}",
                            arity);
                        throw new InvalidOperationException(msg);
                    }

                default:
                    msg = string.Format(
                        "Unknown cell type encountered, tag {0}, '{1}:{2}'",
                        tag,
                        col.Name,
                        col.Type);
                    throw new InvalidOperationException(msg);
            }
        }

        private static Exception OnBadTag(byte tag, Column cgot, params ColumnType[] ctex)
        {
            string msg = string.Format(
                "Expected one of column types {0}, got '{1}:{2}' for data with tag {3}",
                string.Join(", ", ctex),
                cgot.Name,
                cgot.Type,
                tag);
            return new InvalidOperationException(msg);
        }
    }
}
