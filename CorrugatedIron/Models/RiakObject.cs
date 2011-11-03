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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Util;
using Newtonsoft.Json;

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
        public IDictionary<string, string> BinIndexes { get; set; }
        public IDictionary<string, int> IntIndexes { get; set; }

        public IDictionary<string, string> Indexes
        {
            get
            {
                var indexes = BinIndexes.Keys.ToDictionary(key => key, key => BinIndexes[key]);

                foreach(var key in IntIndexes.Keys)
                {
                    indexes.Add(key, IntIndexes[key].ToString());
                }

                return indexes;
            }

            private set
            {
                if(BinIndexes == null)
                {
                    BinIndexes = new Dictionary<string, string>();
                }
                if(IntIndexes == null)
                {
                    IntIndexes = new Dictionary<string, int>();
                }

                foreach(var key in value.Keys)
                {
                    if(key.IndexOf("_int") > -1)
                    {
                        IntIndexes.Add(key, Convert.ToInt32(value[key]));
                    }
                    else
                    {
                        BinIndexes.Add(key, value[key].ToString());
                    }
                }
            }
        }

        private List<string> _vtags;
        private int _hashCode;

        public bool HasChanged
        {
            get { return _hashCode != CalculateHashCode(); }
        }

        public List<string> VTags
        {
            get { return _vtags ?? (_vtags = Siblings.Count == 0 ? new List<string> { VTag } : Siblings.Select(s => s.VTag).ToList()); }
        }

        public RiakObject(string bucket, string key)
            : this(bucket, key, null, RiakConstants.Defaults.ContentType)
        {
        }

        public RiakObject(string bucket, string key, string value)
            : this(bucket, key, value, RiakConstants.Defaults.ContentType)
        {
        }

        public RiakObject(string bucket, string key, object value)
            : this(bucket, key, value.ToJson(), RiakConstants.ContentTypes.ApplicationJson)
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
            Indexes = new Dictionary<string, string>();
            Links = new List<RiakLink>();
            Siblings = new List<RiakObject>();

            BinIndexes = new Dictionary<string, string>();
            IntIndexes = new Dictionary<string, int>();
        }

        public void AddBinIndex(string index, string key)
        {
            BinIndexes.Add(FormatBinKey(index), key);
        }

        public void AddIntIndex(string index, int key)
        {
            IntIndexes.Add(FormatIntKey(index), key);
        }

        public void RemoveBinIndex(string index)
        {
            BinIndexes.Remove(FormatBinKey(index));
        }

        public void RemoveIntIndex(string index)
        {
            IntIndexes.Remove(FormatIntKey(index));
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
            var linksToRemove = Links.Where(l => l.Bucket == riakObjectId.Bucket && l.Key == riakObjectId.Key);

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

        public void MarkClean()
        {
            _hashCode = CalculateHashCode();
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
            Indexes = content.Indexes.ToDictionary(p => p.Key.FromRiakString(), p => p.Value.FromRiakString());
            Links = content.Links.Select(l => new RiakLink(l)).ToList();
            Siblings = new List<RiakObject>();
            LastModified = content.LastMod;
            LastModifiedUsec = content.LastModUSecs;

            _hashCode = CalculateHashCode();
        }

        internal RiakObject(string bucket, string key, ICollection<RpbContent> contents, byte[] vectorClock)
            : this(bucket, key, contents.First(), vectorClock)
        {
            if(contents.Count > 1)
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
                    Indexes = Indexes.Select(kv => new RpbPair { Key = kv.Key.ToRiakString(), Value = kv.Value.ToRiakString() }).ToList(),
                    LastMod = LastModified,
                    LastModUSecs = LastModifiedUsec,
                    Links = Links.Select(l => l.ToMessage()).ToList()
                }
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
            if(obj.GetType() != typeof(RiakObject))
            {
                return false;
            }
            return Equals((RiakObject)obj);
        }

        private void UpdateLastModified()
        {
            if(HasChanged)
            {
                var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                var ms = (ulong)Math.Round(t.TotalMilliseconds);

                LastModified = (uint)(ms / 1000u);
                LastModifiedUsec = (uint)((ms - LastModified * 1000u) * 100u);
            }
        }

        public bool Equals(RiakObject other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }

            if(ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.Bucket, Bucket)
                && Equals(other.Key, Key)
                && Equals(other.Value, Value)
                && Equals(other.ContentType, ContentType)
                && Equals(other.ContentEncoding, ContentEncoding)
                && Equals(other.CharSet, CharSet)
                && Equals(other.VectorClock, VectorClock)
                && Equals(other.UserMetaData, UserMetaData)
                && Equals(other.Indexes, Indexes)
                && other.LastModified == LastModified
                && other.LastModifiedUsec == LastModifiedUsec
                && Equals(other.Links, Links)
                && Equals(other._vtags, _vtags)
                && other.Links.SequenceEqual(Links)
                && other.UserMetaData.SequenceEqual(UserMetaData)
                && other.Indexes.SequenceEqual(Indexes);
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
                result = (result * 397) ^ (Key != null ? Key.GetHashCode() : 0);
                result = (result * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                result = (result * 397) ^ (ContentType != null ? ContentType.GetHashCode() : 0);
                result = (result * 397) ^ (ContentEncoding != null ? ContentEncoding.GetHashCode() : 0);
                result = (result * 397) ^ (CharSet != null ? CharSet.GetHashCode() : 0);
                result = (result * 397) ^ (VectorClock != null ? VectorClock.GetHashCode() : 0);
                result = (result * 397) ^ (UserMetaData != null ? UserMetaData.GetHashCode() : 0);
                result = (result * 397) ^ (BinIndexes != null ? BinIndexes.GetHashCode() : 0);
                result = (result * 397) ^ (IntIndexes != null ? IntIndexes.GetHashCode() : 0);
                result = (result * 397) ^ LastModified.GetHashCode();
                result = (result * 397) ^ LastModifiedUsec.GetHashCode();
                result = (result * 397) ^ (Links != null ? Links.GetHashCode() : 0);
                result = (result * 397) ^ (_vtags != null ? _vtags.GetHashCode() : 0);
                return result;
            }
        }

        // setting content type of SetObject changes content type
        public void SetObject<T>(T value, string contentType = null)
            where T : class
        {
            if(!String.IsNullOrEmpty(contentType))
            {
                ContentType = contentType;
            }

            // check content type
            // save based on content type's deserialization method

            if(ContentType == RiakConstants.ContentTypes.ApplicationJson)
            {
                var json = value.Serialize();
                Value = json.ToRiakString();
                return;
            }

            if(ContentType == RiakConstants.ContentTypes.ProtocolBuffers)
            {
                var memoryStream = new MemoryStream();
                ProtoBuf.Serializer.Serialize(memoryStream, value);
                Value = memoryStream.ToArray();
                return;
            }

            if(ContentType == RiakConstants.ContentTypes.Xml)
            {
                var memoryStream = new MemoryStream();
                var serde = new XmlSerializer(typeof(T));
                serde.Serialize(memoryStream, value);
                Value = memoryStream.ToArray();
                return;
            }

            throw new NotSupportedException(string.Format("Your current ContentType ({0}), is not supported.", ContentType));
        }

        public T GetObject<T>()
        {
            if(ContentType == RiakConstants.ContentTypes.ApplicationJson)
            {
                return JsonConvert.DeserializeObject<T>(Value.FromRiakString());
            }

            if(ContentType == RiakConstants.ContentTypes.ProtocolBuffers)
            {
                var memoryStream = new MemoryStream();

                memoryStream.Write(Value, 0, Value.Length);
                return ProtoBuf.Serializer.Deserialize<T>(memoryStream);
            }

            if(ContentType == RiakConstants.ContentTypes.Xml)
            {
                var reader = XmlReader.Create(Value.FromRiakString());
                var serde = new XmlSerializer(typeof(T));

                return (T)serde.Deserialize(reader);
            }

            throw new NotSupportedException(string.Format("Your current ContentType ({0}), is not supported.", ContentType));
        }

        // TODO remove when we hit 0.3
        [Obsolete("<Marked obsolete in v0.1.2; will be removed in v0.3")]
        public dynamic GetObject()
        {
            return GetObject<dynamic>();
        }

        private string FormatBinKey(string key)
        {
            if(key.IndexOf("_bin") < 0)
            {
                key = "{0}_bin".Fmt(key);
            }

            return key;
        }

        private string FormatIntKey(string key)
        {
            if(key.IndexOf("_int") < 0)
            {
                key = "{0}_int".Fmt(key);
            }

            return key;
        }
    }
}