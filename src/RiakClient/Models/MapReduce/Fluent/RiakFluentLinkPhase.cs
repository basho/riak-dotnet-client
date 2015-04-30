// <copyright file="RiakFluentLinkPhase.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

namespace RiakClient.Models.MapReduce.Fluent
{
    using System;
    using Models.MapReduce.Phases;

    /// <summary>
    /// A fluent builder class for defining input linkwalking phases.
    /// </summary>
    [Obsolete("Linkwalking is a deprecated feature of Riak and will eventually be removed.")]
    public class RiakFluentLinkPhase
    {
        private readonly RiakLinkPhase phase;

        internal RiakFluentLinkPhase(RiakLinkPhase phase)
        {
            this.phase = phase;
        }

        /// <summary>
        /// The option to keep the results of this phase, or just pass them onto the next phase.
        /// </summary>
        /// <param name="keep"><b>true</b> to keep the phase results for the final result set, <b>false</b> to omit them. </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentLinkPhase Keep(bool keep)
        {
            phase.Keep(keep);
            return this;
        }

        /// <summary>
        /// Configure this Link phase to follow only links that go to a certain <paramref name="bucket"/>.
        /// </summary>
        /// <param name="bucket">The bucket to limit linkwalking to.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentLinkPhase Bucket(string bucket)
        {
            phase.Bucket(bucket);
            return this;
        }

        /// <summary>
        /// Configure this Link phase to follow only links with a certain <paramref name="tag"/>.
        /// </summary>
        /// <param name="tag">The tag to limit linkwalking to.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentLinkPhase Tag(string tag)
        {
            phase.Tag(tag);
            return this;
        }

        /// <summary>
        /// Configure this Link phase to follow only links with buckets and tags that match those in <paramref name="link"/>.
        /// </summary>
        /// <param name="link">
        /// The <see cref="RiakLink"/> containing a bucket and tag to limit linkwalking to.
        /// If the <see cref="RiakLink"/>'s Bucket, Key, and Tag are all null or whitespace, then this phase will follow all links.
        /// </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentLinkPhase FromRiakLink(RiakLink link)
        {
            phase.FromRiakLink(link);
            return this;
        }

        /// <summary>
        /// Configure this Link phase to follow all Links found on input objects.
        /// </summary>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentLinkPhase AllLinks()
        {
            phase.AllLinks();
            return this;
        }
    }
}
