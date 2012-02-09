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
using CorrugatedIron.Models.RiakSearch;

namespace CorrugatedIron.Models.RiakSearch
{
    public class And : IRiakSearchTerm
    {
        private readonly Tuple<string, IRiakSearchTerm, IRiakSearchTerm> _booleanDefition;
        
        public And (IRiakSearchTerm left, IRiakSearchTerm right)
        {
            _booleanDefition = Tuple.Create("AND", left, right);
        }
        
        public string Name { get { return _booleanDefition.Item1; } }
        public IRiakSearchTerm Left { get { return _booleanDefition.Item2; } } 
        public IRiakSearchTerm Right { get { return _booleanDefition.Item3; } }
        
        
        public string ToRiakSearchString()
        {
            return String.Format("({0} {1} {2})", Left.ToRiakSearchString(), Right.ToRiakSearchString());
        }
    }
}

