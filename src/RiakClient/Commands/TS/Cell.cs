namespace RiakClient.Commands.TS
{
    using System;
    using Messages;
    using Util;

    public class Cell : IEquatable<Cell>
    {
        private readonly object value;

        protected Cell()
        {
            value = null;
        }

        protected Cell(object value)
        {
            this.value = value;
        }

        public object AsObject
        {
            get { return value; }
        }

        public bool Equals(Cell other)
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
            return base.Equals(obj as Cell);
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses a combination of the public properties to generate a unique hash code.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            if (ReferenceEquals(null, value))
            {
                return base.GetHashCode();
            }
            else
            {
                return value.GetHashCode();
            }
        }

        internal static Cell FromTsCell(TsCell tsc)
        {
            if (tsc.boolean_valueSpecified)
            {
                return new Cell<bool>(tsc.boolean_value);
            }
            else if (tsc.double_valueSpecified)
            {
                return new Cell<double>(tsc.double_value);
            }
            else if (tsc.sint64_valueSpecified)
            {
                return new Cell<long>(tsc.sint64_value);
            }
            else if (tsc.timestamp_valueSpecified)
            {
                return new Cell<DateTime>(
                    DateTimeUtil.FromUnixTimeMillis(tsc.timestamp_value));
            }
            else if (tsc.varchar_valueSpecified)
            {
                return new Cell<string>(RiakString.FromBytes(tsc.varchar_value));
            }

            return new Cell();
        }

        internal virtual TsCell ToTsCell()
        {
            return new TsCell();
        }
    }
}
