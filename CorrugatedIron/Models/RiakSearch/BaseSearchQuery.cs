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

namespace CorrugatedIron.Models.RiakSearch
{
    public abstract class BaseSearchQuery
    {
        /// <summary>
        /// The named index to be used for the search.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// The query to be run. This obeys the usual SOLR query syntax. See http://lucene.apache.org/solr/tutorial.html#Querying+Data
        /// </summary>
        public string Query { get { return QueryPart.ToSearchTerm(); } }

        public RiakSearchQueryPart QueryPart;

        public abstract string ToQueryString();
    }
}
