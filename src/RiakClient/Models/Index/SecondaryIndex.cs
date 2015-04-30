// <copyright file="SecondaryIndex.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.Index
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents an abstract secondary index for a <see cref="RiakObject"/>.
    /// </summary>
    /// <typeparam name="TClass">The concrete type of the implementing class.</typeparam>
    /// <typeparam name="TIndex">The type of the index (<see cref="BigInteger"/> or <see cref="string"/>).</typeparam>
    [ComVisible(false)]
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

        /// <summary>
        /// The term values for the index.
        /// </summary>
        public ReadOnlyCollection<TIndex> Values
        {
            get { return new ReadOnlyCollection<TIndex>(values.ToList()); }
        }

        /// <summary>
        /// The name of the index.
        /// </summary>
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

        /// <summary>
        /// Clear the terms for this index instance.
        /// </summary>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public TClass Clear()
        {
            values.Clear();
            return TypedThis;
        }

        /// <summary>
        /// Add the <see cref="IEnumerable{TIndex}"/> collection of term values to the index.
        /// </summary>
        /// <param name="valuesToAdd">An <see cref="IEnumerable{TIndex}"/> of new terms to add.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public virtual TClass Add(IEnumerable<TIndex> valuesToAdd)
        {
            return Add(valuesToAdd.ToArray());
        }

        /// <summary>
        /// Adds the params array collection of term values to the index.
        /// </summary>
        /// <param name="valuesToAdd">A params array of term values to add.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public virtual TClass Add(params TIndex[] valuesToAdd)
        {
            foreach (var val in valuesToAdd)
            {
                this.values.Add(val);
            }

            return TypedThis;
        }

        /// <summary>
        /// Removes the params array <paramref name="valuesToRemove"/> of term values from the index.
        /// </summary>
        /// <param name="valuesToRemove">An <see cref="IEnumerable{TIndex}"/> of terms values to remove.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public TClass Remove(IEnumerable<TIndex> valuesToRemove)
        {
            return Remove(valuesToRemove.ToArray());
        }

        /// <summary>
        /// Removes the collection <paramref name="valuesToRemove"/> of term values from the index.
        /// </summary>
        /// <param name="valuesToRemove">A params array of term values to remove.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public TClass Remove(params TIndex[] valuesToRemove)
        {
            foreach (var val in valuesToRemove)
            {
                this.values.Remove(val);
            }

            return TypedThis;
        }
        
        /// <summary>
        /// Sets the term collection to those terms in <paramref name="values"/> collection.
        /// Deletes any existing terms in the collection.
        /// Similar to an overwriting assignment.
        /// </summary>
        /// <param name="values">An <see cref="IEnumerable{TIndex}"/> of new term values to add.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public TClass Set(IEnumerable<TIndex> values)
        {
            Clear();
            return Add(values);
        }

        /// <summary>
        /// Sets the term collection to those terms in <paramref name="values"/> params array.
        /// Deletes any existing terms in the collection.
        /// Similar to an overwriting assignment.
        /// </summary>
        /// <param name="values">A params array of new term values to add.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public TClass Set(params TIndex[] values)
        {
            Clear();
            return Add(values);
        }

        /// <summary>
        /// Determines whether an element is in the terms collection.
        /// </summary>
        /// <param name="value">The element to check membership for.</param>
        /// <returns><b>true</b> if the terms collection contains <paramref name="value"/>, <b>false</b>, otherwise.</returns>
        public bool HasValue(TIndex value)
        {
            return values.Contains(value);
        }
    }
}
