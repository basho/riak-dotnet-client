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
using System.Collections.ObjectModel;
using System.Linq;

namespace CorrugatedIron.Models.Index
{
    public abstract class SecondaryIndex<TClass, TIndex>
    {
        private readonly HashSet<TIndex> _values;
        private readonly string _name;

        protected readonly RiakObject Container;

        protected abstract TClass TypedThis { get; }
        protected abstract string IndexSuffix { get; }

        public ReadOnlyCollection<TIndex> Values
        {
            get { return new ReadOnlyCollection<TIndex>(_values.ToList()); }
        }

        internal string RiakIndexName
        {
            get { return _name + IndexSuffix; }
        }

        public string Name
        {
            get { return _name; }
        }

        protected SecondaryIndex(RiakObject container, string name)
        {
            Container = container;

            _values = new HashSet<TIndex>();
            _name = name;
        }

        public TClass Clear()
        {
            _values.Clear();
            return TypedThis;
        }

        public TClass Add(IEnumerable<TIndex> values)
        {
            return Add(values.ToArray());
        }

        public TClass Add(params TIndex[] values)
        {
            foreach (var val in values)
            {
                _values.Add(val);
            }

            return TypedThis;
        }

        public TClass Remove(IEnumerable<TIndex> values)
        {
            return Remove(values.ToArray());
        }

        public TClass Remove(params TIndex[] values)
        {
            foreach (var val in values)
            {
                _values.Remove(val);
            }
            return TypedThis;
        }

        public TClass Set(params TIndex[] values)
        {
            Clear();
            return Add(values);
        }

        public TClass Set(IEnumerable<TIndex> values)
        {
            Clear();
            return Add(values);
        }

        public bool HasValue(TIndex value)
        {
            return _values.Contains(value);
        }
    }
}