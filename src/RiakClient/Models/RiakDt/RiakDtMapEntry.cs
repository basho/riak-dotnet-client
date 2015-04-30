// <copyright file="RiakDtMapEntry.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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
    using System.Collections.Generic;
    using System.Linq;
    using Messages;

    /// <summary>
    /// Represents a Riak Map DataType Entry
    /// </summary>
    public class RiakDtMapEntry
    {
        internal RiakDtMapEntry(MapEntry entry)
        {
            Field = new RiakDtMapField(entry.field);

            if (Field.Type == RiakDtMapField.RiakDtMapFieldType.Counter)
            {
                Counter = new RiakDtCounter { Value = entry.counter_value };
            }

            if (Field.Type == RiakDtMapField.RiakDtMapFieldType.Flag)
            {
                FlagValue = entry.flag_value;
            }

            if (Field.Type == RiakDtMapField.RiakDtMapFieldType.Map)
            {
                MapValue = entry.map_value.Select(mv => new RiakDtMapEntry(mv)).ToList();
            }

            if (Field.Type == RiakDtMapField.RiakDtMapFieldType.Register)
            {
                RegisterValue = entry.register_value;
            }

            if (Field.Type == RiakDtMapField.RiakDtMapFieldType.Set)
            {
                SetValue = entry.set_value;
            }
        }

        /// <summary>
        /// The <see cref="RiakDtMapField"/> field summary. 
        /// Contains the field name and type.
        /// </summary>
        public RiakDtMapField Field { get; internal set; }

        /// <summary>
        /// Contains the counter value if this field is a counter type.
        /// </summary>
        public RiakDtCounter Counter { get; internal set; }

        /// <summary>
        /// Contains the set value if this field is a set type.
        /// </summary>
        public List<byte[]> SetValue { get; internal set; }

        /// <summary>
        /// Contains the register value if this field is a register type.
        /// </summary>
        public byte[] RegisterValue { get; internal set; }

        /// <summary>
        /// Contains the flag value if this field is a flag type.
        /// </summary>
        public bool? FlagValue { get; internal set; }

        /// <summary>
        /// Contains the sub-map value if this field is a map type.
        /// </summary>
        public List<RiakDtMapEntry> MapValue { get; internal set; }
    }
}
