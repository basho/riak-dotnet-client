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
using System.Collections.Generic;
using CorrugatedIron.Messages;
using CorrugatedIron.Util;
using CorrugatedIron.Extensions;
using CorrugatedIron.KeyFilters;

namespace CorrugatedIron
{
    public class RiakMapReduce
    {
        public Dictionary<string, RiakMapReducePhase> MapReducePhases { get; set; }
        
        public string Request { get; set; }
        
        public string Bucket { get; set; }
        public List<IRiakKeyFilter> Filters { get; set; }
        
        // TODO refactor to use phases to build map reduce request on the fly
        public RiakMapReduce (string bucket, string request = "") 
        {
            Bucket = bucket;
            Request = request;
        }
        
        public RpbMapRedReq ToMessage() {
            var message = new RpbMapRedReq {
                Request = this.Request.ToRiakString()
            };
            
           return message;
        }
        
        // TODO add support for passing arguments
        public RiakMapReduce Map (string module, string function, object[] args = null) 
        {
            throw new System.NotImplementedException();
        }
        
        public RiakMapReduce Map (bool keep, string language, string source = "", string name = "") 
        {
            AddMapReducePhase (keep, language, source, name, Constants.MapReducePhaseType.Map);
            
            return this;
        }
        
        public RiakMapReduce Reduce (bool keep, string language, string source = "", string name = "") 
        {
            AddMapReducePhase (keep, language, source, name, Constants.MapReducePhaseType.Reduce);
            
            return this;
        }
        
        public RiakMapReduce Link () 
        {
            throw new System.NotImplementedException();
        }

        private void AddMapReducePhase (bool keep, string language, string source, string name, string mapReducePhaseType)
        {
            var phase = new RiakMapReducePhase (mapReducePhaseType, language, keep) { Source = source, Name = name };
            MapReducePhases.Add(phase.MapReducePhaseType, phase);
        }
    }
}

