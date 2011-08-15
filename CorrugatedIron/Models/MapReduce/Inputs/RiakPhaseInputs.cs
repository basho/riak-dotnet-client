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

//using System.Collections.Generic;
//using CorrugatedIron.Extensions;
//using CorrugatedIron.Models.MapReduce.KeyFilters;
//using Newtonsoft.Json;

//namespace CorrugatedIron.Models.MapReduce.Inputs
//{
//    public class RiakPhaseInputs
//    {
//        private readonly IRiakPhaseInput _input;

//        public List<IRiakKeyFilterToken> Filters { get; set; } 

//        public RiakPhaseInputs(RiakBucketInput bucketInput)
//        {
//            _input = bucketInput;
//            Filters = new List<IRiakKeyFilterToken>();
//        }

//        public RiakPhaseInputs(RiakBucketKeyInput bucketKeyInputs)
//        {
//            _input = bucketKeyInputs;
//            Filters = new List<IRiakKeyFilterToken>();
//        }

//        public RiakPhaseInputs(RiakBucketKeyKeyDataInput bucketKeyKeyDataInputs)
//        {
//            _input = bucketKeyKeyDataInputs;
//            Filters = new List<IRiakKeyFilterToken>();
//        }

//        public JsonWriter WriteJson(JsonWriter writer)
//        {
//            return _input.WriteJson(writer);
//        }
//    }
//}
