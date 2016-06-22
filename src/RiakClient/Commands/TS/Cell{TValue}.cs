namespace RiakClient.Commands.TS
{
    using System;
    using Messages;
    using Util;

    public class Cell<TValue> : Cell, IEquatable<Cell<TValue>>
    {
        private readonly Type vtype = typeof(TValue);
        private readonly TValue v;

        public Cell(TValue value)
            : base(value)
        {
            if (ReferenceEquals(null, value))
            {
                throw new ArgumentNullException("value", "typed Cells require a non-null value");
            }

            v = value;
        }

        public TValue Value
        {
            get { return v; }
        }

        public bool Equals(Cell<TValue> other)
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
            return base.Equals(obj as Cell<TValue>);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        internal override TsCell ToTsCell()
        {
            TsCell rv = null;

            if (ReferenceEquals(null, v))
            {
                throw new InvalidOperationException("typed Cells require a non-null value");
            }

            if (vtype == typeof(string))
            {
                rv = new TsCell
                {
                    varchar_value = RiakString.ToBytes((string)Convert.ChangeType(v, TypeCode.String))
                };
            }
            else if (vtype == typeof(byte[]))
            {
                var bytes = (byte[])AsObject;
                rv = new TsCell
                {
                    varchar_value = bytes
                };
            }
            else if (IsIntegerType(vtype))
            {
                rv = new TsCell
                {
                    sint64_value = (long)Convert.ChangeType(v, TypeCode.Int64)
                };
            }
            else if (vtype == typeof(DateTime))
            {
                var dt = (DateTime)Convert.ChangeType(v, TypeCode.DateTime);
                rv = new TsCell
                {
                    timestamp_value = DateTimeUtil.ToUnixTimeMillis(dt)
                };
            }
            else if (vtype == typeof(bool))
            {
                rv = new TsCell
                {
                    boolean_value = (bool)Convert.ChangeType(v, TypeCode.Boolean)
                };
            }
            else if (IsDoubleType(vtype))
            {
                rv = new TsCell
                {
                    double_value = (double)Convert.ChangeType(v, TypeCode.Double)
                };
            }
            else
            {
                string msg = string.Format("could not convert {0}, type: {1}", v.ToString(), v.GetType().Name);
                throw new InvalidOperationException(msg);
            }

            return rv;
        }

        private static bool IsIntegerType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsBigIntegerType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.UInt64:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsDoubleType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
