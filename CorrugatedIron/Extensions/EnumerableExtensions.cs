// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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
using System.Collections.Generic;

namespace CorrugatedIron.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
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

        public static IEnumerable<T> Cycle<T>(this Func<IEnumerable<T>> generator)
        {
            while (true)
            {
                foreach (var item in generator())
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> Replicate<T>(this T obj, int times)
        {
            while (times-- > 0)
            {
                yield return obj;
            }
        }
    }
}
