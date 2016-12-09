namespace RiakClient.Commands.TS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Util;

    public class Row : IEquatable<Row>
    {
        private readonly IEnumerable<Cell> cells;

        public Row(IEnumerable<Cell> cells)
        {
            this.cells = cells;
        }

        internal Row(TsRow tsr, TsColumnDescription[] tscols = null)
        {
            Cell[] cary = new Cell[tsr.cells.Count];
            for (int i = 0; i < tsr.cells.Count; ++i)
            {
                TsCell tsc = tsr.cells[i];
                TsColumnType tsct = TsColumnType.VARCHAR;
                if (EnumerableUtil.NotNullOrEmpty(tscols))
                {
                    tsct = tscols[i].type;
                }

                Cell c = Cell.FromTsCell(tsc, tsct);
                cary[i] = c;
            }

            cells = cary;
        }

        public ICollection<Cell> Cells
        {
            get { return cells.ToArray(); }
        }

        public bool Equals(Row other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as Row);
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses a combination of the public properties to generate a unique hash code.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            if (EnumerableUtil.IsNullOrEmpty(cells))
            {
                return base.GetHashCode();
            }

            unchecked
            {
                int result = 1;
                bool hashed = false;

                if (EnumerableUtil.NotNullOrEmpty(cells))
                {
                    hashed = true;
                    foreach (Cell cell in cells)
                    {
                        result = (result * 397) ^ cell.GetHashCode();
                    }
                }

                if (!hashed)
                {
                    result = base.GetHashCode();
                }

                return result;
            }
        }

        internal TsRow ToTsRow()
        {
            var rv = new TsRow();
            rv.cells.AddRange(ToTsCells());
            return rv;
        }

        internal IEnumerable<TsCell> ToTsCells()
        {
            return cells.Select(c => c.ToTsCell());
        }
    }
}
