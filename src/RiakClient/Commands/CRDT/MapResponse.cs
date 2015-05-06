// <copyright file="MapResponse.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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

namespace RiakClient.Commands.CRDT
{
    using System;
    using System.Collections.Generic;
    using Messages;

    /// <summary>
    /// Response to a <see cref="FetchMap"/> command.
    /// </summary>
    public class MapResponse : Response<Map>
    {
        /// <inheritdoc />
        public MapResponse() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapResponse"/> class.
        /// </summary>
        /// <param name="key">A <see cref="RiakString"/> representing the key.</param>
        /// <param name="context">The data type context. Necessary to use this if updating a data type with removals.</param>
        /// <param name="value">A map that has been parsed from the Riak CRDT data.</param>
        public MapResponse(RiakString key, byte[] context, Map value)
            : base(key, context, value)
        {
        }

        public MapResponse(RiakString key, byte[] context, IEnumerable<MapEntry> mapEntries)
            : base(key, context, ParseMapEntries(mapEntries))
        {
        }

        private static Map ParseMapEntries(IEnumerable<MapEntry> mapEntries)
        {
            var map = new Map();
            ParseMapEntries(map, mapEntries);
            return map;
        }

        private static void ParseMapEntries(Map map, IEnumerable<MapEntry> mapEntries)
        {
            foreach (MapEntry mapEntry in mapEntries)
            {
                MapField field = mapEntry.field;
                RiakString fieldName = new RiakString(field.name);
                switch (field.type)
                {
                    case MapField.MapFieldType.COUNTER:
                        map.Counters.Add(fieldName, mapEntry.counter_value);
                        break;
                    case MapField.MapFieldType.FLAG:
                        map.Flags.Add(fieldName, mapEntry.flag_value);
                        break;
                    case MapField.MapFieldType.REGISTER:
                        map.Registers.Add(fieldName, mapEntry.register_value);
                        break;
                    case MapField.MapFieldType.SET:
                        map.Sets.Add(fieldName, mapEntry.set_value);
                        break;
                    case MapField.MapFieldType.MAP:
                        var innerMap = new Map();
                        ParseMapEntries(innerMap, mapEntry.map_value);
                        map.Maps.Add(fieldName, innerMap);
                        break;
                    default:
                        throw new InvalidOperationException(
                            string.Format("Unknown map entry type: {0}", field.type));
                }
            }
        }
    }
}