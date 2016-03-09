namespace RiakClient.Messages
{
    using System;
    using Util;

    public sealed partial class TsCell : IEquatable<TsCell>
    {
        public bool Equals(TsCell other)
        {
            if (ReferenceEquals(other, null))
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
            return base.Equals(obj as TsCell);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 1;
                bool hashed = false;

                if (boolean_valueSpecified)
                {
                    result = (result * 397) ^ boolean_value.GetHashCode();
                    hashed = true;
                }

                if (double_valueSpecified)
                {
                    result = (result * 397) ^ double_value.GetHashCode();
                    hashed = true;
                }

                if (sint64_valueSpecified)
                {
                    result = (result * 397) ^ sint64_value.GetHashCode();
                    hashed = true;
                }

                if (timestamp_valueSpecified)
                {
                    result = (result * 397) ^ timestamp_value.GetHashCode();
                    hashed = true;
                }

                if (varchar_valueSpecified)
                {
                    if (EnumerableUtil.NotNullOrEmpty(varchar_value))
                    {
                        hashed = true;
                        foreach (byte b in varchar_value)
                        {
                            result = (result * 397) ^ b.GetHashCode();
                        }
                    }
                }

                if (!hashed)
                {
                    result = base.GetHashCode();
                }

                return result;
            }
        }

        public override string ToString()
        {
            if (boolean_valueSpecified)
            {
                return boolean_value.ToString();
            }

            if (double_valueSpecified)
            {
                return double_value.ToString();
            }

            if (sint64_valueSpecified)
            {
                return sint64_value.ToString();
            }

            if (timestamp_valueSpecified)
            {
                return timestamp_value.ToString();
            }

            if (varchar_valueSpecified)
            {
                return varchar_value.ToString();
            }

            return "TsCell: (unspecified)";
        }
    }
}
