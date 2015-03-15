// <copyright file="RiakSearchResultDocument.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.Search
{
    using System.Collections.Generic;
    using System.Linq;
    using Messages;

    /// <summary>
    /// Represents a single Riak Search result document.
    /// </summary>
    public class RiakSearchResultDocument
    {
        private RiakObjectId riakObjectId = null;

        internal RiakSearchResultDocument(RpbSearchDoc doc)
        {
            string legacyId = null;
            Fields = new List<RiakSearchResultField>();

            foreach (var field in doc.fields.Select(f => new RiakSearchResultField(f)))
            {
                switch (field.Key)
                {
                    case RiakConstants.SearchFieldKeys.Id:
                        Id = field.Value;
                        break;
                    case RiakConstants.SearchFieldKeys.Score:
                        Score = field.Value;
                        break;
                    case RiakConstants.SearchFieldKeys.BucketType:
                        BucketType = field.Value;
                        break;
                    case RiakConstants.SearchFieldKeys.Bucket:
                        Bucket = field.Value;
                        break;
                    case RiakConstants.SearchFieldKeys.Key:
                        Key = field.Value;
                        break;
#pragma warning disable 618
                    case RiakConstants.SearchFieldKeys.LegacySearchId:
                        legacyId = field.Value;
                        break;
#pragma warning restore 618
                }

                Fields.Add(field);
            }

            if (CanUseLegacyId(legacyId))
            {
                Id = legacyId;
            }
        }

        /// <summary>
        /// The Id field of the Lucene document.
        /// </summary>
        /// <remarks>You will also find this value in the <see cref="Fields"/> collection.</remarks>
        public string Id { get; private set; }

        /// <summary>
        /// The document match score field.
        /// </summary>
        /// <remarks>You will also find this value in the <see cref="Fields"/> collection.</remarks>
        public string Score { get; private set; }

        /// <summary>
        /// The Bucket Type field. Contains the Bucket Type of the matching riak object.
        /// </summary>
        /// <remarks>You will also find this value in the <see cref="Fields"/> collection.</remarks>
        public string BucketType { get; private set; }

        /// <summary>
        /// The Bucket Name field. Contains the Bucket Name of the matching riak object.
        /// </summary>
        /// <remarks>You will also find this value in the <see cref="Fields"/> collection.</remarks>
        public string Bucket { get; private set; }

        /// <summary>
        /// The Key field. Contains the Key of the matching riak object.
        /// </summary>
        /// <remarks>You will also find this value in the <see cref="Fields"/> collection.</remarks>
        public string Key { get; private set; }

        /// <summary>
        /// The <see cref="RiakObjectId"/> of the matching riak object.
        /// </summary>
        public RiakObjectId RiakObjectId
        {
            get
            {
                if (riakObjectId == null)
                {
                    riakObjectId = new RiakObjectId(BucketType, Bucket, Key);
                }

                return riakObjectId;
            }
        }

        /// <summary>
        /// The collection of <see cref="RiakSearchResultField"/>s returned by the search. 
        /// </summary>
        public List<RiakSearchResultField> Fields { get; private set; }

        private bool CanUseLegacyId(string legacyId)
        {
            return Id == null && legacyId != null;
        }
    }
}
