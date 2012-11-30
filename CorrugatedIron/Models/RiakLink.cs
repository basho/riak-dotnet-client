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
using CorrugatedIron.Messages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace CorrugatedIron.Models
{
    public class RiakLink
    {
        public string Bucket { get; private set; }
        public string Key { get; private set; }
        public string Tag { get; private set; }

        public static readonly RiakLink AllLinks;

        static RiakLink()
        {
            AllLinks = new RiakLink("", "", "");
        }

        public RiakLink(string bucket, string key, string tag)
        {
            Bucket = bucket;
            Key = key;
            Tag = tag;
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

        private static RiakLink FromArray(IList<string> rawLink)
        {
            return new RiakLink(rawLink[0], rawLink[1], rawLink[2]);
        }

        internal RiakLink(RpbLink link)
        {
            Bucket = link.bucket.FromRiakString();
            Key = link.key.FromRiakString();
            Tag = link.tag.FromRiakString();
        }

        internal RpbLink ToMessage()
        {
            var message = new RpbLink
            {
                bucket = Bucket.ToRiakString(),
                key = Key.ToRiakString(),
                tag = Tag.ToRiakString()
            };

            return message;
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
            {
                return false;
            }
            if(ReferenceEquals(this, obj))
            {
                return true;
            }
            if(obj.GetType() != typeof(RiakLink))
            {
                return false;
            }
            return Equals((RiakLink)obj);
        }

        public bool Equals(RiakLink other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }
            if(ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.Bucket, Bucket) && Equals(other.Key, Key) && Equals(other.Tag, Tag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Bucket != null ? Bucket.GetHashCode() : 0);
                result = (result * 397) ^ (Key != null ? Key.GetHashCode() : 0);
                result = (result * 397) ^ (Tag != null ? Tag.GetHashCode() : 0);
                return result;
            }
        }
    }
}