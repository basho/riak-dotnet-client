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

using System.Collections.Generic;
using System.Linq;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Util;

namespace CorrugatedIron.Models
{
    public class RiakObject
    {
        public string Bucket { get; private set; }
        public string Key { get; private set; }
        public byte[] Value { get; private set; }
        public string ContentType { get; private set; }
        public string ContentEncoding { get; private set; }
        public string CharSet { get; private set; }
        public string VectorClock { get; private set; }
        public string VTag { get; private set; }
        public IDictionary<string, string> UserMetaData { get; set; }
        public int LastModified { get; private set; }
        public int LastModifiedUsec { get; private set; }
        public IList<RiakLink> Links { get; private set; }
        public IList<RiakObject> Siblings { get; set; }

        private List<string> _vtags;

        public List<string> VTags
        {
            get
            {
                if (_vtags == null)
                {
                    _vtags = Siblings.Count == 0 ? new List<string> { VTag } : Siblings.Select(s => s.VTag).ToList();
                }
                return _vtags;
            }
        }

        public RiakObject(string bucket, string key, string value)
            : this(bucket, key, value, Constants.Defaults.ContentType)
        {
        }

        public RiakObject(string bucket, string key, string value, string contentType)
            : this(bucket, key, value, contentType, Constants.Defaults.CharSet)
        {
        }

        public RiakObject(string bucket, string key, string value, string contentType, string charSet)
            : this(bucket, key, value.ToRiakString(), contentType, charSet)
        {
        }

        public RiakObject(string bucket, string key, byte[] value, string contentType, string charSet)
        {
            Bucket = bucket;
            Key = key;
            Value = value;
            ContentType = contentType;
            CharSet = charSet;
            UserMetaData = new Dictionary<string, string>();
            Links = new List<RiakLink>();
            Siblings = new List<RiakObject>();
        }

        public void LinkTo(string bucket, string key, string tag)
        {
            Links.Add(new RiakLink(bucket, key, tag));
        }

        public void LinkTo(RiakObjectId riakObjectId, string tag)
        {
            Links.Add(riakObjectId.ToRiakLink(tag));
        }

        public void LinkTo(RiakObject riakObject, string tag)
        {
            Links.Add(riakObject.ToRiakLink(tag));
        }

        public void RemoveLink(string bucket, string key, string tag)
        {
            var link = new RiakLink(bucket, key, tag);
            RemoveLink(link);
        }

        public void RemoveLink(RiakObjectId riakObjectId, string tag)
        {
            var link = new RiakLink(riakObjectId.Bucket, riakObjectId.Key, tag);
            RemoveLink(link);
        }

        public void RemoveLink(RiakObject riakObject, string tag)
        {
            var link = new RiakLink(riakObject.Bucket, riakObject.Key, tag);
            RemoveLink(link);
        }

        public void RemoveLink(RiakLink link)
        {
            Links.Remove(link);
        }

        public void RemoveLinks(RiakObject riakObject)
        {
            RemoveLinks(new RiakObjectId(riakObject.Bucket, riakObject.Key));
        }

        public void RemoveLinks(RiakObjectId riakObjectId)
        {
            var linksToRemove = from link in Links
                                where link.Bucket == riakObjectId.Bucket
                                      && link.Key == riakObjectId.Key
                                select link;

            linksToRemove.ForEach(l => Links.Remove(l));
        }
        
        internal RiakLink ToRiakLink(string tag)
        {
            return new RiakLink(Bucket, Key, tag);
        }

        internal RiakObject(string bucket, string key, RpbContent content, byte[] vectorClock)
        {
            Bucket = bucket;
            Key = key;
            VectorClock = vectorClock.FromRiakString();

            Value = content.Value;
            VTag = content.VTag.FromRiakString();
            ContentEncoding = content.ContentEncoding.FromRiakString();
            ContentType = content.ContentType.FromRiakString();
            UserMetaData = content.UserMeta.ToDictionary(p => p.Key.FromRiakString(), p => p.Value.FromRiakString());
            Links = content.Links.Select(l => new RiakLink(l)).ToList();
            Siblings = new List<RiakObject>();
        }

        internal RpbPutReq ToMessage()
        {
            var message = new RpbPutReq
            {
                Bucket = Bucket.ToRiakString(),
                Key = Key.ToRiakString(),
                VectorClock = VectorClock.ToRiakString(),
                Content = new RpbContent
                {
                    ContentType = ContentType.ToRiakString(),
                    Value = Value,
                    VTag = VTag.ToRiakString(),
                    UserMeta = UserMetaData.Select(kv => new RpbPair { Key = kv.Key.ToRiakString(), Value = kv.Value.ToRiakString() }).ToList(),
                    Links = Links.Select(l => l.ToMessage()).ToList()
                }
            };

            return message;
        }
    }
}
