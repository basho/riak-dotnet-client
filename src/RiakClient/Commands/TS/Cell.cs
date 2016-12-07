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
        private readonly byte[] varcharValue;
        private readonly long sint64Value;
        private readonly double doubleValue;
        private readonly long timestampValue;
        private readonly bool booleanValue;

        public Cell()
        {
            isNull = true;
            valueType = ColumnType.Null;
        }

        public Cell(object value, ColumnType valueType)
        {
            if (value == null)
            {
                isNull = true;
                this.valueType = ColumnType.Null;
            }
            else
            {
                this.valueType = valueType;
                switch (valueType)
                {
                    case ColumnType.Boolean:
                        booleanValue = Convert.ToBoolean(value);
                        break;

                    case ColumnType.Double:
                        doubleValue = Convert.ToDouble(value);
                        break;

                    case ColumnType.SInt64:
                        sint64Value = Convert.ToInt64(value);
                        break;

                    case ColumnType.Timestamp:
                        if (value is DateTime)
                        {
                            timestampValue = DateTimeUtil.ToUnixTimeMillis((DateTime)value);
                        }
                        else
                        {
                            timestampValue = Convert.ToInt64(value);
                        }

                        break;

                    case ColumnType.Varchar:
                    case ColumnType.Blob:
                        var bytes = value as byte[];
                        if (bytes != null)
                        {
                            varcharValue = bytes;
                        }
                        else
                        {
                            var s = Convert.ToString(value);
                            varcharValue = RiakString.ToBytes(s);
                        }

                        break;

                    default:
                        string msg = string.Format("Unknown value type: {0}", valueType);
                        throw new ArgumentException(msg);
                }
            }
        }

        public Cell(byte[] value)
        {
            if (EnumerableUtil.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value", "Value must not be null or empty. Use the zero-arg ctor for null cells.");
            }

            varcharValue = value;
            valueType = ColumnType.Varchar;
        }

        public Cell(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Value must not be null.");
            }

            varcharValue = RiakString.ToBytes(value);
            valueType = ColumnType.Varchar;
        }

        public Cell(long value, bool isUnixTimestamp = false)
        {
            if (isUnixTimestamp)
            {
                timestampValue = value;
                valueType = ColumnType.Timestamp;
            }
            else
            {
                sint64Value = value;
                valueType = ColumnType.SInt64;
            }
        }

        public Cell(double value)
        {
            doubleValue = value;
            valueType = ColumnType.Double;
        }

        public Cell(DateTime value)
        {
            timestampValue = DateTimeUtil.ToUnixTimeMillis(value);
            valueType = ColumnType.Timestamp;
        }

        public Cell(bool value)
        {
            booleanValue = value;
            valueType = ColumnType.Boolean;
        }

        public ColumnType ValueType
        {
            get { return valueType; }
        }

        public bool ValueAsBoolean
        {
            get
            {
                if (isNull)
                {
                    throw new InvalidOperationException("Can't decode value as Boolean.");
                }

                if (valueType == ColumnType.Boolean)
                {
                    return booleanValue;
                }

                throw new InvalidOperationException("Can't decode value as Boolean.");
            }
        }

        public double ValueAsDouble
        {
            get
            {
                if (isNull)
                {
                    throw new InvalidOperationException("Can't decode value as Double.");
                }

                if (valueType == ColumnType.Double)
                {
                    return doubleValue;
                }

                throw new InvalidOperationException("Can't decode value as Double.");
            }
        }

        public long ValueAsLong
        {
            get
            {
                if (isNull)
                {
                    throw new InvalidOperationException("Can't decode value as Long.");
                }

                if (valueType == ColumnType.Timestamp)
                {
                    return timestampValue;
                }

                if (valueType == ColumnType.SInt64)
                {
                    return sint64Value;
                }

                throw new InvalidOperationException("Can't decode value as Long.");
            }
        }

        public long ValueAsTimestamp
        {
            get
            {
                if (isNull)
                {
                    throw new InvalidOperationException("Can't decode value as Timestamp.");
                }

                if (valueType == ColumnType.Timestamp)
                {
                    return timestampValue;
                }

                throw new InvalidOperationException("Can't decode value as Timestamp.");
            }
        }

        public DateTime ValueAsDateTime
        {
            get
            {
                long ts = ValueAsTimestamp;
                return DateTimeUtil.FromUnixTimeMillis(ts);
            }
        }

        public byte[] ValueAsBytes
        {
            get
            {
                if (valueType == ColumnType.Varchar || valueType == ColumnType.Blob)
                {
                    return varcharValue;
                }

                throw new InvalidOperationException("Can't decode value as byte array.");
            }
        }

        public string ValueAsString
        {
            get
            {
                string s = null;
                byte[] bytes = ValueAsBytes;
                if (bytes != null)
                {
                    s = RiakString.FromBytes(varcharValue);
                }

                return s;
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
                    case ColumnType.Blob:
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
                if (valueType == ColumnType.Varchar || valueType == ColumnType.Blob)
                {
                    for (int i = 0; i < varcharValue.Length; i++)
                    {
                        result = (result * 397) ^ varcharValue[i].GetHashCode();
                    }
                }
                else
                {
                    result = (result * 397) ^ Value.GetHashCode();
                }

                return result;
            }
        }

        public override string ToString()
        {
            if (Value == null)
            {
                return "null";
            }
            else
            {
                return Value.ToString();
            }
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
                return new Cell(tsc.timestamp_value, isUnixTimestamp: true);
            }
            else if (tsc.varchar_valueSpecified)
            {
                return new Cell(tsc.varchar_value);
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

            if (valueType == ColumnType.Varchar || valueType == ColumnType.Blob)
            {
                os.WriteBinary(varcharValue);
            }
            else if (valueType == ColumnType.SInt64)
            {
                os.WriteLong(sint64Value);
            }
            else if (valueType == ColumnType.Timestamp)
            {
                os.WriteLong(timestampValue);
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
                    return new TsCell { timestamp_value = timestampValue };
                case ColumnType.Varchar:
                case ColumnType.Blob:
                    return new TsCell { varchar_value = varcharValue };
                default:
                    throw new InvalidOperationException("Could not convert to TsCell.");
            }
        }
    }
}
