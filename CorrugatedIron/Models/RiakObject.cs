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

using System;
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
        public byte[] Value { get; set; }
        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }
        public string CharSet { get; set; }
        public byte[] VectorClock { get; private set; }
        public string VTag { get; private set; }
        public IDictionary<string, string> UserMetaData { get; set; }
        public uint LastModified { get; internal set; }
        public uint LastModifiedUsec { get; internal set; }
        public IList<RiakLink> Links { get; private set; }
        public IList<RiakObject> Siblings { get; set; }

        private List<string> _vtags;
        private int _hashCode;

        public bool HasChanged
        {
            get { return _hashCode != CalculateHashCode(); }
        }

        public List<string> VTags
        {
            get
            {
                return _vtags ?? (_vtags = Siblings.Count == 0 ? new List<string> {VTag} : Siblings.Select(s => s.VTag).ToList());
            }
        }

        public RiakObject(string bucket, string key)
            : this(bucket, key, null, RiakConstants.Defaults.ContentType)
        {
        }

        public RiakObject(string bucket, string key, string value)
            : this(bucket, key, value, RiakConstants.Defaults.ContentType)
        {
        }

        public RiakObject(string bucket, string key, string value, string contentType)
            : this(bucket, key, value, contentType, RiakConstants.Defaults.CharSet)
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

        public RiakObjectId ToRiakObjectId()
        {
            return new RiakObjectId(Bucket, Key);
        }

        internal RiakObject(string bucket, string key, RpbContent content, byte[] vectorClock)
        {
            Bucket = bucket;
            Key = key;
            VectorClock = vectorClock;

            Value = content.Value;
            VTag = content.VTag.FromRiakString();
            ContentEncoding = content.ContentEncoding.FromRiakString();
            ContentType = content.ContentType.FromRiakString();
            UserMetaData = content.UserMeta.ToDictionary(p => p.Key.FromRiakString(), p => p.Value.FromRiakString());
            Links = content.Links.Select(l => new RiakLink(l)).ToList();
            Siblings = new List<RiakObject>();
            LastModified = content.LastMod;
            LastModifiedUsec = content.LastModUSecs;

            _hashCode = CalculateHashCode();
        }

        internal RiakObject(string bucket, string key, ICollection<RpbContent> contents, byte[] vectorClock)
            : this(bucket, key, contents.First(), vectorClock)
        {
            if (contents.Count > 1)
            {
                Siblings = contents.Select(c => new RiakObject(bucket, key, c, vectorClock)).ToList();
                _hashCode = CalculateHashCode();
            }
        }

        internal RpbPutReq ToMessage()
        {
            UpdateLastModified();
            var message = new RpbPutReq
            {
                Bucket = Bucket.ToRiakString(),
                Key = Key.ToRiakString(),
                VectorClock = VectorClock,
                Content = new RpbContent
                {
                    ContentType = ContentType.ToRiakString(),
                    Value = Value,
                    VTag = VTag.ToRiakString(),
                    UserMeta = UserMetaData.Select(kv => new RpbPair { Key = kv.Key.ToRiakString(), Value = kv.Value.ToRiakString() }).ToList(),
                    LastMod = LastModified,
                    LastModUSecs = LastModifiedUsec,
                    Links = Links.Select(l => l.ToMessage()).ToList()
                }
            };

            return message;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RiakObject)) return false;
            return Equals((RiakObject) obj);
        }

        private void UpdateLastModified()
        {
            if (HasChanged)
            {
                var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                var ms = (ulong)Math.Round(t.TotalMilliseconds);

                LastModified = (uint)(ms / 1000u);
                LastModifiedUsec = (uint)((ms - LastModified * 1000u) * 100u);

                // TODO: figure out if we should be doing this or not, or should we isntead
                // be exposing a "mark as clean" function which does this instead.
                //_hashCode = CalculateHashCode();
            }
        }

        public bool Equals(RiakObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Bucket, Bucket)
                && Equals(other.Key, Key)
                && Equals(other.Value, Value)
                && Equals(other.ContentType, ContentType)
                && Equals(other.ContentEncoding, ContentEncoding)
                && Equals(other.CharSet, CharSet)
                && Equals(other.VectorClock, VectorClock)
                && Equals(other.UserMetaData, UserMetaData)
                && other.LastModified == LastModified
                && other.LastModifiedUsec == LastModifiedUsec
                && Equals(other.Links, Links)
                && Equals(other._vtags, _vtags)
                && other.Links.SequenceEqual(Links)
                && other.UserMetaData.SequenceEqual(UserMetaData);
        }

        public override int GetHashCode()
        {
            return CalculateHashCode();
        }

        /// <summary>
        /// This was moved into its own function that isn't virtual so that it could
        /// be called inside the object's constructor.
        /// </summary>
        /// <returns>The Object's hash code.</returns>
        private int CalculateHashCode()
        {
            unchecked
            {
                var result = (Bucket != null ? Bucket.GetHashCode() : 0);
                result = (result*397) ^ (Key != null ? Key.GetHashCode() : 0);
                result = (result*397) ^ (Value != null ? Value.GetHashCode() : 0);
                result = (result*397) ^ (ContentType != null ? ContentType.GetHashCode() : 0);
                result = (result*397) ^ (ContentEncoding != null ? ContentEncoding.GetHashCode() : 0);
                result = (result*397) ^ (CharSet != null ? CharSet.GetHashCode() : 0);
                result = (result*397) ^ (VectorClock != null ? VectorClock.GetHashCode() : 0);
                result = (result*397) ^ (UserMetaData != null ? UserMetaData.GetHashCode() : 0);
                result = (result*397) ^ LastModified.GetHashCode();
                result = (result*397) ^ LastModifiedUsec.GetHashCode();
                result = (result*397) ^ (Links != null ? Links.GetHashCode() : 0);
                result = (result*397) ^ (_vtags != null ? _vtags.GetHashCode() : 0);
                return result;
            }
        }

        public void SetValue<T>(T value)
            where T : class
        {
            var json = value.Serialize();
            Value = json.ToRiakString();
            ContentType = RiakConstants.ContentTypes.ApplicationJson;
        }

        public T GetValue<T>()
        {
            if (ContentType != RiakConstants.ContentTypes.ApplicationJson)
            {
                throw new InvalidOperationException("Unable to convert Riak Object value to type '{0}'. Content type required: '{1}'. Actual type: '{2}'".Fmt(typeof(T).Name, RiakConstants.ContentTypes.ApplicationJson, ContentType));
            }

            return Value.As<T>();
        }
    }
}
