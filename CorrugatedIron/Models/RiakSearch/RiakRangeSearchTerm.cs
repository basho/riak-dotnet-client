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
    public class RiakRangeSearchTerm : IRiakSearchQueryTerm
    {
        public int From { get; set; }
        public int To { get; set; }
        public bool Inclusive { get; set; }
        
        private string Open = "[";
        private string Close = "[";
        
        public RiakRangeSearchTerm (int from, int to, bool inclusive = true)
        {
            From = from;
            To = to;
            Inclusive = inclusive;
        }
        
        public string ToRiakSearchTermString()
        {
            if (!Inclusive) {
                Open = "{";
                Close = "}";
            }
            
            return string.Format("{0}{1} TO {2}{3}", Open, From, To, Close);
        }
    }
}

