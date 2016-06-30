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
        private static readonly string TsQueryRespAtom = "tsqueryresp";
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
                byte tag = s.Peek();
                switch (tag)
                {
                    case OtpExternal.AtomTag:
                        string atom = s.ReadAtom();
                        decodedResponse = DecodeResponseAtom(atom);
                        break;
                    case OtpExternal.SmallTupleTag:
                    case OtpExternal.LargeTupleTag:
                        decodedResponse = DecodeResponseTuple(s);
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
                rows = tsrows.Select(tsr => new Row(tsr));
            }

            decodedResponse = new DecodedResponse(cols, rows);
        }

        public DecodedResponse Decode()
        {
            return decodedResponse;
        }

        private static DecodedResponse DecodeResponseAtom(string atom)
        {
            if (atom.Equals(TsQueryRespAtom) == false)
            {
                throw new InvalidOperationException("Expected tsqueryresp atom.");
            }

            var cols = Enumerable.Empty<Column>();
            var rows = Enumerable.Empty<Row>();
            return new DecodedResponse(cols, rows);
        }

        private static DecodedResponse DecodeResponseTuple(OtpInputStream s)
        {
            // Response is:
            // {'tsgetresp', {ColNames, ColTypes, Rows}}
            // {'tsqueryresp', {ColNames, ColTypes, Rows}}
            int arity = s.ReadTupleHead();
            if (arity != 2)
            {
                throw new InvalidOperationException("Expected response to be a 2-tuple");
            }

            string atom = s.ReadAtom();
            if (atom.Equals(TsQueryRespAtom) == false)
            {
                string msg = string.Format("Expected tsqueryresp atom, got {0}", atom);
                throw new InvalidOperationException(msg);
            }

            arity = s.ReadTupleHead();
            if (arity != 3)
            {
                throw new InvalidOperationException("Second item in response tuple must be a 3-tuple.");
            }

            IEnumerable<Column> rv_cols = Enumerable.Empty<Column>();
            IEnumerable<Row> rv_rows = Enumerable.Empty<Row>();

            ErlList colNames = rt[0] as ErlList;
            ErlList colTypes = rt[1] as ErlList;
            if (colNames != null && colTypes != null)
            {
                if (colNames.Count > 0)
                {
                    var columns = new List<Column>();
                    for (int i = 0; i < colNames.Count; i++)
                    {
                        var columnNameBin = (ErlBinary)colNames[i];
                        string cname = columnNameBin.ValueAsString;

                        var columnTypeAtom = (ErlAtom)colTypes[i];

                        ColumnType ct = (ColumnType)Enum.Parse(typeof(ColumnType), columnTypeAtom.ValueAsString, true);
                        columns.Add(new Column(cname, ct));
                    }

                    rv_cols = columns;
                }
            }

            /*
            ErlList erows = rt[2] as ErlList;
            if (erows != null)
            {
                if (erows.Count > 0)
                {
                    var rows = new List<Row>();
                    for (int i = 0; i < erows.Count; i++)
                    {
                        ErlTuple erow = (ErlTuple)erows[i];
                        var cells = new List<Cell>();
                        for (int j = 0; j < erow.Count; j++)
                        {
                            IErlObject ecell = erow[j];
                            if (ecell is ErlList)
                            {
                                cells.Add(Cell.Null);
                            }
                            else if (ecell is ErlBoolean)
                            {
                                cells.Add(new Cell(ecell.ValueAsBool));
                            }
                            else if (ecell is ErlBinary)
                            {
                                cells.Add(new Cell(ecell.ValueAsString));
                            }
                            else if (ecell is ErlLong || ecell is ErlByte)
                            {
                                cells.Add(new Cell(ecell.ValueAsLong));
                            }
                            else if (ecell is ErlDouble)
                            {
                                cells.Add(new Cell(ecell.ValueAsDouble));
                            }
                            else if (ecell is ErlBoolean)
                            {
                                cells.Add(new Cell(ecell.ValueAsBool));
                            }
                            else
                            {
                                string msg = string.Format("Can't convert bert value: {0}", ecell.ToString());
                                throw new InvalidOperationException(msg);
                            }
                        }

                        rows.Add(new Row(cells));
                    }

                    rv_rows = rows;
                }
            }
            */

            return new DecodedResponse(rv_cols, rv_rows);
        }
    }
}
