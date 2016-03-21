namespace RiakClient.Commands.TS
{
    using System;
    using Messages;

    public class Column : IEquatable<Column>
    {
        private readonly string name;
        private readonly ColumnType type;

        public Column(string name, ColumnType type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            this.name = name;
            this.type = type;
        }
        
        public string Name
        {
            get { return name; }
        }

        public ColumnType Type
        {
            get { return type; }
        }

        public bool Equals(Column other)
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
            return base.Equals(obj as Column);
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses a combination of the public properties to generate a unique hash code.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = name.GetHashCode();
                result = (result * 397) ^ type.GetHashCode();
                return result;
            }
        }

        internal TsColumnDescription ToTsColumn()
        {
            return new TsColumnDescription
            {
                name = RiakString.ToBytes(name),
                type = (TsColumnType)type
            };
        }
    }
}
