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

using System;
using System.Text;
using System.Collections.Generic;

namespace CorrugatedIron.Models.Solr
{
    public class Group : ISolrQueryPart
    {
        List<ISolrQueryPart> _parts;
        
        public Group ()
        {
            _parts = new List<ISolrQueryPart>();
        }
        
        public Group AddItem(ISolrQueryPart item)
        {
            _parts.Add(item);
            return this;
        }
        
        public override string ToString()
        {
            return ToSolrTerm();
        }

        public string ToSolrTerm()
        {
            var sb = new StringBuilder();
            
            sb.Append("(");
            
            foreach (var part in _parts)
            {
                sb.Append(part.ToString());
            }
            
            sb.Append(")");
            
            return sb.ToString();
        }
    }
}

