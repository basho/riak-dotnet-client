// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
// 
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CorrugatedIron.KeyFilters
{
    public class RiakNValueKeyFilter : IRiakKeyFilter
    {
        public string FunctionName { get; set; }
        public object[] Arguments {get; set;}
        
        public RiakNValueKeyFilter() 
        {
            
        }
        
        public RiakNValueKeyFilter (string functionName, params object[] args)
        {
            FunctionName = functionName;
            Arguments = args;
        }
        
        public override string ToString()
        {
            return ToJsonString();
        }
        
        public string ToJsonString()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartArray();
                jw.WriteStartArray();
                jw.WriteValue(FunctionName);
                
                (new List<object>(Arguments)).ForEach( arg => jw.WriteValue(arg) );
                
                jw.WriteEndArray();
                jw.WriteEndArray();
            }
            
            return sb.ToString();
        }
    }
}

