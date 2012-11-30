// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorrugatedIron.Tests.Extensions
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class UnitTestExtensions
    {
        public static void ShouldEqual<T>(this T actual, T expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldNotEqual<T>(this T actual, T expected)
        {
            Assert.AreNotEqual(expected, actual);
        }

        public static void ShouldBe<T>(this object actual)
        {
            Assert.IsInstanceOf<T>(actual);
        }
        
        public static void ShouldBeGreaterThan(this int actual, int expected)
        {
            Assert.GreaterOrEqual(actual, expected);
        }
        
        public static void ShouldBeLessThan(this int actual, int expected)
        {
            Assert.LessOrEqual(actual, expected);
        }

        public static void ShouldBeFalse(this bool value, string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                Assert.IsFalse(value);
            }
            else
            {
                Assert.IsFalse(value, message);
            }
        }

        public static void ShouldBeTrue(this bool value, string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                Assert.IsTrue(value);
            }
            else
            {
                Assert.IsTrue(value, message);
            }
        }

        public static void ShouldBeNullOrEmpty(this string value)
        {
            Assert.IsNullOrEmpty(value);
        }

        public static void ShouldNotBeNullOrEmpty(this string value)
        {
            Assert.IsNotNullOrEmpty(value);
        }

        public static void ShouldNotBeNull<T>(this T obj) where T : class
        {
            Assert.IsNotNull(obj);
        }

        public static void ShouldBeNull<T>(this T obj) where T : class
        {
            Assert.IsNull(obj);
        }

        public static void IsAtLeast(this int val, int min)
        {
            Assert.Less(min - 1, val);
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

        public static void ShouldContain<T>(this IEnumerable<T> items, T value)
        {
            items.Contains(value).ShouldBeTrue();
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> items, T value)
        {
            items.Contains(value).ShouldBeFalse();
        }

        public static string DisplayString<T>(this T items) where T : IEnumerable
        {
            var sb = new StringBuilder();
            var comma = "";

            foreach(var item in items)
            {
                sb.Append(comma + item.ToString());
                comma = ", ";
            }

            return sb.ToString();
        }
    }
}
