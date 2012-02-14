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

namespace CorrugatedIron.Models.Solr
{
    public class And : ISolrQueryPart
    {
        private readonly Tuple<string, ISolrQueryPart, ISolrQueryPart> _booleanDefition;
        
        public And (ISolrQueryPart left, ISolrQueryPart right)
        {
            _booleanDefition = Tuple.Create("AND", left, right);
        }
        
        public string Name { get { return _booleanDefition.Item1; } }
        public ISolrQueryPart Left { get { return _booleanDefition.Item2; } } 
        public ISolrQueryPart Right { get { return _booleanDefition.Item3; } }
        
        public override string ToString() 
        {
            return ToSolrTerm();
        }   
        
        public string ToSolrTerm()
        {
            return String.Format("{0} {1} {2}", Left.ToString(), Name, Right.ToString());
        }
    }
}

