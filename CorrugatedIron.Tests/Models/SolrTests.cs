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

using CorrugatedIron.Extensions;
using CorrugatedIron.Models.Solr;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Models
{
    [TestFixture()]
    public class BasicSolrTokenTests
    {
        [Test]
        public void SolrTermCorrectlyEscapesOneSpecialCharacter() 
        {
            var token = new SolrToken();
            token.Term = "2+2";
            
            string escapedString = token.ToString();
            
            escapedString.Contains(@"\").ShouldBeTrue();
            escapedString.Equals(@"2\+2").ShouldBeTrue();
        }
        
        [Test]
        public void SolrTermCorrectlyEscapesMultipleSpecialCharacters()
        {
            var token = new SolrToken();
            token.Term = "2+2-2";
            
            string escapedString = token.ToString();
            
            escapedString.Contains(@"\").ShouldBeTrue();
            escapedString.Equals(@"2\+2\-2").ShouldBeTrue();
        }
        
        [Test]
        public void SolrTermIncludesFieldName()
        {
            var token = new SolrToken();
            token.Field = "eyes";
            token.Term = "blue";
            
            string result = token.ToString();
            
            result.Contains("eyes").ShouldBeTrue();
            result.Contains("blue").ShouldBeTrue();
            result.Equals("eyes:blue").ShouldBeTrue();
        }
        
        [Test]
        public void SolrTermWithSpacesShouldBeQuoted()
        {
            var token = new SolrToken();
            token.Field = "artist";
            token.Term = "The Rolling Stones";
            
            string result = token.ToString();
            
            result.Equals(@"artist:""The Rolling Stones""").ShouldBeTrue();
        }
        
        [Test]
        public void RequiredTermsShouldBePrefixedWithAPlusSign()
        {
            var token = new SolrToken();
            token.Term = "riak";
            token.Required = true;
            
            string result = token.ToString();
            
            result.Contains("+").ShouldBeTrue();
            result.Equals("+riak").ShouldBeTrue();
        }
        
        [Test]
        public void BoostedTermsContainACaret()
        {
            var token = new SolrToken();
            token.Term = "Erlang";
            token.Boost = 10;
            
            string result = token.ToString();
            
            result.Contains("^").ShouldBeTrue();
            result.Equals("Erlang^10").ShouldBeTrue();
        }
    
        [Test]
        public void BoostedPhrasesContainACaretOutsideOfQuotedString()
        {
            var token = new SolrToken();
            token.Field = "artist";
            token.Term = "The Rolling Stones";
            token.Boost = 10;
            
            string result = token.ToString();
            
            result.Contains("^").ShouldBeTrue();
            result.Equals(@"artist:""The Rolling Stones""^10").ShouldBeTrue();
        }
    }
}

