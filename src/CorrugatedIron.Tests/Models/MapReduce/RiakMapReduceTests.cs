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

using CorrugatedIron.Extensions;
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

        private const string MrJobWithTimeoutText =
            @"{""inputs"":""animals"",""query"":[{""map"":{""language"":""javascript"",""source"":""function(v) { return [v]; }"",""keep"":true}}],""timeout"":100200}";

        private const string ComplexMrJobText =
            @"{""inputs"":""animals"",""query"":[{""map"":{""language"":""javascript"",""source"":""function(o) { if (o.key.indexOf('spider') != -1) return [1]; return []; }"",""keep"":false}},{""reduce"":{""language"":""javascript"",""name"":""Riak.reduceSum"",""keep"":true}}]}";

        private const string ComplexMrJobWithFilterText =
            @"{""inputs"":{""bucket"":""animals"",""key_filters"":[[""matches"",""spider""]]},""query"":[{""map"":{""language"":""javascript"",""source"":""function(o) { return [1]; }"",""keep"":false}},{""reduce"":{""language"":""javascript"",""name"":""Riak.reduceSum"",""keep"":true}}]}";

        private const string MrJobWithArgumentsArray =
            @"{""inputs"":""animals"",""query"":[{""reduce"":{""language"":""javascript"",""name"":""Riak.reduceSlice"",""arg"":[1,10],""keep"":true}}]}";

        private const string MrJobWithObjectArgument =
            @"{""inputs"":""animals"",""query"":[{""reduce"":{""language"":""javascript"",""name"":""Riak.reduceSlice"",""arg"":{""reduce_phase_only_1"":true},""keep"":true}}]}";

        private const string MrJobWithValueTypeArgument =
            @"{""inputs"":""animals"",""query"":[{""reduce"":{""language"":""javascript"",""name"":""Riak.reduceSlice"",""arg"":""slartibartfast"",""keep"":true}}]}";

        private const string MrContentType = RiakConstants.ContentTypes.ApplicationJson;

        [Test]
        public void BuildingSimpleMapReduceJobsWithTheApiProducesByteArrays()
        {
            var query = new RiakMapReduceQuery
                {
                    ContentType = MrContentType
                }
                .Inputs("animals")
                .MapJs(m => m.Source("function(v) { return [v]; }").Keep(true));

            var request = query.ToMessage();
            request.content_type.ShouldEqual(MrContentType.ToRiakString());
            request.request.ShouldEqual(MrJobText.ToRiakString());
        }

        [Test]
        public void BuildingSimpleMapReduceJobsWithTimeoutProducesTheCorrectCommand()
        {
            var query = new RiakMapReduceQuery
            {
                ContentType = MrContentType,
                Timeout = 100200
            }
                .Inputs("animals")
                .MapJs(m => m.Source("function(v) { return [v]; }").Keep(true));

            var request = query.ToMessage();
            request.content_type.ShouldEqual(MrContentType.ToRiakString());
            request.request.ShouldEqual(MrJobWithTimeoutText.ToRiakString());
        }

        [Test]
        public void BuildingComplexMapReduceJobsWithTheApiProducesTheCorrectCommand()
        {
            var query = new RiakMapReduceQuery
                {
                    ContentType = MrContentType
                }
                .Inputs("animals")
                .MapJs(m => m.Source("function(o) { if (o.key.indexOf('spider') != -1) return [1]; return []; }"))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));

            var request = query.ToMessage();
            request.request.ShouldEqual(ComplexMrJobText.ToRiakString());
        }

        [Test]
        public void BuildingComplexMapReduceJobsWithFiltersProducesTheCorrectCommand()
        {
            var query = new RiakMapReduceQuery
                {
                    ContentType = MrContentType
                }
                .Inputs("animals")
                //.Filter(new Matches<string>("spider"))
                .Filter(f => f.Matches("spider"))
                .MapJs(m => m.Source("function(o) { return [1]; }"))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));

            var request = query.ToMessage();
            request.request.ShouldEqual(ComplexMrJobWithFilterText.ToRiakString());
        }

        [Test]
        public void QueryingDollarKeyDoesNotAppendBinIndexSuffix() 
        {
            var query = new RiakMapReduceQuery 
                {
                    ContentType = MrContentType
                }
                .Inputs(RiakIndex.Range("animals", "$key", "0", "zzzzz"));

            var request = query.ToMessage();
            var requestString = request.request.FromRiakString();

            requestString.Contains("$key").ShouldBeTrue();
            requestString.Contains("$key_bin").ShouldBeFalse();
        }

        [Test]
        public void BuildingMapReducePhaseWithArgumentsArrayProducesCorrectResult()
        {
            var query = new RiakMapReduceQuery { ContentType = MrContentType }
                .Inputs("animals")
                .ReduceJs(c => c.Name("Riak.reduceSlice").Keep(true).Argument(new [] { 1, 10 }));

            var request = query.ToMessage();
            request.request.ShouldEqual(MrJobWithArgumentsArray.ToRiakString());
        }

        [Test]
        public void BuildingMapReducePhaseWithObjectArgumentProducesCorrectResult()
        {
            var query = new RiakMapReduceQuery { ContentType = MrContentType }
                .Inputs("animals")
                .ReduceJs(c => c.Name("Riak.reduceSlice").Keep(true).Argument(new { reduce_phase_only_1 = true }));

            var request = query.ToMessage();
            request.request.ShouldEqual(MrJobWithObjectArgument.ToRiakString());
        }

        [Test]
        public void BuildingMapReducePhaseWithVaueTypeArgumentProducesCorrectResult()
        {
            var query = new RiakMapReduceQuery { ContentType = MrContentType }
                .Inputs("animals")
                .ReduceJs(c => c.Name("Riak.reduceSlice").Keep(true).Argument("slartibartfast"));

            var request = query.ToMessage();
            request.request.ShouldEqual(MrJobWithValueTypeArgument.ToRiakString());
        }
    }
}
