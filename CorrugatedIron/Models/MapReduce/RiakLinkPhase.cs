// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce
{
    public class RiakLinkPhase : RiakPhase
    {
        private string _bucket;
        private string _tag;
        private string _key;

        public override string PhaseType
        {
            get { return "link"; }
        }

        public RiakLinkPhase Bucket(string bucket)
        {
            _bucket = bucket;
            return this;
        }

        public RiakLinkPhase Tag(string tag)
        {
            _tag = tag;
            return this;
        }

        public RiakLinkPhase Key(string key)
        {
            _key = key;
            return this;
        }

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteSpecifiedProperty("bucket", _bucket)
                .WriteSpecifiedProperty("tag", _tag)
                .WriteSpecifiedProperty("key", _key);
        }
    }
}
