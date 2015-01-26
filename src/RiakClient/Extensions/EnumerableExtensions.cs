// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2015 - Basho Technologies, Inc.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CorrugatedIron.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty(this IEnumerable items)
        {
            if (items == null)
            {
                return true;
            }

            var collection = items as ICollection;
            if (collection != null)
            {
                return collection.Count == 0;
            }

            return !items.GetEnumerator().MoveNext();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            if (items == null)
            {
                return true;
            }

            var collection = items as ICollection<T>;
            if (collection != null)
            {
                return collection.Count == 0;
            }

            return ((IEnumerable)items).IsNullOrEmpty();
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static IEnumerable<T> Replicate<T>(this T obj, int count)
        {
            while (count-- > 0)
            {
                yield return obj;
            }
        }

        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> items)
        {
            while (true)
            {
                foreach (var item in items)
                {
                    yield return item;
                }
            }
        }

        public static void Times(this int count, Action action)
        {
            while (count-- > 0)
            {
                action();
            }
        }

        public static IEnumerable<T> Times<T>(this int count, Func<T> generator)
        {
            while (count-- > 0)
            {
                yield return generator();
            }
        }

        public static bool In<T>(this T val, IEnumerable<T> items)
        {
            return items.Contains(val);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }
}
