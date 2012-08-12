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
using System.Text;

namespace CorrugatedIron.Models.RiakSearch
{
    public class RiakSearchToken : RiakSearchQueryPart
    {
        public string Field { get; set; }
        public RiakSearchTerm Term { get; set; }
        public int? Proximity { get; set; } 
        // TODO: Boost must be positive
        public decimal? Boost { get; set; }
        public bool Required { get; set; }
        public bool Prohibit { get; set; }
        
        public RiakSearchToken()
        {
            Required = false;
            Prohibit = false;
        }
        
        public override void ToSearchTerm(StringBuilder sb)
        {
            if (Required) 
            {
                sb.Append("+");
            }
            
            if (Prohibit) 
            {
                sb.Append("-");
            }
            
            if (!string.IsNullOrWhiteSpace(Field)) 
            {
                sb.Append(Field);
                sb.Append(":");
            }

            Term.ToSearchTerm(sb);
            
            if (Proximity.HasValue) 
            {
                sb.AppendFormat("~{0}", Proximity);
            }
            
            if (Boost.HasValue) 
            {
                sb.AppendFormat("^{0}", Boost);
            }
        }
    }
}

