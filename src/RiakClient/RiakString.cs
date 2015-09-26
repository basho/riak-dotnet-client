// <copyright file="RiakString.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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

namespace RiakClient
{
    using System;
    using System.Text;

    /// <summary>
    /// Represents a string that encapsulates conversion to/from <see cref="String"/> and <see cref="Byte"/>[]
    /// values
    /// </summary>
    public class RiakString : IEquatable<RiakString> // TODO 3.0 rename to RString
    {
        private string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakString"/> class.
        /// </summary>
        /// <param name="value">The string from which to construct this instance.</param>
        public RiakString(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakString"/> class.
        /// </summary>
        /// <param name="value">The UTF8 byte array from which to construct this instance.</param>
        public RiakString(byte[] value)
        {
            if (value != null)
            {
                this.value = Encoding.UTF8.GetString(value);
            }
        }

        /// <summary>
        /// Returns a <see cref="Boolean"/> that indicates a non-null value.
        /// </summary>
        /// <returns>
        /// A <see cref="Boolean"/> that indicates a non-null value.
        /// </returns>
        public bool HasValue
        {
            get { return value != null; }
        }

        /// <summary>
        /// Returns a <see cref="RiakString"/> from the byte array of UTF8-encoded characters.
        /// </summary>
        /// <param name="value">The <see cref="Byte"/>[] value from which to construct a <see cref="RiakString"/>.</param>
        /// <returns>
        /// A <see cref="RiakString"/>.
        /// </returns>
        public static RiakString FromBytes(byte[] value)
        {
            return new RiakString(value);
        }

        /// <summary>
        /// Returns a <see cref="Byte"/>[] from the string, encoded in UTF8.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value from which to construct a <see cref="Byte"/>[].</param>
        /// <returns>
        /// A <see cref="Byte"/>[].
        /// </returns>
        public static byte[] ToBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// Returns a <see cref="Byte"/>[] from the string, encoded in UTF8.
        /// </summary>
        /// <param name="value">The <see cref="RiakString"/> value from which to construct a <see cref="Byte"/>[].</param>
        /// <returns>
        /// A <see cref="Byte"/>[].
        /// </returns>
        public static byte[] ToBytes(RiakString value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// Cast the value of this <see cref="RiakString"/> to an <see cref="String"/>.
        /// </summary>
        /// <param name="value">The <see cref="RiakString"/> value to cast to an <see cref="String"/>.</param>
        /// <returns>An <see cref="String"/> based on the value of the this <see cref="RiakString"/>.</returns>
        public static implicit operator string(RiakString value)
        {
            return value.value;
        }

        /// <summary>
        /// Cast the value of this <see cref="String"/> to a <see cref="RiakString"/>.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to cast to a <see cref="RiakString"/>.</param>
        /// <returns>A <see cref="RiakString"/> based on the value of the this <see cref="String"/>.</returns>
        public static implicit operator RiakString(string value)
        {
            return new RiakString(value);
        }

        /// <summary>
        /// Cast the value of this <see cref="RiakString"/> to an <see cref="Boolean"/>.
        /// </summary>
        /// <param name="value">The <see cref="RiakString"/> value to cast to an <see cref="Boolean"/>.</param>
        /// <returns>An <see cref="Boolean"/> indicating if this <see cref="RiakString"/> is null or not-null.</returns>
        public static implicit operator bool(RiakString value)
        {
            return value.HasValue;
        }

        /// <summary>
        /// Cast the value of this <see cref="RiakString"/> to an <see cref="Byte"/>[].
        /// </summary>
        /// <param name="value">The <see cref="RiakString"/> value to cast to an <see cref="Byte"/>[].</param>
        /// <returns>A UTF8-encoded <see cref="Byte"/>[] based on the value of the this <see cref="RiakString"/>.</returns>
        public static implicit operator byte[](RiakString value)
        {
            byte[] rv = null;

            if (value != null && value.HasValue)
            {
                rv = Encoding.UTF8.GetBytes(value.value);
            }

            return rv;
        }

        /// <summary>
        /// Cast the value of a <see cref="Byte"/>[] to a <see cref="RiakString"/>.
        /// </summary>
        /// <param name="value">The <see cref="Byte"/>[] value to cast to an <see cref="RiakString"/>.</param>
        /// <returns>A new <see cref="RiakString"/>.</returns>
        public static implicit operator RiakString(byte[] value)
        {
            return new RiakString(value);
        }

        /// <summary>
        /// Returns a string that represents the RiakString value.
        /// </summary>
        /// <returns>
        /// A string that represents the RiakString value. 
        /// </returns>
        public override string ToString()
        {
            string rv = null;

            if (this.value != null)
            {
                rv = this.value.ToString();
            }

            return rv;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as RiakString);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public bool Equals(RiakString other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (object.ReferenceEquals(other, this))
            {
                return true;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}