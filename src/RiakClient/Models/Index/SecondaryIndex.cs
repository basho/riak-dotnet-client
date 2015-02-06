// <copyright file="SecondaryIndex.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.Index
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public abstract class SecondaryIndex<TClass, TIndex>
    {
        protected readonly RiakObject Container;

        private readonly HashSet<TIndex> values = new HashSet<TIndex>();
        private readonly string name;

        protected SecondaryIndex(RiakObject container, string name)
        {
            this.Container = container;
            this.name = name.ToLower();
        }

        public ReadOnlyCollection<TIndex> Values
        {
            get { return new ReadOnlyCollection<TIndex>(values.ToList()); }
        }

        public string Name
        {
            get { return name; }
        }

        internal string RiakIndexName
        {
            get { return name + IndexSuffix; }
        }

        protected abstract TClass TypedThis { get; }

        protected abstract string IndexSuffix { get; }

        public TClass Clear()
        {
            values.Clear();
            return TypedThis;
        }

        public virtual TClass Add(IEnumerable<TIndex> valuesToAdd)
        {
            return Add(valuesToAdd.ToArray());
        }

        public virtual TClass Add(params TIndex[] valuesToAdd)
        {
            foreach (var val in valuesToAdd)
            {
                this.values.Add(val);
            }

            return TypedThis;
        }

        public TClass Remove(IEnumerable<TIndex> valuesToRemove)
        {
            return Remove(valuesToRemove.ToArray());
        }

        public TClass Remove(params TIndex[] valuesToRemove)
        {
            foreach (var val in valuesToRemove)
            {
                this.values.Remove(val);
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
            return values.Contains(value);
        }
    }
}
