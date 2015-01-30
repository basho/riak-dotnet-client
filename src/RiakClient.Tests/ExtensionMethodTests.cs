// <copyright file="ExtensionMethodTests.cs" company="Basho Technologies, Inc.">
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RiakClient.Extensions;
using NUnit.Framework;

namespace RiakClient.Tests
{
    [TestFixture]
    public class ExtensionMethodTests
    {
        private static readonly string[] data = new[] {
            "one", "two", "three", "four", "five"
        };

        private static readonly ICollection<string> collection = new TestCollection<string> {
            "one", "two", "three", "four", "five"
        };

        [Test]
        public void WhenCalledOnEnumerable_IsNullOrEmptyDoesNotLoseData()
        {
            var containsLetterE = data.Where(d => d.Contains("e"));
            Assert.False(containsLetterE.IsNullOrEmpty());

            int i = 0;
            foreach (string s in containsLetterE)
            {
                if (s.Contains("e"))
                {
                    ++i;
                }
            }
            Assert.AreEqual(3, i);
        }

        [Test]
        public void WhenCalledOnCollection_CountIsUsed()
        {
            Assert.False(collection.IsNullOrEmpty());
            Assert.False(collection.IsNullOrEmpty());

            var tc = (TestCollection<string>)collection;
            Assert.AreEqual(2, tc.CountCalled);
        }

        private class TestCollection<T> : ICollection<T>
        {
            private int countCalled = 0;
            private IList<T> data = new List<T>();

            public void Add(T item)
            {
                data.Add(item);
            }

            public void Clear()
            {
                data.Clear();
            }

            public bool Contains(T item)
            {
                return data.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                data.CopyTo(array, arrayIndex);
            }

            public int CountCalled
            {
                get { return countCalled; }
            }

            public int Count
            {
                get
                {
                    ++countCalled;
                    return data.Count;
                }
            }

            public bool IsReadOnly
            {
                get { return data.IsReadOnly; }
            }

            public bool Remove(T item)
            {
                return data.Remove(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return data.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return data.GetEnumerator();
            }
        }

    }
}
