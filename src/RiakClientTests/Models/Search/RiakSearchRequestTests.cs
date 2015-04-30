// <copyright file="RiakSearchRequestTests.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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

    [TestFixture]
    public class RiakSearchRequestTests
    {
        [Test]
        public void UsingFluentQueryProducesSameQueryAsString()
        {
            string index = "index";
            string field = "data_s";
            string search = "frazzle";
            string solrQuery = string.Format("{0}:{1}", field, search);

            var fluentSearch = new RiakFluentSearch(index, field).Search(search).Build();
            var s1 = new RiakSearchRequest { Query = fluentSearch };
            var s2 = new RiakSearchRequest(index, solrQuery);

            Assert.AreEqual(s1, s2);
        }

        [Test]
        public void UsingFluentQueryWithFilterProducesSameQueryAsString()
        {
            string index = "index";
            string field = "data_s";
            string search = "frazzle";
            string solrQuery = string.Format("{0}:{1}", field, search);
            string solrFilter = string.Format("{0}:[10 TO 20]", field);

            var fluentSearch = new RiakFluentSearch(index, field).Search(search).Build();
            var fluentFilter = new RiakFluentSearch(index, field).Between("10", "20", true).Build();
            var s1 = new RiakSearchRequest { Query = fluentSearch, Filter = fluentFilter };
            var s2 = new RiakSearchRequest(index, solrQuery, solrFilter);

            Assert.AreEqual(s1, s2);
        }
    }
}