namespace RiakClient.Commands.TS
{
    using System;
    using Messages;

    public class Cell<TValue> : Cell
    {
        private readonly Type vtype = typeof(TValue);
        private readonly TValue v;

        public Cell(TValue value)
            : base(value)
        {
            v = value;
        }

        public TValue Value
        {
            get { return v; }
        }

        internal static Cell From(TsCell tsc)
        {
            throw new NotImplementedException();
        }

        internal override TsCell ToTsCell()
        {
            TsCell rv = null;

            if (ReferenceEquals(null, v))
            {
                return new TsCell();
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
                    timestamp_value = dt.ToUnixTimeMillis()
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
