// <copyright file="JsonExtensions.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Extensions
{
    using System.Collections.Generic;
    using Models.MapReduce.KeyFilters;
    using Newtonsoft.Json;

    /// <summary>
    /// Helper extension methods to use common Newtonsoft.Json methods with less code. 
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <typeparam name="T">A value or reference type.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The object serialized as a JSON string.</returns>
        public static string ToJson<T>(this T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <typeparam name="T">A reference type.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The object serialized as a JSON string.</returns>
        public static string Serialize<T>(this T obj) where T : class
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Writes the property name and the value as a name/value pair.
        /// An error will be raised if the value cannot be written as a single JSON token.
        /// </summary>
        /// <typeparam name="T">A value or reference type</typeparam>
        /// <param name="writer">The JsonWriter to write the property to.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property, writes a System.Object value.</param>
        /// <returns>The original JsonWriter, not modified but useful for call chaining.</returns>
        public static JsonWriter WriteProperty<T>(this JsonWriter writer, string name, T value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
            return writer;
        }

        /// <summary>
        /// If the value is non null, this method will write the property name and the value as a name/value pair.
        /// An error will be raised if the value cannot be written as a single JSON token.
        /// </summary>
        /// <typeparam name="T">A reference type</typeparam>
        /// <param name="writer">The JsonWriter to write the property to.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property, writes a System.Object value.</param>
        /// <returns>The original JsonWriter, not modified but useful for call chaining.</returns>
        public static JsonWriter WriteNonNullProperty<T>(this JsonWriter writer, string name, T value)
            where T : class
        {
            if (value != null)
            {
                writer.WriteProperty(name, value);
            }

            return writer;
        }

        /// <summary>
        /// If the nullable value type is non null, this method will write the property name and the value as a name/value pair.
        /// An error will be raised if the value cannot be written as a single JSON token.
        /// </summary>
        /// <typeparam name="T">A nullable value type</typeparam>
        /// <param name="writer">The JsonWriter to write the property to.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property, writes a System.Object value.</param>
        /// <returns>The original JsonWriter, not modified but useful for call chaining.</returns>
        public static JsonWriter WriteNullableProperty<T>(this JsonWriter writer, string name, T? value)
            where T : struct
        {
            if (value.HasValue)
            {
                writer.WriteProperty(name, value.Value);
            }

            return writer;
        }

        /// <summary>
        /// If the string value is non-empty, this method will write the property name and the value as a name/value pair.
        /// An error will be raised if the value cannot be written as a single JSON token.
        /// </summary>
        /// <param name="writer">The JsonWriter to write the property to.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property, writes a System.Object value.</param>
        /// <returns>The original JsonWriter, not modified but useful for call chaining.</returns>
        public static JsonWriter WriteSpecifiedProperty(this JsonWriter writer, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                writer.WriteProperty(name, value);
            }

            return writer;
        }

        internal static void WriteRawFilterTokenArray(this JsonWriter writer, List<IRiakKeyFilterToken> tokens)
        {
            writer.WriteStartArray();

            tokens.ForEach(t => writer.WriteRawValue(t.ToString()));

            writer.WriteEndArray();
        }
    }
}
