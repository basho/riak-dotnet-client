// <copyright file="EnumerableUtil.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Util
{
    using System.Collections;
    using System.Collections.Generic;

    internal static class EnumerableUtil
    {
        public static bool NotNullOrEmpty(IEnumerable items)
        {
            return !EnumerableUtil.IsNullOrEmpty(items);
        }

        public static bool NotNullOrEmpty<T>(IEnumerable<T> items)
        {
            return !EnumerableUtil.IsNullOrEmpty(items);
        }

        public static bool IsNullOrEmpty(IEnumerable items)
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

        public static bool IsNullOrEmpty<T>(IEnumerable<T> items)
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

            return IsNullOrEmpty((IEnumerable)items);
        }
    }
}
