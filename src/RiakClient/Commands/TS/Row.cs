namespace RiakClient.Commands.TS
{
    using System.Collections.Generic;
    using System.Linq;
    using Messages;

    public class Row
    {
        private readonly IEnumerable<Cell> cells;

        public Row(IEnumerable<Cell> cells)
        {
            this.cells = cells;
        }

        internal IEnumerable<TsCell> ToTsCells()
        {
            return cells.Select(c => c.ToTsCell());
        }
    }
}
