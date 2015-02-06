// <copyright file="RiakLinkPhase.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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
// </copyright>

namespace RiakClient.Models.MapReduce.Phases
{
    using Extensions;
    using Newtonsoft.Json;

    internal class RiakLinkPhase : RiakPhase
    {
        private string bucket;
        private string tag;

        public RiakLinkPhase()
        {
        }

        public RiakLinkPhase(RiakLink riakLink)
        {
            if (string.IsNullOrWhiteSpace(riakLink.Bucket)
                && string.IsNullOrWhiteSpace(riakLink.Key)
                && string.IsNullOrWhiteSpace(riakLink.Tag))
            {
                AllLinks();
            }
            else
            {
                bucket = riakLink.Bucket;
                tag = riakLink.Tag;
            }
        }

        public override string PhaseType
        {
            get { return "link"; }
        }

        public RiakLinkPhase FromRiakLink(RiakLink riakLink)
        {
            if (string.IsNullOrWhiteSpace(riakLink.Bucket)
                && string.IsNullOrWhiteSpace(riakLink.Key)
                && string.IsNullOrWhiteSpace(riakLink.Tag))
            {
                AllLinks();
            }
            else
            {
                bucket = riakLink.Bucket;
                tag = riakLink.Tag;
            }

            return this;
        }

        public RiakLinkPhase Bucket(string bucket)
        {
            this.bucket = bucket;
            return this;
        }

        public RiakLinkPhase Tag(string tag)
        {
            this.tag = tag;
            return this;
        }

        public RiakLinkPhase AllLinks()
        {
            bucket = default(string);
            tag = default(string);
            Keep(false);
            return this;
        }

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteSpecifiedProperty("bucket", bucket)
                .WriteSpecifiedProperty("tag", tag);
        }
    }
}
