namespace RiakClient.Commands.TS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Util;

    internal class ResponseDecoder
    {
        // private static readonly ErlAtom TsQueryRespAtom = new ErlAtom("tsqueryresp");

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
            /*
            using (var istream = new ErlInputStream(response.Response, checkVersion: true))
            {
                bool isAtom = istream.PeekAtom();
                if (isAtom)
                {
                    ErlAtom atom = istream.ReadAtom();
                    decodedResponse = DecodeResponseAtom(atom);
                    return;
                }

                bool isTuple = istream.PeekTuple();
                if (isTuple)
                {
                    ErlTuple tuple = istream.ReadTuple();
                    decodedResponse = DecodeResponseTuple(tuple);
                    return;
                }

                throw new InvalidOperationException("Expected an atom or tuple.");
            }
            */
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

        // private static DecodedResponse DecodeResponseAtom(ErlAtom resp)
        private static DecodedResponse DecodeResponseAtom(object resp)
        {
            /*
            if (resp.Equals(TsQueryRespAtom) == false)
            {
                throw new InvalidOperationException("Expected tsqueryresp atom.");
            }
            */

            var cols = Enumerable.Empty<Column>();
            var rows = Enumerable.Empty<Row>();
            return new DecodedResponse(cols, rows);
        }

        private static DecodedResponse DecodeResponseTuple(object resp)
        {
            IEnumerable<Column> rv_cols = Enumerable.Empty<Column>();
            IEnumerable<Row> rv_rows = Enumerable.Empty<Row>();
            return new DecodedResponse(rv_cols, rv_rows);
            /*
            // Response is:
            // {'tsgetresp', {ColNames, ColTypes, Rows}}
            // {'tsqueryresp', {ColNames, ColTypes, Rows}}
            if (resp.Count != 2)
            {
                throw new InvalidOperationException("Expected response to be a 2-tuple");
            }

            if ((resp[0] is ErlAtom) == false)
            {
                throw new InvalidOperationException("First item in response tuple must be an atom.");
            }

            ErlAtom atom = (ErlAtom)resp[0];
            if (atom.Equals(TsQueryRespAtom) == false)
            {
                string msg = string.Format("Expected tsqueryresp atom, got {0}", atom);
                throw new InvalidOperationException(msg);
            }

            if (resp[1].IsScalar)
            {
                throw new InvalidOperationException("Second item in response tuple must not be a scalar.");
            }

            ErlTuple rt = resp[1] as ErlTuple;
            if (rt == null)
            {
                throw new InvalidOperationException("Second item in response tuple must be a tuple.");
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

            // return new DecodedResponse(rv_cols, rv_rows);
        }
    }
}
