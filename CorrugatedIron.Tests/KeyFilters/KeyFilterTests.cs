// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Models.MapReduce.KeyFilters;
using CorrugatedIron.Tests.Extensions;
using NUnit.Framework;
using System.Collections.Generic;

namespace CorrugatedIron.Tests.KeyFilters
{
    [TestFixture]
    public abstract class KeyFilterTests
    {
        internal const string IntToStringJson = @"[""int_to_string""]";
        internal const string StringToIntJson = @"[""string_to_int""]";
        internal const string FloatToStringJson = @"[""float_to_string""]";
        internal const string StringToFloatJson = @"[""string_to_float""]";
        internal const string ToUpperJson = @"[""to_upper""]";
        internal const string ToLowerJson = @"[""to_lower""]";
        internal const string TokenizeJson = @"[""tokenize"",""/"",4]";
        internal const string UrlDecodeJson = @"[""urldecode""]";
        
        internal const string GreaterThanJson = @"[""greater_than"",50]";
        internal const string LessThanJson = @"[""less_than"",10]";
        internal const string GreaterThanOrEqualToJson = @"[""greater_than_eq"",2000]";
        internal const string LessThanOrEqualToJson = @"[""less_than_eq"",-2]";
        internal const string BetweenJson = @"[""between"",10,20,false]";
        internal const string MatchesJson = @"[""matches"",""solutions""]";
        internal const string NotEqualJson = @"[""neq"",""foo""]";
        internal const string EqualJson = @"[""eq"",""basho""]";
        internal const string SetMemberJson = @"[""set_member"",""basho"",""google"",""yahoo""]";
        internal const string SimilarToJson = @"[""similar_to"",""newyork"",3]";
        internal const string StartsWithJson = @"[""starts_with"",""closed""]";
        internal const string EndsWithJson = @"[""ends_with"",""0603""]";

        internal const string AndJson = @"[""and"",[[""ends_with"",""0603""]],[[""starts_with"",""basho""]]]";
        internal const string OrJson = @"[""or"",[[""eq"",""google""]],[[""less_than"",""g""]]]";
        internal const string NotJson = @"[""not"",[[""matches"",""solution""]]]";
    }
    
    public class WhenConstructingSimpleKeyFilters : KeyFilterTests
    {
        [Test]
        public void IntToStringCorrectlyConvertsToJson ()
        {
            var its = new IntToString();
            its.ToString().ShouldEqual(IntToStringJson);
        }
        
        [Test]
        public void StringToIntCorrectlyConvertsToJson()
        {
            var sti = new StringToInt();
            sti.ToString().ShouldEqual(StringToIntJson);
        }
        
        [Test]
        public void FloatToStringCorrectlyConvertsToJson()
        {
            var fts = new FloatToString();
            fts.ToString().ShouldEqual(FloatToStringJson);
        }
        
        [Test]
        public void StringToFloatCorrectlyConvertsToJson()
        {
            var stf = new StringToFloat();
            stf.ToString().ShouldEqual(StringToFloatJson);
        }
        
        [Test]
        public void ToUpperCorrectlyConvertsToJson()
        {
            var tu = new ToUpper();
            tu.ToString().ShouldEqual(ToUpperJson);
        }
        
        [Test]
        public void ToLowerCorrectlyConvertsToJson() {
            var tl = new ToLower();
            tl.ToString().ShouldEqual(ToLowerJson);
        }
        
        [Test]
        public void TokenizeCorrectlyConvertsToJson() {
            var tokenize = new Tokenize("/", 4);
            tokenize.ToString().ShouldEqual(TokenizeJson);
        }

        [Test]
        public void UrlDecodeCorrectlyConvertsToJson()
        {
            var ud = new UrlDecode();
            ud.ToString().ShouldEqual(UrlDecodeJson);
        }
    }

    public class WhenConstructingSimplePredicates : KeyFilterTests
    {
        [Test]
        public void GreaterThanCorrectlyConvertsToJson()
        {
            var gt = new GreaterThan<int>(50);
            gt.ToString().ShouldEqual(GreaterThanJson);
        }
        
        [Test]
        public void LessThanCorrectlyConvertsToJson()
        {
            var lt = new LessThan<int>(10);
            lt.ToString().ShouldEqual(LessThanJson);
        }
        
        [Test]
        public void GreaterThanOrEqualCorrectlyConvertsToJson()
        {
            var gte = new GreaterThanOrEqualTo<int>(2000);
            gte.ToString().ShouldEqual(GreaterThanOrEqualToJson);
        }
        
        [Test]
        public void LessThanOrEqualCorrectlyConvertsToJson()
        {
            var lte = new LessThanOrEqualTo<int>(-2);
            lte.ToString().ShouldEqual(LessThanOrEqualToJson);
        }
        
        [Test]
        public void BetweenCorrectlyConvertsToJson()
        {
            var between = new Between<int>(10, 20, false);
            between.ToString().ShouldEqual(BetweenJson);
        }
        
        [Test]
        public void MatchesCorrectlyConvertsToJson()
        {
            var matches = new Matches("solutions");
            matches.ToString().ShouldEqual(MatchesJson);
        }
        
        [Test]
        public void NotEqualCorrectlyConvertsToJson()
        {
            var neq = new NotEqual<string>("foo");
            neq.ToString().ShouldEqual(NotEqualJson);
        }
        
        [Test]
        public void EqualCorrectlyConvertsToJson()
        {
            var eq = new Equal<string>("basho");
            eq.ToString().ShouldEqual(EqualJson);
        }
        
        [Test]
        public void SetMemberCorrectlyConvertsToJson()
        {
            var setMember = new SetMember<string>(new List<string>{"basho","google","yahoo"});
            setMember.ToString().ShouldEqual(SetMemberJson);
        }
        
        [Test]
        public void SimilarToCorrectlyConvertsToJson()
        {
            var st = new SimilarTo<string>("newyork", 3);
            st.ToString().ShouldEqual(SimilarToJson);
        }
        
        [Test]
        public void StartsWithCorrectlyConvertsToJson()
        {
            var sw = new StartsWith("closed");
            sw.ToString().ShouldEqual(StartsWithJson);
        }
        
        [Test]
        public void EndsWithCorrectlyConvertsToJson()
        {
            var ew = new EndsWith("0603");
            ew.ToString().ShouldEqual(EndsWithJson);
        }
    }

    public class WhenConstructingComplexPredicates : KeyFilterTests
    {
        [Test]
        [Ignore]
        public void AndCorrectlyConvertsToJson()
        {
            //var and = new And(new EndsWith("0603"), new StartsWith("basho"));
            //and.ToString().ShouldEqual(AndJson);
        }
        
        [Test]
        [Ignore]
        public void OrCorrectlyConvertsToJson()
        {
            //var or = new Or(new Equal<string>("google"), new LessThan<string>("g"));
            //or.ToString().ShouldEqual(OrJson);
        }
        
        [Test]
        [Ignore]
        public void NotCorrectlyConvertsToJson()
        {
            //var not = new Not(new Matches("solution"));
            //not.ToString().ShouldEqual(NotJson);
        }
    }
}

