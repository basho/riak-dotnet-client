// <copyright file="RiakLink.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Messages;
    using Newtonsoft.Json;

    public class RiakLink
    {
        public static readonly RiakLink AllLinks = new RiakLink(string.Empty, string.Empty, string.Empty);

        private readonly string bucket;
        private readonly string key;
        private readonly string tag;

        public RiakLink(string bucket, string key, string tag)
        {
            this.bucket = bucket;
            this.key = key;
            this.tag = tag;
        }

        internal RiakLink(RpbLink link)
        {
            this.bucket = link.bucket.FromRiakString();
            this.key = link.key.FromRiakString();
            this.tag = link.tag.FromRiakString();
        }

        public string Bucket
        {
            get { return bucket; }
        }

        public string Key
        {
            get { return key; }
        }

        public string Tag
        {
            get { return tag; }
        }

        public static RiakLink FromJsonString(string jsonString)
        {
            var rawLink = JsonConvert.DeserializeObject<string[]>(jsonString);

            return new RiakLink(rawLink[0], rawLink[1], rawLink[2]);
        }

        public static IList<RiakLink> ParseArrayFromJsonString(string jsonString)
        {
            // FIXME HAX!
            jsonString = jsonString.Replace("]][[", "],[");
            var rawLinks = JsonConvert.DeserializeObject<IList<IList<string>>>(jsonString);

            return rawLinks.Select(FromArray).ToList();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as RiakLink);
        }

        public bool Equals(RiakLink other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.Bucket, bucket) && Equals(other.Key, key) && Equals(other.Tag, tag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = bucket != null ? bucket.GetHashCode() : 0;
                result = (result * 397) ^ (key != null ? key.GetHashCode() : 0);
                result = (result * 397) ^ (tag != null ? tag.GetHashCode() : 0);
                return result;
            }
        }

        internal RpbLink ToMessage()
        {
            var message = new RpbLink
            {
                bucket = bucket.ToRiakString(),
                key = key.ToRiakString(),
                tag = tag.ToRiakString()
            };

            return message;
        }

        private static RiakLink FromArray(IList<string> rawLink)
        {
            return new RiakLink(rawLink[0], rawLink[1], rawLink[2]);
        }
    }
}
