// <copyright file="RiakDtMapField.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

namespace RiakClient.Models.RiakDt
{
    using Extensions;
    using Messages;

    /// <summary>
    /// Represents a Map field in a Riak Map data type.
    /// </summary>
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
