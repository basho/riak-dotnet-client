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

namespace CorrugatedIron.Models.Solr
{
    /// <summary>
    /// Riak solr query. SOLR syntax is a combination of http://wiki.apache.org/solr/SolrQuerySyntax and http://lucene.apache.org/java/2_9_1/queryparsersyntax.html
    /// </summary>
    public class SolrQuery
    {
        public string Index { get; set; }
        /// <summary>
        /// The query to be run. This obeys the usual SOLR query syntax. See http://lucene.apache.org/solr/tutorial.html#Querying+Data
        /// </summary>
        public string Query { get; set; }
        public string Fieldname { get; set; }
        public ISolrQueryPart Token { get; set; }
        // TODO restrict operation to "and" or "or"
        public string Operation { get; set; }
        public int? StartOffset { get; set; }
        public int? Rows { get; set; }
        public string SortField { get; set; }
        public string OutputFormat { get; set; }
        public string FilterQuery { get; set; }
        public string SolrPrefix { get; set; }
        
        public SolrQuery ()
        {
            SolrPrefix = "solr";
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            
            sb.Append("/");
            sb.Append(SolrPrefix);
            sb.Append("/");
            sb.Append(Index);
            sb.Append("/");
            sb.Append("select?");
            
            var searchTerms = new List<string>();
            
            if (!String.IsNullOrEmpty(Query)) 
            {
                searchTerms.Add(string.Format("q={0}", Query));
            }
            
            if (!String.IsNullOrEmpty(Fieldname)) 
            {
                searchTerms.Add(string.Format("df={0}", Fieldname));
            }
            
            if (!String.IsNullOrEmpty(Operation)) 
            {
                searchTerms.Add(string.Format("q.op={0}", Operation));
            }
            
            if (StartOffset.HasValue) 
            {
                searchTerms.Add(string.Format("start={0}", StartOffset.Value));
            }
            
            if (Rows.HasValue) 
            {
                searchTerms.Add(string.Format("rows={0}", Rows.Value));
            }
            
            if (!String.IsNullOrEmpty(SortField)) 
            {
                searchTerms.Add(string.Format("sort={0}", SortField));
            }
            
            if (!String.IsNullOrEmpty(OutputFormat)) 
            {
                searchTerms.Add(string.Format("wt={0}", OutputFormat));
            }
            
            if (!String.IsNullOrEmpty(FilterQuery)) 
            {
                searchTerms.Add(string.Format("filter={0}", FilterQuery));
            }
            
            sb.Append(String.Join("&", searchTerms));
                
            return sb.ToString();
        }
    }
}

