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

using CorrugatedIron.Models;
using CorrugatedIron.Tests.Extensions;
using NUnit.Framework;
using System.Linq;

namespace CorrugatedIron.Tests.Models
{
    [TestFixture]
    public class RiakObjectTests
    {
        private const string Bucket = "bucket";
        private const string Key = "key";

        [Test]
        public void ToRiakObjectIdProducesAValidRiakObjectId()
        {
            var riakObject = new RiakObject(Bucket, Key, "value");
            var riakObjectId = riakObject.ToRiakObjectId();

            riakObjectId.Bucket.ShouldEqual(Bucket);
            riakObjectId.Key.ShouldEqual(Key);
        }
        
        [Test]
        public void RiakIndexNameManglingIsHandledAutomatically()
        {
            var riakObject = new RiakObject(Bucket, Key, "value");
            riakObject.BinIndex("name").Set("jeremiah");
            riakObject.BinIndex("state_bin").Set("oregon");
            riakObject.IntIndex("age").Add(32);
            riakObject.IntIndex("cats_int").Add(2);
            
            riakObject.BinIndexes.Values.Select(v => v.RiakIndexName).Contains("name").ShouldBeFalse();
            riakObject.BinIndexes.Values.Select(v => v.RiakIndexName).Contains("name_bin").ShouldBeTrue();
            riakObject.BinIndexes.Values.Select(v => v.RiakIndexName).Contains("state").ShouldBeFalse();
            riakObject.BinIndexes.Values.Select(v => v.RiakIndexName).Contains("state_bin").ShouldBeFalse();
            riakObject.BinIndexes.Values.Select(v => v.RiakIndexName).Contains("state_bin_bin").ShouldBeTrue();

            riakObject.IntIndexes.Values.Select(v => v.RiakIndexName).Contains("age").ShouldBeFalse();
            riakObject.IntIndexes.Values.Select(v => v.RiakIndexName).Contains("age_int").ShouldBeTrue();
            riakObject.IntIndexes.Values.Select(v => v.RiakIndexName).Contains("cats").ShouldBeFalse();
            riakObject.IntIndexes.Values.Select(v => v.RiakIndexName).Contains("cats_int").ShouldBeFalse();
            riakObject.IntIndexes.Values.Select(v => v.RiakIndexName).Contains("cats_int_int").ShouldBeTrue();
        }

        [Test]
        public void RiakIndexingSupportsMultipleValuesCorrectly()
        {
            var riakObject = new RiakObject(Bucket, Key, "value");

            riakObject.BinIndex("jobs").Set("dogsbody");
            riakObject.BinIndex("jobs").Values.Count.ShouldEqual(1);

            riakObject.BinIndex("jobs").Add("toilet cleaner", "president", "juggler");
            riakObject.BinIndex("jobs").Values.Count.ShouldEqual(4);

            riakObject.BinIndex("jobs").Remove("dogsbody", "juggler");
            riakObject.BinIndex("jobs").Values.Count.ShouldEqual(2);

            riakObject.BinIndex("jobs").Set("general", "engineer", "cook");
            riakObject.BinIndex("jobs").Values.Count.ShouldEqual(3);
            riakObject.BinIndex("jobs").Values.Contains("general").ShouldBeTrue();

            riakObject.BinIndex("jobs").Clear();
            riakObject.BinIndex("jobs").Values.Count.ShouldEqual(0);

            riakObject.BinIndex("jobs").Delete();
            riakObject.BinIndexes.ContainsKey("jobs").ShouldBeFalse();

            riakObject.IntIndex("years").Set(10);
            riakObject.IntIndex("years").Values.Count.ShouldEqual(1);

            riakObject.IntIndex("years").Add(20, 40, 999);
            riakObject.IntIndex("years").Values.Count.ShouldEqual(4);

            riakObject.IntIndex("years").Remove(40, 999);
            riakObject.IntIndex("years").Values.Count.ShouldEqual(2);

            riakObject.IntIndex("years").Set(51, 52, 53);
            riakObject.IntIndex("years").Values.Count.ShouldEqual(3);
            riakObject.IntIndex("years").Values.Contains(52).ShouldBeTrue();

            riakObject.IntIndex("years").Clear();
            riakObject.IntIndex("years").Values.Count.ShouldEqual(0);

            riakObject.IntIndex("years").Delete();
            riakObject.IntIndexes.ContainsKey("years").ShouldBeFalse();
        }
    }
}
