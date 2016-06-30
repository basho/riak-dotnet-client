namespace RiakClient.Commands.TS
{
    using System;
    using Erlang;
    using Messages;
    using Util;

    public class Cell : IEquatable<Cell>
    {
        public static readonly Cell Null = new Cell();

        private readonly bool isNull = false;
        private readonly ColumnType valueType;
        private readonly string varcharValue;
        private readonly long sint64Value;
        private readonly double doubleValue;
        private readonly DateTime timestampValue;
        private readonly bool booleanValue;

        public Cell()
        {
            isNull = true;
        }

        public Cell(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Value must not be null.");
            }

            varcharValue = value;
            valueType = ColumnType.Varchar;
        }

        public Cell(long value)
        {
            sint64Value = value;
            valueType = ColumnType.SInt64;
        }

        public Cell(double value)
        {
            doubleValue = value;
            valueType = ColumnType.Double;
        }

        public Cell(DateTime value)
        {
            timestampValue = value;
            valueType = ColumnType.Timestamp;
        }

        public Cell(bool value)
        {
            booleanValue = value;
            valueType = ColumnType.Boolean;
        }

        public ColumnType? ValueType
        {
            get
            {
                if (isNull)
                {
                    return null;
                }

                return valueType;
            }
        }

        public object Value
        {
            get
            {
                if (isNull)
                {
                    return null;
                }

                switch (valueType)
                {
                    case ColumnType.Boolean:
                        return booleanValue;
                    case ColumnType.Double:
                        return doubleValue;
                    case ColumnType.SInt64:
                        return sint64Value;
                    case ColumnType.Timestamp:
                        return timestampValue;
                    case ColumnType.Varchar:
                        return varcharValue;
                    default:
                        throw new InvalidOperationException();
                }
            }
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

            if (ReferenceEquals(this, Null) &&
                ReferenceEquals(other, Null))
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
            if (isNull)
            {
                return isNull.GetHashCode();
            }

            unchecked
            {
                int result = valueType.GetHashCode();
                result = (result * 397) ^ Value.GetHashCode();
                return result;
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        internal static Cell FromTsCell(TsCell tsc)
        {
            if (tsc.boolean_valueSpecified)
            {
                return new Cell(tsc.boolean_value);
            }
            else if (tsc.double_valueSpecified)
            {
                return new Cell(tsc.double_value);
            }
            else if (tsc.sint64_valueSpecified)
            {
                return new Cell(tsc.sint64_value);
            }
            else if (tsc.timestamp_valueSpecified)
            {
                return new Cell(
                    DateTimeUtil.FromUnixTimeMillis(tsc.timestamp_value));
            }
            else if (tsc.varchar_valueSpecified)
            {
                string s = RiakString.FromBytes(tsc.varchar_value);
                return new Cell(s);
            }

            return new Cell();
        }

        internal void ToTtbCell(OtpOutputStream os)
        {
            if (isNull)
            {
                os.WriteNil();
                return;
            }

            if (valueType == ColumnType.Varchar)
            {
                os.WriteStringAsBinary(varcharValue);
            }
            else if (valueType == ColumnType.SInt64)
            {
                os.WriteLong(sint64Value);
            }
            else if (valueType == ColumnType.Timestamp)
            {
                long ts = DateTimeUtil.ToUnixTimeMillis(timestampValue);
                os.WriteLong(ts);
            }
            else if (valueType == ColumnType.Boolean)
            {
                os.WriteBoolean(booleanValue);
            }
            else if (valueType == ColumnType.Double)
            {
                os.WriteDouble(doubleValue);
            }
            else
            {
                throw new InvalidOperationException("Could not convert to TTB value.");
            }
        }

        internal TsCell ToTsCell()
        {
            if (isNull)
            {
                return new TsCell();
            }

            switch (valueType)
            {
                case ColumnType.Boolean:
                    return new TsCell { boolean_value = booleanValue };
                case ColumnType.Double:
                    return new TsCell { double_value = doubleValue };
                case ColumnType.SInt64:
                    return new TsCell { sint64_value = sint64Value };
                case ColumnType.Timestamp:
                    long ts = DateTimeUtil.ToUnixTimeMillis(timestampValue);
                    return new TsCell { timestamp_value = ts };
                case ColumnType.Varchar:
                    return new TsCell { varchar_value = RiakString.ToBytes(varcharValue) };
                default:
                    throw new InvalidOperationException("Could not convert to TsCell.");
            }
        }
    }
}
