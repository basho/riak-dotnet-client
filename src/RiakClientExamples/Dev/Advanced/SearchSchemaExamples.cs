// <copyright file="SearchSchemaExamples.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientExamples.Dev.Using.Advanced
{
    using System.IO;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models.Search;

    /*
     * http://docs.basho.com/riak/latest/dev/advanced/search-schema/
     */
    public sealed class SearchSchemaExamples : ExampleBase
    {
        [Test, Ignore("Requires cartoons.xml to be present")]
        public void CustomSchema()
        {
            var xml = File.ReadAllText("cartoons.xml");
            var schema = new SearchSchema("cartoons", xml);
            var rslt = client.PutSearchSchema(schema);
            CheckResult(rslt);
        }
    }
}
