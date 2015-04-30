// <copyright file="IRiakDtType.cs" company="Basho Technologies, Inc.">
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
    using System.Collections.ObjectModel;
    using Messages;

    // TODO: Deprecate this?

    /// <summary>
    /// Generic interface for a Riak data type.
    /// </summary>
    /// <typeparam name="T">The type of the data type.</typeparam>
    /// <remarks>Not used.</remarks>
    public interface IRiakDtType<T>
    {
        /// <summary>
        /// The bucket name of the data type object.
        /// </summary>
        string Bucket { get; }

        /// <summary>
        /// The bucket type of the data type object.
        /// </summary>
        string BucketType { get; }

        /// <summary>
        /// The key of the data type object.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// A collection of operations for the data type object.
        /// </summary>
        ReadOnlyCollection<T> Operations { get; }

        /// <summary>
        /// Convert this object to a new <see cref="MapEntry"/>.
        /// </summary>
        /// <param name="fieldName">The field name for the map entry.</param>
        /// <returns>A newly initialized and configured <see cref="MapEntry"/>.</returns>
        MapEntry ToMapEntry(string fieldName);
    }
}
