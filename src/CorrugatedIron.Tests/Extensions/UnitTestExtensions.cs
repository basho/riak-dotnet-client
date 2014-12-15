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

using System;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CorrugatedIron.Tests.Extensions
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class UnitTestExtensions
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

        public static void ShouldBeNullOrEmpty(this string value, string message = null)
        {
            Assert.IsNullOrEmpty(value, message);
        }

        public static void ShouldNotBeNullOrEmpty(this string value, string message = null)
        {
            Assert.IsNotNullOrEmpty(value, message);
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

            foreach(var item in items)
            {
                sb.Append(comma + item.ToString());
                comma = ", ";
            }

            return sb.ToString();
        }

        private static readonly Func<RiakResult, bool> DefaultSuccessFunc = result => result.IsSuccess;

        public static T WaitUntil<T>(this Func<T> action, int attempts = 10) where T : RiakResult
        {
            return WaitUntil<T>(action, DefaultSuccessFunc, attempts); 
        }

        public static T WaitUntil<T>(this Func<T> action, Func<T, bool> successCriteriaFunc, int attempts = 10) where T : RiakResult
        {
            var invalidResults = new List<T>();
            var exceptions = new List<Exception>();

            T result = null;
            for (var i = 0; i < attempts; i++)
            {
                result = null;
                try
                {
                    result = action.Invoke();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    // Do nothing, try again
                }

                if (result != null && successCriteriaFunc.Invoke(result))
                    return result;

                invalidResults.Add(result);

                Thread.Sleep(i * 1000);
            }
            // print retry "trace" and
            // return last result if all failed the success check

            PrintFailedRetries(invalidResults, exceptions);

            return result;
        }

        private static void PrintFailedRetries<T>(IList<T> invalidResults, IList<Exception> exceptions) where T : RiakResult
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var testMethod = stackTrace.GetFrame(2).GetMethod();
            var testClass = testMethod.ReflectedType;


            Console.WriteLine("Could not reach success criteria while running {0}.{1}\n", testClass.FullName, testMethod.Name);

            for (var i = 0; i < 10; i++)
            {
                var result = invalidResults[i];
                var exception = exceptions[i];

                Console.WriteLine("Iteration {0}:", i);
                Console.WriteLine("----------------------------------------\n");

                if (result != null)
                {
                    Console.WriteLine(
                        "RiakResult:\nSuccess: {0}\nNodeOffline: {1}\nResultCode: {2}\nError Message: {3}\n",
                        result.IsSuccess,
                        result.NodeOffline,
                        result.ResultCode,
                        result.ErrorMessage);
                }
                else
                {
                    Console.WriteLine("RiakResult: No RiakResult Recorded\n");
                }

                if (exception != null)
                {
                    Console.WriteLine("Exception: {0}\n", exception);
                }
                else
                {
                    Console.WriteLine("Exception: No Exception Recorded\n");
                }
            }
            Console.WriteLine("----------------------------------------\n");
        }
    }
}
