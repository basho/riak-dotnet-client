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

using System.Text;
using CorrugatedIron.Extensions;

namespace CorrugatedIron.Models.RiakSearch
{
    public class RiakSearchRangeToken : RiakSearchQueryPart
    {
        public string From { get; set; }
        public string To { get; set; }
        public bool Inclusive { get; set; }
        
        public override void ToSearchTerm(StringBuilder sb)
        {
            sb.AppendFormat(Inclusive ? "[{0} TO {1}]" : "{{{0} TO {1}}}", From.ToRiakSearchTerm(), To.ToRiakSearchTerm());
        }
    }
}

