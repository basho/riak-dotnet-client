// <copyright file="Extensions.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using RiakClient.Util;

    [System.Diagnostics.DebuggerNonUserCode]
    public static class Extensions
    {
        public static void ShouldEqual<T>(this T actual, T expected, string message = null)
        {
            Assert.AreEqual(expected, actual, message);
        }

        public static void ShouldNotEqual<T>(this T actual, T expected, string message = null)
        {
            Assert.AreNotEqual(expected, actual, message);
        }

        public static void ShouldBe<T>(this object actual, string message = null)
        {
            Assert.IsInstanceOf<T>(actual, message);
        }

        public static void ShouldBeGreaterThan(this long? actual, long? expected, string message = null)
        {
            Assert.GreaterOrEqual(actual, expected, message);
        }

        public static void ShouldBeLessThan(this long? actual, long? expected, string message = null)
        {
            Assert.LessOrEqual(actual, expected, message);
        }

        public static void ShouldBeGreaterThan(this long actual, long expected, string message = null)
        {
            Assert.GreaterOrEqual(actual, expected, message);
        }

        public static void ShouldBeLessThan(this long actual, long expected, string message = null)
        {
            Assert.LessOrEqual(actual, expected, message);
        }

        public static void ShouldBeGreaterThan(this int actual, int expected, string message = null)
        {
            Assert.GreaterOrEqual(actual, expected, message);
        }

        public static void ShouldBeLessThan(this int actual, int expected, string message = null)
        {
            Assert.LessOrEqual(actual, expected, message);
        }

        public static void ShouldBeFalse(this bool value, string message = null)
        {
            Assert.IsFalse(value, message);
        }

        public static void ShouldBeTrue(this bool value, string message = null)
        {
            Assert.IsTrue(value, message);
        }

        public static void ShouldNotBeNullOrEmpty(this string value, string message = null)
        {
            Assert.True(EnumerableUtil.NotNullOrEmpty(value), message);
        }

        public static void ShouldNotBeNull<T>(this T obj, string message = null) where T : class
        {
            Assert.IsNotNull(obj, message);
        }

        public static void ShouldBeNull<T>(this T obj, string message = null) where T : class
        {
            Assert.IsNull(obj, message);
        }

        public static void IsAtLeast(this int val, int min, string message = null)
        {
            Assert.Less(min - 1, val, message);
        }

        public static void ContentsShouldEqual<T>(this T actual, T expected) where T : IEnumerable
        {
            var actualEnumerator = actual.GetEnumerator();
            var expectedEnumerator = expected.GetEnumerator();

            while (actualEnumerator.MoveNext())
            {
                if (!expectedEnumerator.MoveNext() || !actualEnumerator.Current.Equals(expectedEnumerator.Current))
                {
                    Assert.Fail("Contents are not the same:\n{0}\n{1}\n", actual.DisplayString(), expected.DisplayString());
                }
            }

            if (expectedEnumerator.MoveNext())
            {
                Assert.Fail("Contents are not the same:\n{0}\n{1}\n", actual.DisplayString(), expected.DisplayString());
            }
        }

        public static void ShouldContain<T>(this IEnumerable<T> items, T value, string message = null)
        {
            items.Contains(value).ShouldBeTrue(message);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> items, T value, string message = null)
        {
            items.Contains(value).ShouldBeFalse(message);
        }

        public static string DisplayString<T>(this T items) where T : IEnumerable
        {
            var sb = new StringBuilder();
            var comma = "";

            foreach (var item in items)
            {
                sb.Append(comma + item.ToString());
                comma = ", ";
            }

            return sb.ToString();
        }
    }
}
