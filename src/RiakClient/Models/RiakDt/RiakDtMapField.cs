namespace RiakClient.Models.RiakDt
{
    using Extensions;
    using Messages;

    /// <summary>
    /// Represents a Map field in a Riak Map data type.
    /// </summary>
    [System.Obsolete("RiakDt is deprecated. Please use Commands/CRDT namespace.")]
    public class RiakDtMapField
    {
        internal RiakDtMapField(MapField mapField)
        {
            Name = mapField.name.FromRiakString();
            Type = (RiakDtMapFieldType)mapField.type;
        }

        /// <summary>
        /// An enumeration of different map field types.
        /// </summary>
        public enum RiakDtMapFieldType
        {
            /// <summary>
            /// The Counter field type.
            /// </summary>
            Counter = MapField.MapFieldType.COUNTER,

            /// <summary>
            /// The Set field type.
            /// </summary>
            Set = MapField.MapFieldType.SET,

            /// <summary>
            /// The Register field type.
            /// </summary>
            Register = MapField.MapFieldType.REGISTER,

            /// <summary>
            /// The Flag field type.
            /// </summary>
            Flag = MapField.MapFieldType.FLAG,

            /// <summary>
            /// The Map field type.
            /// </summary>
            Map = MapField.MapFieldType.MAP
        }

        /// <summary>
        /// The name of the Map field.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The field type of the Map field.
        /// </summary>
        public RiakDtMapFieldType Type { get; private set; }

        /// <summary>
        /// Convert this object to a <see cref="MapField"/>.
        /// </summary>
        /// <returns>A newly instantiated and configured <see cref="MapField"/>.</returns>
        public MapField ToMapField()
        {
            return new MapField { name = Name.ToRiakString(), type = (MapField.MapFieldType)Type };
        }
    }
}
