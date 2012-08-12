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

using System.Text;
using System.Collections.Generic;

namespace CorrugatedIron.Models.RiakSearch
{
    public class Group : RiakSearchQueryPart
    {
        private List<RiakSearchQueryPart> _parts;
        
        public Group()
        {
            _parts = new List<RiakSearchQueryPart>();
        }
        
        public Group AddItem(RiakSearchQueryPart item)
        {
            _parts.Add(item);
            return this;
        }
        
        public override void ToSearchTerm(StringBuilder sb)
        {
            sb.Append("(");
            
            foreach (var part in _parts)
            {
                part.ToSearchTerm(sb);
            }
            
            sb.Append(")");
        }
    }
}

