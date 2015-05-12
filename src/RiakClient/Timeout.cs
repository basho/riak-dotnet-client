// <copyright file="Timeout.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

    /// <summary>
    /// Represents a wall-clock timeout for Riak operations. 
    /// </summary>
    public struct Timeout : IEquatable<Timeout>
    {
        private readonly TimeSpan timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timeout"/> struct.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> to base this <see cref="Timeout"/> off of.</param>
        public Timeout(TimeSpan timeout)
        {
            this.timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timeout"/> struct.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to base this <see cref="Timeout"/> off of.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="milliseconds"/> must be greater than or equal to zero.</exception>
        public Timeout(int milliseconds)
        {
            if (milliseconds < 0)
            {
                throw new ArgumentOutOfRangeException("milliseconds must be greater than or equal to zero");
            }

            this.timeout = TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// Cast the value of this <see cref="Int32"/> to a <see cref="Timeout"/>.
        /// </summary>
        /// <param name="timeout">The <see cref="Int32"/> value to cast to a <see cref="Timeout"/>.</param>
        /// <returns>A <see cref="Timeout"/> based on the value of the this <see cref="Int32"/>.</returns>
        public static explicit operator Timeout(int timeout)
        {
            return new Timeout(timeout);
        }

        /// <summary>
        /// Cast the value of this <see cref="Timeout"/> to an <see cref="Int32"/>.
        /// </summary>
        /// <param name="timeout">The <see cref="Timeout"/> value to cast to an <see cref="Int32"/>.</param>
        /// <returns>An <see cref="Int32"/> based on the value of the this <see cref="Timeout"/>.</returns>
        public static explicit operator int(Timeout timeout)
        {
            return (int)timeout.timeout.TotalMilliseconds;
        }

        /// <summary>
        /// Cast the value of this <see cref="Timeout"/> to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="timeout">The <see cref="Timeout"/> value to cast to a <see cref="TimeSpan"/>.</param>
        /// <returns>A <see cref="TimeSpan"/> based on the value of the this <see cref="Timeout"/>.</returns>
        public static implicit operator TimeSpan(Timeout timeout)
        {
            return timeout.timeout;
        }

        /// <summary>
        /// Cast the value of this <see cref="Timeout"/> to a <see cref="String"/>.
        /// </summary>
        /// <param name="timeout">The <see cref="Timeout"/> value to cast to a <see cref="String"/>.</param>
        /// <returns>A <see cref="String"/> based on the value of the this <see cref="Timeout"/>.</returns>
        public static implicit operator string(Timeout timeout)
        {
            return timeout.timeout.TotalMilliseconds.ToString();
        }

        /// <summary>
        /// Cast the value of this <see cref="Timeout"/> to a <see cref="UInt32"/>.
        /// </summary>
        /// <param name="timeout">The <see cref="Timeout"/> value to cast to a <see cref="UInt32"/>.</param>
        /// <returns>A <see cref="UInt32"/> based on the value of the this <see cref="Timeout"/>.</returns>
        [CLSCompliant(false)]
        public static explicit operator uint(Timeout timeout)
        {
            return (uint)timeout.timeout.TotalMilliseconds;
        }

        /// <summary>
        /// Returns a string that represents the total milliseconds of the timeout.
        /// </summary>
        /// <returns>A string that represents the total milliseconds of the timeout.</returns>
        public override string ToString()
        {
            return timeout.TotalMilliseconds.ToString();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Timeout)
            {
                return Equals((Timeout)obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public bool Equals(Timeout other)
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
        /// Uses the HashCode of the internal <see cref="TimeSpan"/> value.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return timeout.GetHashCode();
        }
    }
}