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

using CorrugatedIron.Models.RiakSearch;
using CorrugatedIron.Tests.Extensions;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Models
{
    [TestFixture]
    public class BasicSolrTokenTests
    {
        [Test]
        public void SolrTermCorrectlyEscapesOneSpecialCharacter() 
        {
            var token = new RiakSearchToken();
            var spt = new RiakSearchPhraseToken {Term = "2+2"};
            token.Term = spt;
            
            string escapedString = token.ToString();
            
            escapedString.Contains(@"\").ShouldBeTrue();
            escapedString.Equals(@"2\+2").ShouldBeTrue();
        }
        
        [Test]
        public void SolrTermCorrectlyEscapesMultipleSpecialCharacters()
        {
            var token = new RiakSearchToken();
            var spt = new RiakSearchPhraseToken {Term = "2+2-2"};
            token.Term = spt;
            
            string escapedString = token.ToString();
            
            escapedString.Contains(@"\").ShouldBeTrue();
            escapedString.Equals(@"2\+2\-2").ShouldBeTrue();
        }
        
        [Test]
        public void SolrTermIncludesFieldName()
        {
            var token = new RiakSearchToken {Field = "eyes"};
            var spt = new RiakSearchPhraseToken {Term = "blue"};
            token.Term = spt;
            
            string result = token.ToString();
            
            result.Contains("eyes").ShouldBeTrue();
            result.Contains("blue").ShouldBeTrue();
            result.Equals("eyes:blue").ShouldBeTrue();
        }
        
        [Test]
        public void SolrTermWithSpacesShouldBeQuoted()
        {
            var token = new RiakSearchToken {Field = "artist"};

            var spt = new RiakSearchPhraseToken {Term = "The Rolling Stones"};

            token.Term = spt;
            
            string result = token.ToString();
            
            result.Equals(@"artist:""The Rolling Stones""").ShouldBeTrue();
        }
        
        [Test]
        public void RequiredTermsShouldBePrefixedWithAPlusSign()
        {
            var token = new RiakSearchToken {Required = true};

            var spt = new RiakSearchPhraseToken {Term = "riak"};

            token.Term = spt;
            
            string result = token.ToString();
            
            result.Contains("+").ShouldBeTrue();
            result.Equals("+riak").ShouldBeTrue();
        }
        
        [Test]
        public void BoostedTermsShouldContainACaret()
        {
            var token = new RiakSearchToken {Boost = 10};
            var spt = new RiakSearchPhraseToken {Term = "Erlang"};

            token.Term = spt;
            
            string result = token.ToString();
            
            result.Contains("^").ShouldBeTrue();
            result.Equals("Erlang^10").ShouldBeTrue();
        }
    
        [Test]
        public void BoostedPhrasesShouldContainACaretOutsideOfQuotedString()
        {
            var token = new RiakSearchToken {Field = "artist", Boost = 10};
            var spt = new RiakSearchPhraseToken {Term = "The Rolling Stones"};

            token.Term = spt;
            
            string result = token.ToString();
            
            result.Contains("^").ShouldBeTrue();
            result.ShouldEqual(@"artist:""The Rolling Stones""^10");
        }
    }
    
    [TestFixture]
    public class SolrRangeTokenTests
    {
        [Test]
        public void InclusiveTermShouldBeFormattedCorrecly()
        {
            var token = new RiakSearchToken();
            var srt = new RiakSearchRangeToken {From = "20020101", To = "20030101", Inclusive = true};

            token.Field = "mod_date";
            token.Term = srt;
            
            token.Term.ToSearchTerm().ShouldEqual("[20020101 TO 20030101]");
            token.ToString().ShouldEqual("mod_date:[20020101 TO 20030101]");
        }
        
        [Test]
        public void ExclusiveTermShouldBeFormattedCorrecly()
        {
            var token = new RiakSearchToken {Field = "title"};
            var srt = new RiakSearchRangeToken {From = "Aida", To = "Carmen", Inclusive = false};

            token.Term = srt;
            
            token.Term.ToSearchTerm().ShouldEqual("{Aida TO Carmen}");
            token.ToString().ShouldEqual("title:{Aida TO Carmen}");
        }
    }
    
    [TestFixture]
    public class BooleanSolrTokenTests
    {
        [Test]
        public void AndTokensAreFormattedCorrectly()
        {
            var first = new RiakSearchToken {Term = new RiakSearchPhraseToken("coffee")};
            var second = new RiakSearchToken {Term = new RiakSearchPhraseToken("tea")};
            var and = new And(first, second);
            
            var result = and.ToString();
            
            result.Contains("AND").ShouldBeTrue();
            result.ShouldEqual("coffee AND tea");
        }
        
        [Test]
        public void OrTokensAreFormattedCorrectly()
        {
            var first = new RiakSearchToken {Term = new RiakSearchPhraseToken("coffee")};
            var second = new RiakSearchToken {Term = new RiakSearchPhraseToken("tea")};
            var or = new Or(first, second);
            
            var result = or.ToString();
            
            result.Contains("OR").ShouldBeTrue();
            result.ShouldEqual("coffee OR tea");
        }
        
        [Test]
        public void GroupTokensAreFormattedCorrectly()
        {
            var first = new RiakSearchToken {Term = new RiakSearchPhraseToken("coffee")};
            var second = new RiakSearchToken {Term = new RiakSearchPhraseToken("tea")};
            var or = new Or(first, second);
            
            var g = new Group();
            g.AddItem(or);
            
            string result = g.ToString();
            
            result.Contains("OR").ShouldBeTrue();
            result.ShouldEqual("(coffee OR tea)");
        }
    }
}

