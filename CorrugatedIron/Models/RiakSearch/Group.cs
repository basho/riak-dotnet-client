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
using System.Collections.Generic;
using System.Text;

namespace CorrugatedIron.Models.RiakSearch
{
    public class Group : IRiakSearchTerm
    {
        private List<IRiakSearchTerm> _groupItems;
            
        public Group ()
        {
            _groupItems = new List<IRiakSearchTerm>();
        }
        
        public Group(List<IRiakSearchTerm> groupItems)
        {
            _groupItems = groupItems;
        }
        
        public override string ToString()
        {
            return ToRiakSearchString();
        }

        public string ToRiakSearchString()
        {
            var sb = new StringBuilder();
            
            sb.Append("(");
            
            _groupItems.ForEach(gi => sb.Append(gi.ToString()));
            
            sb.Append(")");
            
            return sb.ToString();
        }
    }
}

