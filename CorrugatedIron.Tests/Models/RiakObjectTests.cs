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
            riakObject.AddIndex("name", "jeremiah");
            riakObject.AddIndex("state_bin", "oregon");
            riakObject.AddIndex("age", 32);
            riakObject.AddIndex("cats_int", 2);
            
            riakObject.Indexes.Keys.Contains("name").ShouldBeFalse();
            riakObject.Indexes.Keys.Contains("age").ShouldBeFalse();
            riakObject.Indexes.Keys.Contains("state").ShouldBeFalse();
            riakObject.Indexes.Keys.Contains("state_bin_bin").ShouldBeFalse();
            riakObject.Indexes.Keys.Contains("cats").ShouldBeFalse();
            riakObject.Indexes.Keys.Contains("cats_int_int").ShouldBeFalse();
            
            riakObject.Indexes.Keys.Contains("name_bin").ShouldBeTrue();
            riakObject.Indexes.Keys.Contains("age_int").ShouldBeTrue();
            riakObject.Indexes.Keys.Contains("state_bin").ShouldBeTrue();
            riakObject.Indexes.Keys.Contains("cats_int").ShouldBeTrue();
        }
    }
}
