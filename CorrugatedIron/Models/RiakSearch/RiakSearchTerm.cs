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
using CorrugatedIron.Models.RiakSearch;

namespace CorrugatedIron.Models.RiakSearch
{
    public class RiakSearchTerm : IRiakSearchTerm
    {
        public string Field { get; set; }
        public IRiakSearchQueryTerm Term { get; set; }
        public int? Proximity { get; set; }
        public int? Boost { get; set; }
        public bool Prohibited { get; set; }
        
        public override string ToString()
        {
            return ToRiakSearchString();
        }
        
        public string ToRiakSearchString ()
        {
            var sb = new StringBuilder();
            
            if (Prohibited) {
                sb.Append("NOT ");
            }
            
            if (!String.IsNullOrEmpty(Field)) {
                sb.Append(Field);
                sb.Append(":");
            }
            
            sb.Append(EscapeTerm());
            
            if (Proximity.HasValue) {
                sb.Append(string.Format("~{0}", Proximity.Value));
            }
            
            if (Boost.HasValue) {
                sb.Append(string.Format("^{0}", Boost.Value));   
            }
            
            return sb.ToString();
        }
        
        private string EscapeTerm() {
            string pattern = @"[+-/[](): ]";
            string replacement = @"\$1";
            
            var regex = new Regex(pattern);
            return regex.Replace(Term.ToRiakSearchTermString(), replacement);
        }
    }
}

