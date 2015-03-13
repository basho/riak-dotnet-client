// <copyright file="IntIndex.cs" company="Basho Technologies, Inc.">
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
    using System.Linq;
    using System.Numerics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// An integer secondary index for a <see cref="RiakObject"/>.
    /// </summary>
    [ComVisible(false)]
    public class IntIndex : SecondaryIndex<IntIndex, BigInteger>
    {
        internal IntIndex(RiakObject container, string name)
            : base(container, name)
        {
        }

        protected override IntIndex TypedThis
        {
            get { return this; }
        }

        protected override string IndexSuffix
        {
            get { return RiakConstants.IndexSuffix.Integer; }
        }

        /// <summary>
        /// Sets the term collection to those term values in param string[] <paramref name="values"/>.
        /// Deletes any existing terms in the collection.
        /// Similar to an overwriting assignment.
        /// </summary>
        /// <param name="values">The new terms to add.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public IntIndex Set(params string[] values)
        {
            return Set(values.Select(BigInteger.Parse));
        }

        /// <summary>
        /// Sets the term collection to those term values in the <see cref="IEnumerable{String}"/> collection <paramref name="values"/>.
        /// Deletes any existing terms in the collection.
        /// Similar to an overwriting assignment.
        /// </summary>
        /// <param name="values">The new terms to add.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public IntIndex Set(IEnumerable<string> values)
        {
            return Set(values.Select(BigInteger.Parse));
        }

        /// <summary>
        /// Adds the term values collection in param string[] <paramref name="values"/> to the index.
        /// </summary>
        /// <param name="values">The term values to add.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public IntIndex Add(params string[] values)
        {
            return Add(values.Select(BigInteger.Parse));
        }

        /// <summary>
        /// Adds the term values in the <see cref="IEnumerable{String}"/> collection <paramref name="values"/> to the index.
        /// </summary>
        /// <param name="values">The term values to add.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public IntIndex Add(IEnumerable<string> values)
        {
            return Add(values.Select(BigInteger.Parse));
        }

        /// <summary>
        /// Removes the term values in param string[] <paramref name="values"/> from the index.
        /// </summary>
        /// <param name="values">The term values to remove.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public IntIndex Remove(params string[] values)
        {
            return Remove(values.Select(BigInteger.Parse));
        }

        /// <summary>
        /// Removes the term values in the <see cref="IEnumerable{String}"/> collection <paramref name="values"/> from the index.
        /// </summary>
        /// <param name="values">The term values to remove.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public IntIndex Remove(IEnumerable<string> values)
        {
            return Remove(values.Select(BigInteger.Parse));
        }

        /// <summary>
        /// Delete this index from it's parent <see cref="RiakObject"/>.
        /// </summary>
        /// <returns>
        /// A reference to the updated parent <see cref="RiakObject"/>.
        /// </returns>
        public RiakObject Delete()
        {
            Container.IntIndexes.Remove(Name);
            return Container;
        }
    }
}
