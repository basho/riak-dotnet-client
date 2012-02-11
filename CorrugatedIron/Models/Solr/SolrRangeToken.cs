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
using System.Text.RegularExpressions;

namespace CorrugatedIron.Models.Solr
{
    public class SolrRangeToken : SolrToken
    {
        public string From { get; set; }
        public string To { get; set; }
        public bool Inclusive { get; set; }
        new public string Term {
            get {
                return ToString();
            }
        }
        
        private string Open = "[";
        private string Close = "[";
        
        public override string ToString()
        {
            return ToSolrQueryString();
        }
        
        public override string ToSolrQueryString()
        {
            if (!Inclusive) {
                Open = "{";
                Close = "}";
            }
            
            return String.Format("{0}{1} TO {2}{3}", Open, EscapeTerm(From), EscapeTerm(To), Close);
        }
    }
}

