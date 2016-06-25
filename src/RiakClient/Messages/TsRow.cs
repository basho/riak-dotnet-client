namespace RiakClient.Messages
{
    using System;
    using Util;

    public sealed partial class TsRow : IEquatable<TsRow>
    {
        public bool Equals(TsRow other)
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
            return base.Equals(obj as TsRow);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 1;
                bool hashed = false;

                if (EnumerableUtil.NotNullOrEmpty(cells))
                {
                    hashed = true;
                    foreach (TsCell c in cells)
                    {
                        result = (result * 397) ^ c.GetHashCode();
                    }
                }

                if (!hashed)
                {
                    result = base.GetHashCode();
                }

                return result;
            }
        }
    }
}
