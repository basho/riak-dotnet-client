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
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Phases
{
    internal class RiakLinkPhase : RiakPhase
    {
        private string _bucket;
        private string _tag;

        public RiakLinkPhase()
        {
        }

        public RiakLinkPhase(RiakLink riakLink)
        {
            if(string.IsNullOrWhiteSpace(riakLink.Bucket)
                && string.IsNullOrWhiteSpace(riakLink.Key)
                && string.IsNullOrWhiteSpace(riakLink.Tag))
            {
                AllLinks();
            }
            else
            {
                _bucket = riakLink.Bucket;
                _tag = riakLink.Tag;
            }
        }

        public RiakLinkPhase FromRiakLink(RiakLink riakLink)
        {
            if(string.IsNullOrWhiteSpace(riakLink.Bucket)
                && string.IsNullOrWhiteSpace(riakLink.Key)
                && string.IsNullOrWhiteSpace(riakLink.Tag))
            {
                AllLinks();
            }
            else
            {
                _bucket = riakLink.Bucket;
                _tag = riakLink.Tag;
            }

            return this;
        }

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

        public RiakLinkPhase AllLinks()
        {
            _bucket = default(string);
            _tag = default(string);
            Keep(false);
            return this;
        }

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteSpecifiedProperty("bucket", _bucket)
                .WriteSpecifiedProperty("tag", _tag);
        }
    }
}