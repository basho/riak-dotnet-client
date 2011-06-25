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

using CorrugatedIron.Extensions;
using CorrugatedIron.KeyFilters;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Models.MapReduce
{
    [TestFixture]
    public class RiakMapReduceTests
    {
        private const string MrJobText =
            @"{""inputs"":""animals"",""query"":[{""map"":{""language"":""javascript"",""source"":""function(v) { return [v]; }"",""keep"":true}}]}";

        private const string ComplexMrJobText =
            @"{""inputs"":""animals"",""query"":[{""map"":{""language"":""javascript"",""source"":""function(o) { if (o.key.indexOf('spider') != -1) return [1]; return []; }"",""keep"":false}},{""reduce"":{""language"":""javascript"",""name"":""Riak.reduceSum"",""keep"":true}}]}";

        private const string ComplexMrJobWithFilterText =
            @"{""inputs"":""animals"",""key_filters"":[[""matches"",""spider""]],""query"":[{""map"":{""language"":""javascript"",""source"":""function(o) { return [1]; }"",""keep"":false}},{""reduce"":{""language"":""javascript"",""name"":""Riak.reduceSum"",""keep"":true}}]}";

        private const string MrContentType = Constants.ContentTypes.ApplicationJson;

        [Test]
        public void BuildingSimpleMapReduceJobsWithTheApiProducesByteArrays()
        {
            var query = new RiakMapReduceQuery
                {
                    ContentType = MrContentType
                }
                .Inputs(new RiakBucketInput("animals"))
                .MapJs(m => m.Source("function(v) { return [v]; }").Keep(true));

            var request = query.ToMessage();
            request.ContentType.ShouldEqual(MrContentType.ToRiakString());
            request.Request.ShouldEqual(MrJobText.ToRiakString());
        }

        [Test]
        public void BuildingComplexMapReduceJobsWithTheApiProducesTheCorrectCommand()
        {
            var query = new RiakMapReduceQuery
                {
                    ContentType = MrContentType
                }
                .Inputs(new RiakBucketInput("animals"))
                .MapJs(m => m.Source("function(o) { if (o.key.indexOf('spider') != -1) return [1]; return []; }"))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));

            var request = query.ToMessage();
            request.Request.ShouldEqual(ComplexMrJobText.ToRiakString());
        }

        [Test]
        public void BuildingComplexMapReduceJobsWithFiltersProducesTheCorrectCommand()
        {
            var query = new RiakMapReduceQuery
                {
                    ContentType = MrContentType
                }
                .Inputs(new RiakBucketInput("animals"))
                .Filter(new Matches<string>("spider"))
                .MapJs(m => m.Source("function(o) { return [1]; }"))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));

            var request = query.ToMessage();
            request.Request.ShouldEqual(ComplexMrJobWithFilterText.ToRiakString());
        }
    }
}
