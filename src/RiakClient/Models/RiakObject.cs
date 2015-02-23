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
    using Exceptions;
    using Extensions;
    using Messages;
    using Models.Index;
    using Newtonsoft.Json;
    using ProtoBuf;

    /// <summary>
    /// A delegate to handle serialization of an object to a string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="theObject">The object to serialize.</param>
    /// <returns>A string containing the serialized object.</returns>
    public delegate string SerializeObjectToString<in T>(T theObject);

    /// <summary>
    /// A delegate to handle serialization of an object to a byte[].
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="theObject">The object to serialize.</param>
    /// <returns>A byte[] containing the serialized object.</returns>
    public delegate byte[] SerializeObjectToByteArray<in T>(T theObject);

    /// <summary>
    /// A delegate to handle deserialization of an byte[] serialized object to it's original type.
    /// </summary>
    /// <typeparam name="T">The destination type of the object.</typeparam>
    /// <param name="theObject">The serialized object.</param>
    /// <param name="contentType">Content type of the object.</param>
    /// <returns>The deserialized object.</returns>
    public delegate T DeserializeObject<out T>(byte[] theObject, string contentType = null);

    /// <summary>
    /// A delegate to handle resolution of sibling objects. 
    /// Takes all the sibling objects as input and returns one "resolved" object.
    /// </summary>
    /// <typeparam name="T">The type of the objects.</typeparam>
    /// <param name="conflictedObjects">The conflicting sibling objects.</param>
    /// <returns>A single resolved object.</returns>
    public delegate T ResolveConflict<T>(List<T> conflictedObjects);

    /// <summary>
    /// Contains all the information about a single object in Riak.
    /// </summary>
    public class RiakObject : IWriteableVClock
    {
        private readonly int hashCode;
        private readonly Dictionary<string, IntIndex> intIndexes;
        private readonly Dictionary<string, BinIndex> binIndexes;

        private List<string> vtags;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">The object's bucket name.</param>
        /// <param name="key">The object's key.</param>
        public RiakObject(string bucket, string key)
            : this(bucket, key, null, RiakConstants.Defaults.ContentType)
        {
        }

        // TODO: Fix?  String value & JSON content type

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">The object's bucket name.</param>
        /// <param name="key">The object's key.</param>
        /// <param name="value">The object's value.</param>
        /// <remarks>Uses the default (JSON) content-type.</remarks>
        public RiakObject(string bucket, string key, string value)
            : this(bucket, key, value, RiakConstants.Defaults.ContentType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">The object's bucket name.</param>
        /// <param name="key">The object's key.</param>
        /// <param name="value">The object's value.</param>
        /// <remarks>
        /// This overload expects that <paramref name="value"/> be serializable to JSON. 
        /// Uses the default (JSON) content-type.
        /// </remarks>
        public RiakObject(string bucket, string key, object value)
            : this(new RiakObjectId(bucket, key), value.ToJson(), RiakConstants.ContentTypes.ApplicationJson)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// Uses an "application/json" content type.
        /// </summary>
        /// <param name="objectId">The object's Id.</param>
        /// <param name="value">The object's value.</param>
        /// <remarks>Uses the default (JSON) content-type.</remarks>
        public RiakObject(RiakObjectId objectId, object value)
            : this(objectId, value.ToJson(), RiakConstants.ContentTypes.ApplicationJson)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">The object's bucket name.</param>
        /// <param name="key">The object's key.</param>
        /// <param name="value">The object's value.</param>
        /// <param name="contentType">
        /// The content-type of the object, must be a MIME compliant content-type.
        /// See <see cref="RiakConstants.ContentTypes"/> for common options.
        /// If the content type is not know, fall back to application/octet-stream.
        /// </param>
        public RiakObject(string bucket, string key, string value, string contentType)
            : this(new RiakObjectId(bucket, key), value, contentType, RiakConstants.Defaults.CharSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="objectId">The object's Id.</param>
        /// <param name="value">The object's value.</param>
        /// <param name="contentType">
        /// The content-type of the object, must be a MIME compliant content-type.
        /// See <see cref="RiakConstants.ContentTypes"/> for common options.
        /// If the content type is not know, fall back to application/octet-stream.
        /// </param>
        public RiakObject(RiakObjectId objectId, string value, string contentType)
            : this(objectId, value, contentType, RiakConstants.Defaults.CharSet)
        {
        }

        // TODO: Fix?  Takes charset parameter, but encodes with UTF8

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">The object's bucket name.</param>
        /// <param name="key">The object's key.</param>
        /// <param name="value">The object's value.</param>
        /// <param name="contentType">
        /// The content-type of the object, must be a MIME compliant content-type.
        /// See <see cref="RiakConstants.ContentTypes"/> for common options.
        /// If the content type is not know, fall back to application/octet-stream.
        /// </param>
        /// <param name="charSet">The character set used to encode saved data.</param>
        /// <remarks>
        /// When saving binary data to Riak, the constant 
        /// <see cref="RiakConstants.CharSets.Binary"/> should be used.
        /// </remarks>
        public RiakObject(string bucket, string key, string value, string contentType, string charSet)
            : this(new RiakObjectId(bucket, key), value.ToRiakString(), contentType, charSet)
        {
        }

        // TODO: Fix?  Takes charset parameter, but encodes with UTF8

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="objectId">The object's Id.</param>
        /// <param name="value">The object's value.</param>
        /// <param name="contentType">
        /// The content-type of the object, must be a MIME compliant content-type.
        /// See <see cref="RiakConstants.ContentTypes"/> for common options.
        /// If the content type is not know, fall back to application/octet-stream.
        /// </param>
        /// <param name="charSet">The character set used to encode saved data.</param>
        /// <exception cref="ArgumentNullException">
        /// The value of 'objectId', cannot be null. 
        /// </exception>
        public RiakObject(RiakObjectId objectId, string value, string contentType, string charSet)
            : this(objectId, value.ToRiakString(), contentType, charSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="bucket">The object's bucket name.</param>
        /// <param name="key">The object's key.</param>
        /// <param name="value">The object's value.</param>
        /// <param name="contentType">
        /// The content-type of the object, must be a MIME compliant content-type.
        /// See <see cref="RiakConstants.ContentTypes"/> for common options.
        /// If the content type is not know, fall back to application/octet-stream.
        /// </param>
        /// <param name="charSet">The character set used to encode saved data.</param>
        /// <remarks>
        /// When saving binary data to Riak, the constant 
        /// <see cref="RiakConstants.CharSets.Binary"/> should be used.
        /// </remarks>
        public RiakObject(string bucket, string key, byte[] value, string contentType, string charSet)
            : this(new RiakObjectId(bucket, key), value, contentType, charSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakObject"/> class.
        /// </summary>
        /// <param name="objectId">The object's Id.</param>
        /// <param name="value">The object's value.</param>
        /// <param name="contentType">
        /// The content-type of the object, must be a MIME compliant content-type.
        /// See <see cref="RiakConstants.ContentTypes"/> for common options.
        /// If the content type is not know, fall back to application/octet-stream.
        /// </param>
        /// <param name="charSet">The character set used to encode saved data.</param>
        /// <remarks>
        /// When saving binary data to Riak, the constant 
        /// <see cref="RiakConstants.CharSets.Binary"/> should be used.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The value of 'objectId', cannot be null. 
        /// </exception>
        public RiakObject(RiakObjectId objectId, byte[] value, string contentType, string charSet)
        {
            if (objectId == null)
            {
                throw new ArgumentNullException("objectId");
            }

            BucketType = objectId.BucketType;

            Bucket = objectId.Bucket;

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

        /// <summary>
        /// Get the Bucket Type (if any). 
        /// </summary>
        public string BucketType { get; private set; }

        /// <summary>
        /// Get the Bucket name.
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// Get the Key.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Get the raw value, as a byte[].
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// Get the ContentEncoding. 
        /// </summary>
        public string ContentEncoding { get; set; }

        /// <summary>
        /// Get the Content Type (MIME Type).
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Get the CharSet encoding, if set.
        /// </summary>
        public string CharSet { get; set; }

        /// <summary>
        /// Get the Vector Clock.
        /// </summary>
        public byte[] VectorClock { get; private set; }

        /// <summary>
        /// Get the VTag.
        /// </summary>
        public string VTag { get; private set; }

        /// <summary>
        /// Get any user set MetaData.
        /// </summary>
        public IDictionary<string, string> UserMetaData { get; set; }

        /// <summary>
        /// Get the Last Modified unix timestamp. 
        /// </summary>
        public long LastModified { get; internal set; }

        /// <summary>
        /// Get the Last Modified unix timestamp in microseconds.
        /// </summary>
        public long LastModifiedUsec { get; internal set; }

        /// <summary>
        /// Get the list of Links to other Riak objects. 
        /// </summary>
        public IList<RiakLink> Links { get; private set; }

        /// <summary>
        /// Get a list of conflicting Sibling objects. 
        /// </summary>
        public IList<RiakObject> Siblings { get; set; }

        /// <summary>
        /// Get the collection of Integer secondary indexes for this object.
        /// </summary>
        public IDictionary<string, IntIndex> IntIndexes
        {
            get { return intIndexes; }
        }

        /// <summary>
        /// Get the collection of string (binary) secondary indexes for this object.
        /// </summary>
        public IDictionary<string, BinIndex> BinIndexes
        {
            get { return binIndexes; }
        }

        /// <summary>
        /// Check to see if the object has changed since it was fetched.
        /// </summary>
        public bool HasChanged
        {
            get { return hashCode != CalculateHashCode(); }
        }

        /// <summary>
        /// Get the VTags.
        /// </summary>
        public List<string> VTags
        {
            // TODO: this is too complex
            get { return vtags ?? (vtags = Siblings.Count == 0 ? new List<string> { VTag } : Siblings.Select(s => s.VTag).ToList()); }
        }

        /// <summary>
        /// Fetch a single integer secondary index to work with.
        /// </summary>
        /// <param name="name">The name of the index to fetch.</param>
        /// <returns>The index.</returns>
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

        /// <summary>
        /// Fetch a single string (binary) secondary idnex to work with.
        /// </summary>
        /// <param name="name">The name of the index to fetch.</param>
        /// <returns>The index.</returns>
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

        /// <summary>
        /// Create a link from this object to another using a tag.
        /// </summary>
        /// <param name="bucket">The other object's bucket name.</param>
        /// <param name="key">The other object's key.</param>
        /// <param name="tag">The tag to use.</param>
        [Obsolete("Linkwalking has been depreciated as of Riak 2.0. This method will be removed in the next major version.")]
        public void LinkTo(string bucket, string key, string tag)
        {
            Links.Add(new RiakLink(bucket, key, tag));
        }

        /// <summary>
        /// Create a link from this object to another using a tag.
        /// </summary>
        /// <param name="riakObjectId">The other's object's Id.</param>
        /// <param name="tag">The tag to use.</param>
        /// <exception cref="RiakUnsupportedException">
        /// Thrown if RiakObjectId has a bucket type. 
        /// Combining linkwalking and bucket types is not supported.
        /// </exception>
        [Obsolete("Linkwalking has been depreciated as of Riak 2.0. This method will be removed in the next major version.")]
        public void LinkTo(RiakObjectId riakObjectId, string tag)
        {
            ThrowIfMixingBucketTypesAndLinks(riakObjectId.BucketType);

            Links.Add(riakObjectId.ToRiakLink(tag));
        }

        /// <summary>
        /// Create a link from this object to another using a tag.
        /// </summary>
        /// <param name="riakObject">The other object.</param>
        /// <param name="tag">The tag to use.</param>
        /// <exception cref="RiakUnsupportedException">
        /// Thrown if RiakObjectId has a bucket type. 
        /// Combining linkwalking and bucket types is not supported.
        /// </exception>
        [Obsolete("Linkwalking has been depreciated as of Riak 2.0. This method will be removed in the next major version.")]
        public void LinkTo(RiakObject riakObject, string tag)
        {
            ThrowIfMixingBucketTypesAndLinks(riakObject.BucketType);

            Links.Add(riakObject.ToRiakLink(tag));
        }

        /// <summary>
        /// Remove a link, that goes from this object to another.
        /// </summary>
        /// <param name="bucket">The other object's bucket.</param>
        /// <param name="key">The other object's key.</param>
        /// <param name="tag">The tag for the link to remove.</param>
        [Obsolete("Linkwalking has been depreciated as of Riak 2.0. This method will be removed in the next major version.")]
        public void RemoveLink(string bucket, string key, string tag)
        {
            var link = new RiakLink(bucket, key, tag);
            RemoveLink(link);
        }

        /// <summary>
        /// Remove a link, that goes from this object to another.
        /// </summary>
        /// <param name="riakObjectId">The other object's Id.</param>
        /// <param name="tag">The tag for the link to remove.</param>
        /// <exception cref="RiakUnsupportedException">
        /// Thrown if RiakObjectId has a bucket type. 
        /// Combining linkwalking and bucket types is not supported.
        /// </exception>
        [Obsolete("Linkwalking has been depreciated as of Riak 2.0. This method will be removed in the next major version.")]
        public void RemoveLink(RiakObjectId riakObjectId, string tag)
        {
            ThrowIfMixingBucketTypesAndLinks(riakObjectId.BucketType);

            var link = new RiakLink(riakObjectId.Bucket, riakObjectId.Key, tag);
            RemoveLink(link);
        }

        /// <summary>
        /// Remove a link, that goes from this object to another.
        /// </summary>
        /// <param name="riakObject">The other object.</param>
        /// <param name="tag">The tag for the link to remove.</param>
        /// <exception cref="RiakUnsupportedException">
        /// Thrown if RiakObjectId has a bucket type. 
        /// Combining linkwalking and bucket types is not supported.
        /// </exception>
        [Obsolete("Linkwalking has been depreciated as of Riak 2.0. This method will be removed in the next major version.")]
        public void RemoveLink(RiakObject riakObject, string tag)
        {
            ThrowIfMixingBucketTypesAndLinks(riakObject.BucketType);

            var link = new RiakLink(riakObject.Bucket, riakObject.Key, tag);
            RemoveLink(link);
        }

        /// <summary>
        /// Remove a link, that goes from this object to another.
        /// </summary>
        /// <param name="link">The link to remove.</param>
        [Obsolete("Linkwalking has been depreciated as of Riak 2.0. This method will be removed in the next major version.")]
        public void RemoveLink(RiakLink link)
        {
            Links.Remove(link);
        }

        /// <summary>
        /// Remove all links from this object that link to <paramref name="riakObject"/>.
        /// </summary>
        /// <param name="riakObject">The other object.</param>
        /// <exception cref="RiakUnsupportedException">
        /// Thrown if RiakObjectId has a bucket type. 
        /// Combining linkwalking and bucket types is not supported.
        /// </exception>
        [Obsolete("Linkwalking has been depreciated as of Riak 2.0. This method will be removed in the next major version.")]
        public void RemoveLinks(RiakObject riakObject)
        {
            ThrowIfMixingBucketTypesAndLinks(riakObject.BucketType);

            RemoveLinks(new RiakObjectId(riakObject.Bucket, riakObject.Key));
        }

        /// <summary>
        /// Remove all links from this object that link to <paramref name="riakObjectId"/>.
        /// </summary>
        /// <param name="riakObjectId">The other object's Id.</param>
        /// <exception cref="RiakUnsupportedException">
        /// Thrown if RiakObjectId has a bucket type. 
        /// Combining linkwalking and bucket types is not supported.
        /// </exception>
        [Obsolete("Linkwalking has been depreciated as of Riak 2.0. This method will be removed in the next major version.")]
        public void RemoveLinks(RiakObjectId riakObjectId)
        {
            ThrowIfMixingBucketTypesAndLinks(riakObjectId.BucketType);

            var linksToRemove = Links.Where(l => l.Bucket == riakObjectId.Bucket && l.Key == riakObjectId.Key);
            foreach (RiakLink linkToRemove in linksToRemove)
            {
                Links.Remove(linkToRemove);
            }
        }

        /// <summary>
        /// Get the Id for this object.
        /// </summary>
        /// <returns>The object Id.</returns>
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

        /// <summary>
        /// Set the object's value, after serializing it with the provided serializer.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The unserialized value.</param>
        /// <param name="serializeObject">A delegate to handle serialization of an object to a string.</param>
        public void SetObject<T>(T value, SerializeObjectToString<T> serializeObject)
            where T : class
        {
            if (serializeObject == null)
            {
                throw new ArgumentException("serializeObject cannot be null");
            }

            Value = serializeObject(value).ToRiakString();
        }

        /// <summary>
        /// Set the object's value, after serializing it with the provided serializer.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The unserialized value.</param>
        /// <param name="contentType">The content type of the object.</param>
        /// <param name="serializeObject">A delegate to handle serialization of an object to a string.</param>
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

        /// <summary>
        /// Set the object's value, after serializing it with the provided serializer.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The unserialized value.</param>
        /// <param name="serializeObject">A delegate to handle serialization of an object to a string.</param>
        public void SetObject<T>(T value, SerializeObjectToByteArray<T> serializeObject)
        {
            if (serializeObject == null)
            {
                throw new ArgumentException("serializeObject cannot be null");
            }

            Value = serializeObject(value);
        }

        /// <summary>
        /// Set the object's value, after serializing it with the provided serializer.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The unserialized value.</param>
        /// <param name="contentType">The content type of the object.</param>
        /// <param name="serializeObject">A delegate to handle serialization of an object to a string.</param>
        public void SetObject<T>(T value, string contentType, SerializeObjectToByteArray<T> serializeObject)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentException("contentType must be a valid MIME type");
            }

            ContentType = contentType;

            SetObject(value, serializeObject);
        }

        /// <summary>
        /// Set the object's value, after serializing it.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The unserialized value.</param>
        /// <param name="contentType">The content type of the object.</param>
        /// <remarks>
        /// This overload will choose its serializer based on the <paramref name="contentType"/>.
        /// Supported types for this overload are: <see cref="RiakConstants.ContentTypes.ApplicationJson"/>,
        /// <see cref="RiakConstants.ContentTypes.ProtocolBuffers"/>
        /// <see cref="RiakConstants.ContentTypes.Xml"/>
        /// and text.
        /// </remarks>
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
                var objectToByteArrayFunc = new SerializeObjectToByteArray<T>(theObject =>
                {
                    using (var ms = new MemoryStream())
                    {
                        Serializer.Serialize(ms, value);
                        return ms.ToArray();
                    }
                });

                SetObject(value, ContentType, objectToByteArrayFunc);

                return;
            }

            if (ContentType == RiakConstants.ContentTypes.Xml)
            {
                var objectToByteArrayFunc = new SerializeObjectToByteArray<T>(theObject =>
                {
                    var ms = new MemoryStream();
                    var serde = new XmlSerializer(typeof(T));
                    serde.Serialize(ms, value);
                    return ms.ToArray();
                });

                SetObject(value, ContentType, objectToByteArrayFunc);

                return;
            }

            if (ContentType.StartsWith("text"))
            {
                Value = value.ToString().ToRiakString();
                return;
            }

            throw new NotSupportedException(string.Format("Your current ContentType ({0}), is not supported.", ContentType));
        }

        /// <summary>
        /// Deserializes and returns the object's value, using the provided deserializer. 
        /// </summary>
        /// <typeparam name="T">The value's target type.</typeparam>
        /// <param name="deserializeObject">A delegate to handle deserialization of an byte[] serialized object to it's original type.</param>
        /// <param name="resolveConflict">
        /// A delegate to handle resolution of sibling objects. 
        /// Takes all the sibling objects as input and returns one "resolved" object.
        /// </param>
        /// <returns>The deserialized value.</returns>
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

        // TODO: mismatch between Get/SetObject with auto-serialization/deserialization.
        // SetObject can serialize anything with a content-type of "text*", but 
        // GetObject will not deserialize it.

        /// <summary>
        /// Deserializes and returns the object's value.
        /// </summary>
        /// <typeparam name="T">The value's target type.</typeparam>
        /// <returns>The deserialized value.</returns>
        /// <remarks>
        /// This overload will choose its deserializer based on the object's <see cref="ContentType"/> property.
        /// Supported types for this overload are: <see cref="RiakConstants.ContentTypes.ApplicationJson"/>,
        /// <see cref="RiakConstants.ContentTypes.ProtocolBuffers"/>, and <see cref="RiakConstants.ContentTypes.Xml"/>.
        /// </remarks>
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
        /// Helper method to check if we're mixing bucket types and links.
        /// Throws exception if we are.
        /// </summary>
        /// <exception cref="RiakUnsupportedException">
        /// Thrown if RiakObjectId has a bucket type. 
        /// Combining linkwalking and bucket types is not supported.
        /// </exception>
        private void ThrowIfMixingBucketTypesAndLinks(string otherBucketType)
        {
            if (!string.IsNullOrWhiteSpace(BucketType) || !string.IsNullOrWhiteSpace(otherBucketType))
            {
                throw new RiakUnsupportedException("Combining linkwalking and bucket types is not supported.");
            }
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
