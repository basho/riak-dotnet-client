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

using CorrugatedIron.Models.MapReduce.Phases;

namespace CorrugatedIron.Models.MapReduce.Fluent
{
    public class RiakFluentLinkPhase
    {
        private readonly RiakLinkPhase _phase;

        internal RiakFluentLinkPhase(RiakLinkPhase phase)
        {
            _phase = phase;
        }

        public RiakFluentLinkPhase Keep(bool keep)
        {
            _phase.Keep(keep);
            return this;
        }

        public RiakFluentLinkPhase Bucket(string bucket)
        {
            _phase.Bucket(bucket);
            return this;
        }

        public RiakFluentLinkPhase Tag(string tag)
        {
            _phase.Tag(tag);
            return this;
        }

        public RiakFluentLinkPhase FromRiakLink(RiakLink link)
        {
            _phase.FromRiakLink(link);
            return this;
        }

        public RiakFluentLinkPhase Empty()
        {
            _phase.Empty();
            return this;
        }
    }
}
