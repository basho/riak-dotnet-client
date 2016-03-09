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

        internal Row(TsRow tsr)
        {
            cells = tsr.cells.Select(tsc => Cell.FromTsCell(tsc));
        }

        public IEnumerable<Cell> Cells
        {
            get { return cells; }
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
                int result = base.GetHashCode();

                foreach (Cell cell in cells)
                {
                    result = (result * 397) ^ cell.GetHashCode();
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
