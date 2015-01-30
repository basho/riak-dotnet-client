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

using Newtonsoft.Json;
using RiakClient.Containers;

namespace RiakClient.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson<T>(this T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static string Serialize<T>(this T obj) where T : class
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static JsonWriter WriteProperty<T>(this JsonWriter writer, string name, T value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
            return writer;
        }

        public static JsonWriter WriteNonNullProperty<T>(this JsonWriter writer, string name, T value)
            where T : class
        {
            if (value != null)
            {
                writer.WriteProperty(name, value);
            }
            return writer;
        }

        public static JsonWriter WriteEither<TLeft, TRight>(this JsonWriter writer, string name, Either<TLeft, TRight> either)
        {
            if (either != null)
            {
                if (either.IsLeft)
                {
                    writer.WriteProperty(name, either.Left);
                }
                else
                {
                    writer.WriteProperty(name, either.Right);
                }
            }
            return writer;
        }

        public static JsonWriter WriteNullableProperty<T>(this JsonWriter writer, string name, T? value)
            where T : struct
        {
            if (value.HasValue)
            {
                writer.WriteProperty(name, value.Value);
            }
            return writer;
        }

        public static JsonWriter WriteSpecifiedProperty(this JsonWriter writer, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                writer.WriteProperty(name, value);
            }
            return writer;
        }
    }
}
