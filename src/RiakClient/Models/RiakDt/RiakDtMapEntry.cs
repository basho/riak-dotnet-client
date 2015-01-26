// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using System.Collections.Generic;
using System.Linq;
using CorrugatedIron.Messages;

namespace CorrugatedIron.Models.RiakDt
{
    public class RiakDtMapEntry
    {
        public RiakDtMapField Field { get; internal set; }
        public RiakDtCounter Counter { get; internal set; }
        public List<byte[]> SetValue { get; internal set; }
        public byte[] RegisterValue { get; internal set; }
        public bool? FlagValue { get; internal set; }
        public List<RiakDtMapEntry> MapValue { get; internal set; }

        internal RiakDtMapEntry(MapEntry entry)
        {
            Field = new RiakDtMapField(entry.field);

            if (Field.Type == RiakDtMapField.RiakDtMapFieldType.Counter)
                Counter = new RiakDtCounter {Value = entry.counter_value};

            if (Field.Type == RiakDtMapField.RiakDtMapFieldType.Flag)
                FlagValue = entry.flag_value;

            if (Field.Type == RiakDtMapField.RiakDtMapFieldType.Map)
                MapValue = entry.map_value.Select(mv => new RiakDtMapEntry(mv)).ToList();

            if (Field.Type == RiakDtMapField.RiakDtMapFieldType.Register)
                RegisterValue = entry.register_value;

            if (Field.Type == RiakDtMapField.RiakDtMapFieldType.Set)
                SetValue = entry.set_value;
        }
    }
}
