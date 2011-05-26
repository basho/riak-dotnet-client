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
            @"{""map"":{""language"":""javascript"",""keep"":true,""source"":""function(v) { return [v]; }""}}";

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
            mr.Map(true, Constants.MapReduceLanguage.Json, "function(v) { return [v]; }");

            var mrRequest = mr.ToMessage();

            mrRequest.ContentType.ShouldEqual(Constants.ContentTypes.ApplicationJson.ToRiakString());
            mrRequest.Request.ShouldEqual(MrJobText.ToRiakString());
        }
    }
}