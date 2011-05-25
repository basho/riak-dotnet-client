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
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using CorrugatedIron.Util;
using CorrugatedIron.Messages;
using CorrugatedIron.Extensions;
using Newtonsoft.Json;

namespace CorrugatedIron
{
    /* Looks like there will be some custom work to do to get MapReduce phases into a 
     * string properly. JSON.NET won't let us designate this class with a title for 
     * the SerDe process. We could, in theory, drop these into a 
     * Dictionary<string, RiakMapReducePhase> on the RiakMapReduce object and use that 
     * to generate the appropriate JSON to send to Riak.
     */
    [JsonObject(MemberSerialization.OptIn)]
    public class RiakMapReducePhase
    {
        public string MapReducePhaseType { get; set; }
        
        [JsonProperty("keep")]
        public bool Keep { get; set; }
        
        // function magic
        
        [JsonProperty("language")]
        public string MapReduceLanguage { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("source")]
        public string Source { get; set; }
        
        public RiakMapReducePhase () { }
        
        
        public RiakMapReducePhase (string mapReducePhaseType, 
                                   string mapReduceLanguage,
                                   bool keep = true            
                                   )
        {
            MapReducePhaseType = mapReducePhaseType;
            MapReduceLanguage = mapReduceLanguage;
            Keep = keep;
        }
        
        public string ToJsonString() 
        {
            if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Source)) {
                throw new Exception("One of Name or Source must be supplied");
            }
            
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Source)) {
                throw new Exception("Only one of Name and Source may be supplied");
            }
            
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
             
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartObject();
                jw.WritePropertyName(MapReducePhaseType);
                
                // begin phase                
                jw.WriteStartObject();
                jw.WritePropertyName("language");
                jw.WriteValue(MapReduceLanguage);
                jw.WritePropertyName("keep");
                jw.WriteValue(Keep);
                
                if (!string.IsNullOrEmpty(Name))
                {
                    jw.WritePropertyName("name");
                    jw.WriteValue(Name);
                }
                
                if (!string.IsNullOrEmpty(Source))
                {
                    jw.WritePropertyName("source");
                    jw.WriteValue(Source);
                }                
                jw.WriteEndObject();
                // end phase
                
                jw.WriteEndObject();
            }
            
            return sb.ToString();
        }
        
        
        public static RiakMapReducePhase FromPhaseString(string phaseString)
        {
            throw new NotImplementedException();
        }
    }
}

