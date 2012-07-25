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
using ProtoBuf;

namespace CorrugatedIron.Models
{
    public delegate string SerializeObjectToString<in T>(T theObject);

    public delegate byte[] SerializeObjectToByteArray<in T>(T theObject);

    public delegate T DeserializeObject<out T>(byte[] theObject, string contentType);

    public delegate T ResolveConflict<T>(List<T> conflictedObjects);

    public class RiakObject
    {
        private List<string> _vtags;
        private readonly int _hashCode;

        public string Bucket { get; private set; }
        public string Key { get; private set; }
        public byte[] Value { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentType { get; set; }
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

                foreach(var item in value)
                {
                    if(item.Key.IndexOf(RiakConstants.IndexSuffix.Integer) > -1)
                    {
                        IntIndexes.Add(item.Key, Convert.ToInt32(item.Value));
                    }
                    else
                    {
                        BinIndexes.Add(item.Key, item.Value);
                    }
                }
            }
        }

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

        public void AddIndex(string index, string key)
        {
            BinIndexes.Add(index.ToBinaryKey(), key);
        }

        public void AddIndex(string index, int key)
        {
            IntIndexes.Add(index.ToIntegerKey(), key);
        }

        public void RemoveBinIndex(string index)
        {
            IntIndexes.Remove(index.ToBinaryKey());
        }

        public void RemoveIntIndex(string index)
        {
            IntIndexes.Remove(index.ToIntegerKey());
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

        internal RiakObject(string bucket, string key, RpbContent content, byte[] vectorClock)
        {
            Bucket = bucket;
            Key = key;
            VectorClock = vectorClock;

            Value = content.value;
            VTag = content.vtag.FromRiakString();
            ContentEncoding = content.content_encoding.FromRiakString();
            ContentType = content.content_type.FromRiakString();
            UserMetaData = content.usermeta.ToDictionary(p => p.key.FromRiakString(), p => p.value.FromRiakString());
            Indexes = content.indexes.ToDictionary(p => p.key.FromRiakString(), p => p.value.FromRiakString());
            Links = content.links.Select(l => new RiakLink(l)).ToList();
            Siblings = new List<RiakObject>();
            LastModified = content.last_mod;
            LastModifiedUsec = content.last_mod_usecs;

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
            var message = new RpbPutReq
            {
                bucket = Bucket.ToRiakString(),
                key = Key.ToRiakString(),
                vclock = VectorClock,
                content = new RpbContent
                {
                    content_type = ContentType.ToRiakString(),
                    value = Value,
                    vtag = VTag.ToRiakString()
                }
            };

            message.content.usermeta.AddRange(UserMetaData.Select(kv => new RpbPair { key = kv.Key.ToRiakString(), value = kv.Value.ToRiakString() }));
            message.content.indexes.AddRange(Indexes.Select(kv => new RpbPair { key = kv.Key.ToRiakString(), value = kv.Value.ToRiakString() }));
            message.content.links.AddRange(Links.Select(l => l.ToMessage()));
            
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

        public void SetObject<T>(T value, string contentType, SerializeObjectToString<T> serializeObject)
            where T : class 
        {
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentException("contentType must be a valid MIME type");
            }

            if (serializeObject == null)
            {
                throw new ArgumentException("serializeObject cannot be null");
            }
            
            ContentType = contentType;

            Value = serializeObject(value).ToRiakString();
        }

        public void SetObject<T>(T value, string contentType, SerializeObjectToByteArray<T> serializeObject)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentException("contentType must be a valid MIME type");
            }

            if (serializeObject == null)
            {
                throw new ArgumentException("serializeObject cannot be null");
            }

            ContentType = contentType;

            Value = serializeObject(value);
        }

        // setting content type of SetObject changes content type
        public void SetObject<T>(T value, string contentType = null)
            where T : class
        {
            if(!string.IsNullOrEmpty(contentType))
            {
                ContentType = contentType;
            }

            // check content type
            // save based on content type's deserialization method
            if(ContentType == RiakConstants.ContentTypes.ApplicationJson)
            {
                var sots = new SerializeObjectToString<T>(theObject => theObject.Serialize());
                SetObject(value, ContentType, sots);
                return;
            }

            if(ContentType == RiakConstants.ContentTypes.ProtocolBuffers)
            {
                var soba = new SerializeObjectToByteArray<T>(theObject =>  
                {
                    var ms = new MemoryStream();
                    Serializer.Serialize(ms, value);
                    return ms.ToArray();
                });
                SetObject(value, ContentType, soba);
                return;
            }

            if(ContentType == RiakConstants.ContentTypes.Xml)
            {
                var soba = new SerializeObjectToByteArray<T>(theObject =>
                {
                    var ms = new MemoryStream();
                    var serde = new XmlSerializer(typeof(T));
                    serde.Serialize(ms, value);
                    return ms.ToArray();
                });
                SetObject(value, ContentType, soba);
                return;
            }
            
            if(ContentType.StartsWith("text"))
            {
                Value = value.ToString().ToRiakString();
                return;
            }

            throw new NotSupportedException(string.Format("Your current ContentType ({0}), is not supported.", ContentType));
        }

        public T GetObject<T>(DeserializeObject<T> deserializeObject, ResolveConflict<T> resolveConflict = null)
        {
            if (deserializeObject == null)
            {
                throw new ArgumentException("deserializeObject must not be null");
            }

            if (Siblings.Count > 1 && resolveConflict != null)
            {
                var conflictedObjects = Siblings.Select(s => deserializeObject(s.Value, ContentType)).ToList();

                return resolveConflict(conflictedObjects);
            }

            return deserializeObject(Value, ContentType);
        }

        public T GetObject<T>()
        {
            if(ContentType == RiakConstants.ContentTypes.ApplicationJson)
            {
                var deserializeObject = new DeserializeObject<T>((value, contentType) => JsonConvert.DeserializeObject<T>(Value.FromRiakString()));
                return GetObject(deserializeObject, null);
            }

            if(ContentType == RiakConstants.ContentTypes.ProtocolBuffers)
            {
                var deserializeObject = new DeserializeObject<T>((value, contentType) =>
                                                                     {
                                                                         var ms = new MemoryStream();
                                                                         ms.Write(value, 0, Value.Length);
                                                                         return Serializer.Deserialize<T>(ms);
                                                                     });
                return GetObject(deserializeObject, null);
            }

            if(ContentType == RiakConstants.ContentTypes.Xml)
            {
                var deserializeObject = new DeserializeObject<T>((value, contenType) =>
                                                                     {
                                                                         var r = XmlReader.Create(Value.FromRiakString());
                                                                         var serde = new XmlSerializer(typeof (T));
                                                                         return (T) serde.Deserialize(r);
                                                                     }
                    );
                return GetObject<T>(deserializeObject, null);
            }

            throw new NotSupportedException(string.Format("Your current ContentType ({0}), is not supported.", ContentType));
        }
    }
}