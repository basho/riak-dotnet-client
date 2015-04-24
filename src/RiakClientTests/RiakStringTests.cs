// <copyright file="RiakStringTests.cs" company="Basho Technologies, Inc.">
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
// </copyright>

namespace RiakClientTests
{
    using System.Text;
    using NUnit.Framework;
    using RiakClient;

    [TestFixture]
    public class RiakStringTests
    {
        const string testString = "test1234";
        private static readonly byte[] testBytes = Encoding.UTF8.GetBytes(testString);

        [Test]
        public void Can_Convert_To_Byte_Array()
        {
            var rs = new RiakString(testString);
            Assert.AreEqual(testBytes, (byte[])rs);
        }

        [Test]
        public void Can_Convert_To_Boolean_To_Indicate_Non_Null_Value()
        {
            var rs = new RiakString(testString);
            Assert.True(rs);
            Assert.True(rs.HasValue);

            rs = new RiakString(null);
            Assert.False(rs);
            Assert.False(rs.HasValue);
        }
    }
}