namespace RiakClient.Commands.TS
{
    using System;
    using Messages;

    public class Cell<TValue> : Cell
    {
        private readonly TValue v;

        public Cell(TValue value)
        {
            v = value;
        }

        internal Cell(TsCell tsc)
        {
        }

        public override object AsObject
        {
            get { return v; }
        }

        public TValue Value
        {
            get { return v; }
        }

        internal override TsCell ToTsCell()
        {
            TsCell rv = null;

            if (Equals(v, default(TValue)))
            {
                return rv;
            }

            if (typeof(TValue) == typeof(bool))
            {
                rv = new TsCell
                {
                    boolean_value = (bool)Convert.ChangeType(v, TypeCode.Boolean)
                };
            }
            else if (typeof(TValue) == typeof(string))
            {
                rv = new TsCell
                {
                    varchar_value = RiakString.ToBytes((string)Convert.ChangeType(v, TypeCode.String))
                };
            }

            return rv;
        }
    }
}
