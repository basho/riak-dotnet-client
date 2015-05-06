// <copyright file="RiakSearchResultTests.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.Models.Search
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RiakClient.Models.Search;

    [TestFixture, UnitTest]
    public class RiakSearchResultTests
    {

        [Test]
        public void TestPublicConstructor()
        {
            var result = new RiakSearchResult(100.0f, 42, new List<RiakSearchResultDocument>());
            result.MaxScore.ShouldEqual(100.0f);
            result.NumFound.ShouldEqual(42);
            result.Documents.ShouldNotBeNull();
            result.Documents.Count.ShouldEqual(0);
        }

        [Test]
        public void TestPublicConstructorThrowsExceptionWhenDocumentsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RiakSearchResult(100.0f, 42, null));
        }
    }
}