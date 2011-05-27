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
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Models
{
    [TestFixture]
    public class RiakMapReduceTests
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
        }

        #endregion

        private const string MrJobText =
            @"{""inputs"":""animals"",""query"":[{""map"":{""language"":""javascript"",""keep"":true,""source"":""function(v) { return [v]; }""}}]}";

        private const string ComplexMrJobText =
            @"{""inputs"":""animals"",""query"":[{""map"":{""language"":""javascript"",""keep"":false,""source"":""function(o) { if (o.key.indexOf('spider') != -1) return [1]; else return []; }""}},{""reduce"":{""language"":""javascript"",""keep"":true,""name"":""Riak.reduceSum""}}]}";

        private const string ComplexMrJobWithFilterText =
            @"{""inputs"":""animals"",""key_filters"":[[""matches"",""spider""]],""query"":[{""map"":{""language"":""javascript"",""keep"":false,""source"":""function(o) { return [1]; }""}},{""reduce"":{""language"":""javascript"",""keep"":true,""name"":""Riak.reduceSum""}}]}";

        private const string MrContentType = Constants.ContentTypes.ApplicationJson;

        [Test]
        public void ASimplePhaseMapReduceJobConvertsToByteArrays()
        {
            var mr = new RiakMapReduce
                         {
                             Request = MrJobText,
                             ContentType = Constants.ContentTypes.ApplicationJson
                         };

            var mrRequest = mr.ToMessage();

            mrRequest.ContentType.ShouldEqual(MrContentType.ToRiakString());
            mrRequest.Request.ShouldEqual(MrJobText.ToRiakString());
        }

        [Test]
        public void BuildingSimpleMapReduceJobsWithTheApiProducesByteArrays()
        {
            var mr = new RiakMapReduce
                         {
                             ContentType = Constants.ContentTypes.ApplicationJson
                         };
            mr.SetInputs("animals")
                .Map(true, Constants.MapReduceLanguage.Json, "function(v) { return [v]; }");

            var mrRequest = mr.ToMessage();

            mrRequest.ContentType.ShouldEqual(Constants.ContentTypes.ApplicationJson.ToRiakString());
            mrRequest.Request.ShouldEqual(MrJobText.ToRiakString());
        }

        [Test]
        public void BuildingComplexMapReduceJobsWithTheApiProducesTheCorrectCommand()
        {
            var mr = new RiakMapReduce();
            mr.SetInputs("animals")
                .Map(false, Constants.MapReduceLanguage.JavaScript,
                     "function(o) { if (o.key.indexOf('spider') != -1) return [1]; else return []; }")
                .Reduce(true, Constants.MapReduceLanguage.JavaScript, "", "Riak.reduceSum");

            mr.ToMessage().Request.ShouldEqual(ComplexMrJobText.ToRiakString());
        }

        [Test]
        public void BuildingComplexMapReduceJobsWithFiltersProducesTheCorrectCommand()
        {
            var mr = new RiakMapReduce();
            mr.SetInputs("animals")
                .Filter(new Matches<string>("spider"))
                .Map(false, Constants.MapReduceLanguage.JavaScript,
                     "function(o) { return [1]; }")
                .Reduce(true, Constants.MapReduceLanguage.JavaScript, "", "Riak.reduceSum");
            
            mr.ToMessage().Request.ShouldEqual(ComplexMrJobWithFilterText.ToRiakString());
        }

        [Test]
        public void BuildingComplexMapReduceJobsWithObjectInitializersProducesTheCorrectCommand()
        {
            var mr = new RiakMapReduce();
            mr.SetInputs("animals")
                .Filter(new Matches<string>("spider"))
                .Map(new RiakMapReducePhase
                         {
                             Keep = false,
                             MapReduceLanguage = Constants.MapReduceLanguage.JavaScript,
                             MapReducePhaseType = Constants.MapReducePhaseType.Map,
                             Source = "function(o) { return [1]; }"
                         })
                .Reduce(new RiakMapReducePhase
                            {
                                Keep = true,
                                MapReduceLanguage = Constants.MapReduceLanguage.JavaScript,
                                MapReducePhaseType = Constants.MapReducePhaseType.Reduce,
                                Name = "Riak.reduceSum"
                            });

            mr.ToMessage().Request.ShouldEqual(ComplexMrJobWithFilterText.ToRiakString());
        }
    }
}