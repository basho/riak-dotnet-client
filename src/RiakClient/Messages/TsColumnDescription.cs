namespace RiakClient.Messages
{
    using System;

    public sealed partial class TsColumnDescription : IEquatable<TsColumnDescription>
    {
        public bool Equals(TsColumnDescription other)
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
            return base.Equals(obj as TsColumnDescription);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = type.GetHashCode();

                if (name != null)
                {
                    foreach (byte b in name)
                    {
                        result = (result * 397) ^ b.GetHashCode();
                    }
                }

                return result;
            }
        }
    }
}
