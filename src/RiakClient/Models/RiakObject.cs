// <copyright file="RiakObject.cs" company="Basho Technologies, Inc.">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using Extensions;
    using Messages;
    using Models.Index;
    using Newtonsoft.Json;
    using ProtoBuf;
    using Util;

    public delegate string SerializeObjectToString<in T>(T theObject);

    public delegate byte[] SerializeObjectToByteArray<in T>(T theObject);

    public delegate T DeserializeObject<out T>(byte[] theObject, string contentType = null);

    public delegate T ResolveConflict<T>(List<T> conflictedObjects);

    public class RiakObject : IWriteableVClock
    {
        private readonly int hashCode;
        private readonly Dictionary<string, IntIndex> intIndexes;
        private readonly Dictionary<string, BinIndex> binIndexes;

        private List<string> vtags;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">Object bucket name</param>
        /// <param name="key">Object key</param>
        /// <remarks>When saving a binary object to Riak, one of the appropriate binary 
        /// <see cref="RiakConstants.ContentTypes"/> should be used.
        /// If the content type is not know, fall back to application/octet-stream. In addition,
        /// when saving binary data to Riak, a charSet of null/empty string should be used. The 
        /// constant CharSets.Binary should be used.</remarks>
        public RiakObject(string bucket, string key)
            : this(bucket, key, null, RiakConstants.Defaults.ContentType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">Object bucket name</param>
        /// <param name="key">Object key</param>
        /// <param name="value">Object value</param>
        /// <remarks>When saving a binary object to Riak, one of the appropriate binary 
        /// <see cref="RiakConstants.ContentTypes"/> should be used.
        /// If the content type is not know, fall back to application/octet-stream. In addition,
        /// when saving binary data to Riak, a charSet of null/empty string should be used. The 
        /// constant CharSets.Binary should be used.</remarks>
        public RiakObject(string bucket, string key, string value)
            : this(bucket, key, value, RiakConstants.Defaults.ContentType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">Object bucket name</param>
        /// <param name="key">Object key</param>
        /// <param name="value">Object value</param>
        /// <remarks>When saving a binary object to Riak, one of the appropriate binary 
        /// <see cref="RiakConstants.ContentTypes"/> should be used.
        /// If the content type is not know, fall back to application/octet-stream. In addition,
        /// when saving binary data to Riak, a charSet of null/empty string should be used. The 
        /// constant CharSets.Binary should be used.</remarks>
        public RiakObject(string bucket, string key, object value)
            : this(new RiakObjectId(bucket, key), value.ToJson(), RiakConstants.ContentTypes.ApplicationJson)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="objectId">Object ID</param>
        /// <param name="value">Object value</param>
        public RiakObject(RiakObjectId objectId, object value)
            : this(objectId, value.ToJson(), RiakConstants.ContentTypes.ApplicationJson)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">Object bucket name</param>
        /// <param name="key">Object key</param>
        /// <param name="value">Object value</param>
        /// <param name="contentType">Content type of the object. These should be MIME compliant content types.</param>
        /// <remarks>When saving a binary object to Riak, one of the appropriate binary 
        /// <see cref="RiakConstants.ContentTypes"/> should be used.
        /// If the content type is not know, fall back to application/octet-stream. In addition,
        /// when saving binary data to Riak, a charSet of null/empty string should be used. The 
        /// constant CharSets.Binary should be used.</remarks>
        public RiakObject(string bucket, string key, string value, string contentType)
            : this(new RiakObjectId(bucket, key), value, contentType, RiakConstants.Defaults.CharSet)
        {
        }

        public RiakObject(RiakObjectId objectId, string value, string contentType)
            : this(objectId, value, contentType, RiakConstants.Defaults.CharSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">Object bucket name</param>
        /// <param name="key">Object key</param>
        /// <param name="value">Object value</param>
        /// <param name="contentType">Content type of the object. These should be MIME compliant content types.</param>
        /// <param name="charSet">Character set used to encode saved data.</param>
        /// <remarks>When saving a binary object to Riak, one of the appropriate binary 
        /// <see cref="RiakConstants.ContentTypes"/> should be used.
        /// If the content type is not know, fall back to application/octet-stream. In addition,
        /// when saving binary data to Riak, a charSet of null/empty string should be used. The 
        /// constant CharSets.Binary should be used.</remarks>
        public RiakObject(string bucket, string key, string value, string contentType, string charSet)
            : this(new RiakObjectId(bucket, key), value.ToRiakString(), contentType, charSet)
        {
        }

        public RiakObject(RiakObjectId objectId, string value, string contentType, string charSet)
            : this(objectId, value.ToRiakString(), contentType, charSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">Object bucket name</param>
        /// <param name="key">Object key</param>
        /// <param name="value">Object value</param>
        /// <param name="contentType">Content type of the object. These should be MIME compliant content types.</param>
        /// <param name="charSet">Character set used to encode saved data.</param>
        /// <remarks>When saving a binary object to Riak, one of the appropriate binary 
        /// <see cref="RiakConstants.ContentTypes"/> should be used.
        /// If the content type is not know, fall back to application/octet-stream. In addition,
        /// when saving binary data to Riak, a charSet of null/empty string should be used. The 
        /// constant CharSets.Binary should be used.</remarks>
        public RiakObject(string bucket, string key, byte[] value, string contentType, string charSet)
            : this(null, bucket, key, value, contentType, charSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucketType">Riak bucket type - a collection of buckets with similar configuraiton</param> 
        /// <param name="bucket">Object bucket name</param>
        /// <param name="key">Object key</param>
        /// <param name="value">Object value</param>
        /// <param name="contentType">Content type of the object. These should be MIME compliant content types.</param>
        /// <param name="charSet">Character set used to encode saved data.</param>
        /// <remarks>When saving a binary object to Riak, one of the appropriate binary 
        /// <see cref="RiakConstants.ContentTypes"/> should be used.
        /// If the content type is not know, fall back to application/octet-stream. In addition,
        /// when saving binary data to Riak, a charSet of null/empty string should be used. The 
        /// constant CharSets.Binary should be used.</remarks>
        public RiakObject(string bucketType, string bucket, string key, byte[] value, string contentType, string charSet)
            : this(new RiakObjectId(bucketType, bucket, key), value, contentType, charSet)
        {
        }

        public RiakObject(RiakObjectId objectId, byte[] value, string contentType, string charSet)
        {
            if (objectId == null)
            {
                throw new ArgumentNullException("objectId");
            }

            // NB: BucketType is *not* required due to legacy bucket.
            BucketType = objectId.BucketType;

            Bucket = objectId.Bucket;
            if (string.IsNullOrWhiteSpace(Bucket))
            {
                throw new ArgumentNullException("objectId.Bucket");
            }

            // TODO: FUTURE - should Key be required?
            Key = objectId.Key;

            Value = value;
            ContentType = contentType;
            CharSet = charSet;
            UserMetaData = new Dictionary<string, string>();
            Links = new List<RiakLink>();
            Siblings = new List<RiakObject>();

            intIndexes = new Dictionary<string, IntIndex>();
            binIndexes = new Dictionary<string, BinIndex>();
        }

        internal RiakObject(string bucketType, string bucket, string key, RpbContent content, byte[] vectorClock)
        {
            BucketType = bucketType;
            Bucket = bucket;
            Key = key;
            VectorClock = vectorClock;

            Value = content.value;
            VTag = content.vtag.FromRiakString();
            ContentEncoding = content.content_encoding.FromRiakString();
            ContentType = content.content_type.FromRiakString();
            UserMetaData = content.usermeta.ToDictionary(p => p.key.FromRiakString(), p => p.value.FromRiakString());
            Links = content.links.Select(l => new RiakLink(l)).ToList();
            Siblings = new List<RiakObject>();

            // TODO - FUTURE: look at how other clients use this data
            LastModified = content.last_mod;
            LastModifiedUsec = content.last_mod_usecs;

            intIndexes = new Dictionary<string, IntIndex>();
            binIndexes = new Dictionary<string, BinIndex>();

            foreach (var index in content.indexes)
            {
                var name = index.key.FromRiakString();

                if (name.EndsWith(RiakConstants.IndexSuffix.Integer))
                {
                    IntIndex(name.Remove(name.Length - RiakConstants.IndexSuffix.Integer.Length))
                        .Add(index.value.FromRiakString());
                }
                else
                {
                    BinIndex(name.Remove(name.Length - RiakConstants.IndexSuffix.Binary.Length))
                        .Add(index.value.FromRiakString());
                }
            }

            hashCode = CalculateHashCode();
        }

        internal RiakObject(string bucketType, string bucket, string key, ICollection<RpbContent> contents, byte[] vectorClock)
            : this(bucketType, bucket, key, contents.First(), vectorClock)
        {
            if (contents.Count > 1)
            {
                Siblings = contents.Select(c => new RiakObject(bucketType, bucket, key, c, vectorClock)).ToList();
                hashCode = CalculateHashCode();
            }
        }

        public string BucketType { get; private set; }

        public string Bucket { get; private set; }

        public string Key { get; private set; }

        public byte[] Value { get; set; }

        public string ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public string CharSet { get; set; }

        public byte[] VectorClock { get; private set; }

        public string VTag { get; private set; }

        public IDictionary<string, string> UserMetaData { get; set; }

        public long LastModified { get; internal set; }

        public long LastModifiedUsec { get; internal set; }

        public IList<RiakLink> Links { get; private set; }

        public IList<RiakObject> Siblings { get; set; }

        public IDictionary<string, IntIndex> IntIndexes
        {
            get { return intIndexes; }
        }

        public IDictionary<string, BinIndex> BinIndexes
        {
            get { return binIndexes; }
        }

        public bool HasChanged
        {
            get { return hashCode != CalculateHashCode(); }
        }

        public List<string> VTags
        {
            // TODO: this is too complex
            get { return vtags ?? (vtags = Siblings.Count == 0 ? new List<string> { VTag } : Siblings.Select(s => s.VTag).ToList()); }
        }

        public IntIndex IntIndex(string name)
        {
            var index = default(IntIndex);
            name = name.ToLower();

            if (!intIndexes.TryGetValue(name, out index))
            {
                intIndexes[name] = index = new IntIndex(this, name);
            }

            return index;
        }

        public BinIndex BinIndex(string name)
        {
            var index = default(BinIndex);
            name = name.ToLower();

            if (!binIndexes.TryGetValue(name, out index))
            {
                binIndexes[name] = index = new BinIndex(this, name);
            }

            return index;
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
            foreach (RiakLink linkToRemove in linksToRemove)
            {
                Links.Remove(linkToRemove);
            }
        }

        public RiakObjectId ToRiakObjectId()
        {
            return new RiakObjectId(BucketType, Bucket, Key);
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

            if (obj.GetType() != typeof(RiakObject))
            {
                return false;
            }

            return Equals((RiakObject)obj);
        }

        public bool Equals(RiakObject other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.BucketType, Bucket)
                && Equals(other.Bucket, Bucket)
                && Equals(other.Key, Key)
                && Equals(other.Value, Value)
                && Equals(other.ContentType, ContentType)
                && Equals(other.ContentEncoding, ContentEncoding)
                && Equals(other.CharSet, CharSet)
                && Equals(other.VectorClock, VectorClock)
                && Equals(other.UserMetaData, UserMetaData)
                && Equals(other.BinIndexes, BinIndexes)
                && Equals(other.IntIndexes, IntIndexes)
                && other.LastModified == LastModified
                && other.LastModifiedUsec == LastModifiedUsec
                && Equals(other.Links, Links)
                && Equals(other.vtags, vtags)
                && other.Links.SequenceEqual(Links)
                && other.UserMetaData.SequenceEqual(UserMetaData)
                && other.BinIndexes.SequenceEqual(BinIndexes)
                && other.IntIndexes.SequenceEqual(IntIndexes);
        }

        public override int GetHashCode()
        {
            return CalculateHashCode();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1033:InterfaceMethodsShouldBeCallableByChildTypes",
            Scope = "member",
            Target = "RiakClient.Models.RiakObject.#RiakClient.Models.RiakObject.SetVClock(byte[])",
            Justification = "Mucking with a VClock should require casting to IWriteableVClock. TODO - FUTURE: evaluate this decision")]
        void IWriteableVClock.SetVClock(byte[] vclock)
        {
            VectorClock = vclock;
        }

        public void SetObject<T>(T value, SerializeObjectToString<T> serializeObject)
            where T : class
        {
            if (serializeObject == null)
            {
                throw new ArgumentException("serializeObject cannot be null");
            }

            Value = serializeObject(value).ToRiakString();
        }

        public void SetObject<T>(T value, string contentType, SerializeObjectToString<T> serializeObject)
            where T : class
        {
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentException("contentType must be a valid MIME type");
            }

            ContentType = contentType;

            SetObject(value, serializeObject);
        }

        public void SetObject<T>(T value, SerializeObjectToByteArray<T> serializeObject)
        {
            if (serializeObject == null)
            {
                throw new ArgumentException("serializeObject cannot be null");
            }

            Value = serializeObject(value);
        }

        public void SetObject<T>(T value, string contentType, SerializeObjectToByteArray<T> serializeObject)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentException("contentType must be a valid MIME type");
            }

            ContentType = contentType;

            SetObject(value, serializeObject);
        }

        // setting content type of SetObject changes content type
        public void SetObject<T>(T value, string contentType = null)
            where T : class
        {
            if (!string.IsNullOrEmpty(contentType))
            {
                ContentType = contentType;
            }

            // check content type
            // save based on content type's deserialization method
            if (ContentType == RiakConstants.ContentTypes.ApplicationJson)
            {
                var sots = new SerializeObjectToString<T>(theObject => theObject.Serialize());
                SetObject(value, ContentType, sots);
                return;
            }

            if (ContentType == RiakConstants.ContentTypes.ProtocolBuffers)
            {
                var soba = new SerializeObjectToByteArray<T>(theObject =>
                {
                    using (var ms = new MemoryStream())
                    {
                        Serializer.Serialize(ms, value);
                        return ms.ToArray();
                    }
                });

                SetObject(value, ContentType, soba);

                return;
            }

            if (ContentType == RiakConstants.ContentTypes.Xml)
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

            if (ContentType.StartsWith("text"))
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
            if (ContentType == RiakConstants.ContentTypes.ApplicationJson)
            {
                var deserializeObject = new DeserializeObject<T>((value, contentType) => JsonConvert.DeserializeObject<T>(Value.FromRiakString()));
                return GetObject(deserializeObject);
            }

            if (ContentType == RiakConstants.ContentTypes.ProtocolBuffers)
            {
                var deserializeObject = new DeserializeObject<T>((value, contentType) =>
                {
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(value, 0, Value.Length);
                        ms.Position = 0;
                        return Serializer.Deserialize<T>(ms);
                    }
                });

                return GetObject(deserializeObject);
            }

            if (ContentType == RiakConstants.ContentTypes.Xml)
            {
                var deserializeObject = new DeserializeObject<T>((value, contentType) =>
                {
                    var r = XmlReader.Create(Value.FromRiakString());
                    var serde = new XmlSerializer(typeof(T));
                    return (T)serde.Deserialize(r);
                });

                return GetObject(deserializeObject);
            }

            throw new NotSupportedException(string.Format("Your current ContentType ({0}), is not supported.", ContentType));
        }

        internal RiakLink ToRiakLink(string tag)
        {
            return new RiakLink(Bucket, Key, tag);
        }

        internal RpbPutReq ToMessage()
        {
            var message = new RpbPutReq
            {
                type = BucketType.ToRiakString(),
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

            message.content.links.AddRange(Links.Select(l => l.ToMessage()));

            message.content.indexes.AddRange(IntIndexes.Values.SelectMany(i =>
                i.Values.Select(v =>
                    new RpbPair
                    {
                        key = i.RiakIndexName.ToRiakString(),
                        value = v.ToString().ToRiakString()
                    })));

            message.content.indexes.AddRange(BinIndexes.Values.SelectMany(i =>
                i.Values.Select(v =>
                    new RpbPair
                    {
                        key = i.RiakIndexName.ToRiakString(),
                        value = v.ToRiakString()
                    })));

            return message;
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
                int result = BucketType != null ? BucketType.GetHashCode() : 0;
                result = (result * 397) ^ (Bucket != null ? Bucket.GetHashCode() : 0);
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
                result = (result * 397) ^ (vtags != null ? vtags.GetHashCode() : 0);
                return result;
            }
        }
    }
}
