// // Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
// //
// // This file is provided to you under the Apache License,
// // Version 2.0 (the "License"); you may not use this file
// // except in compliance with the License.  You may obtain
// // a copy of the License at
// //
// //   http://www.apache.org/licenses/LICENSE-2.0
// //
// // Unless required by applicable law or agreed to in writing,
// // software distributed under the License is distributed on an
// // "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// // KIND, either express or implied.  See the License for the
// // specific language governing permissions and limitations
// // under the License.
using System;
using CorrugatedIron.Models;
using CorrugatedIron.Util;
using CorrugatedIron.Tests.Extensions;
using NUnit.Framework;
using Newtonsoft.Json;

namespace CorrugatedIron.Tests
{
    [TestFixture]
    public class RiakMapReducePhaseTests
    {
        private const string MRMapPhaseString = "{\"map\":{\"language\":\"javascript\",\"keep\":true,\"source\":\"function(o) { return [1]; }\"}}";
        private const string MRReducePhaseString = "{\"reduce\":{\"language\":\"javascript\",\"keep\":true,\"name\":\"Riak.reduceSum\"}}";
        
        [SetUp]
        public void SetUp()
        { }
        
        [Test]
        public void ConvertingMapPhaseToPhaseStringProducesValidJson()
        {
            var phase = new RiakMapReducePhase();
            phase.Keep = true;
            phase.Source = "function(o) { return [1]; }";
            phase.MapReduceLanguage = Constants.MapReduceLanguage.JavaScript;
            phase.MapReducePhaseType = Constants.MapReducePhaseType.Map;
            
            var json = phase.ToPhaseString();
            
            json.ShouldEqual(MRMapPhaseString);
        }
        
        [Test]
        public void ConvertingReducePhaseToPhaseStringProducesValidJson()
        {
            var phase = new RiakMapReducePhase();
            phase.Keep = true;
            phase.Name = "Riak.reduceSum";
            phase.MapReduceLanguage = Constants.MapReduceLanguage.JavaScript;
            phase.MapReducePhaseType = Constants.MapReducePhaseType.Reduce;
            
            var json = phase.ToPhaseString();
            
            json.ShouldEqual(MRReducePhaseString);
        }
    }
}

