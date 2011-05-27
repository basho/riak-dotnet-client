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
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CorrugatedIron.Models
{
    /* Looks like there will be some custom work to do to get MapReduce phases into a 
     * string properly. JSON.NET won't let us designate this class with a title for 
     * the SerDe process. We could, in theory, drop these into a 
     * Dictionary<string, RiakMapReducePhase> on the RiakMapReduce object and use that 
     * to generate the appropriate JSON to send to Riak.
     */
    public class RiakMapReducePhase : IRiakMapReducePhase
    {
        public string MapReducePhaseType { get; set; }
        public bool Keep { get; set; }
        public string MapReduceLanguage { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Argument { get; set; }

        public RiakMapReducePhase()
        {
        }

        public override string ToString()
        {
            return ToJsonString();
        }

        public string ToJsonString()
        {
            if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Source))
            {
                throw new Exception("One of Name or Source must be supplied");
            }

            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Source))
            {
                throw new Exception("Only one of Name and Source may be supplied");
            }

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

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

                if (!string.IsNullOrEmpty(Argument))
                {
                    jw.WritePropertyName("arg");
                    jw.WriteValue(Argument);
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