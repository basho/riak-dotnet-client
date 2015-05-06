// <copyright file="Quorum.cs" company="Basho Technologies, Inc.">
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
    using System.Collections.Generic;

    /// <summary>
    /// Represents the possible values for Riak operation parameters such as R, W, PR, PW, DW, and RW. 
    /// </summary>
    public class Quorum : IEquatable<Quorum>
    {
        private const int OneAsInt = -2;
        private const int QuorumAsInt = -3;
        private const int AllAsInt = -4;
        private const int DefaultAsInt = -5;

        private static readonly IDictionary<string, int> QuorumStrMap = new Dictionary<string, int>
        {
            { "one", OneAsInt },
            { "quorum", QuorumAsInt },
            { "all", AllAsInt },
            { "default", DefaultAsInt }
        };

        private static readonly IDictionary<int, string> QuorumIntMap = new Dictionary<int, string>
        {
            { OneAsInt, "one" },
            { QuorumAsInt, "quorum" },
            { AllAsInt, "all" },
            { DefaultAsInt, "default" }
        };

        private readonly int quorumValue = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Quorum"/> class.
        /// </summary>
        /// <param name="quorum">A well known quorum value string, such as "one", "quorum", "all", or "default".</param>
        /// <exception cref="ArgumentNullException">
        /// The value of <paramref name="quorum"/> cannot be null, empty, or whitespace.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value of <paramref name="quorum"/> must be well known quorum value. 
        /// Valid values are "one", "quorum", "all", and "default".
        /// </exception>
        public Quorum(string quorum)
        {
            if (string.IsNullOrWhiteSpace(quorum))
            {
                throw new ArgumentNullException("quorum");
            }

            int tmp;
            if (QuorumStrMap.TryGetValue(quorum.ToLowerInvariant(), out tmp))
            {
                quorumValue = tmp;
            }
            else
            {
                throw new ArgumentOutOfRangeException("quorum");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quorum"/> class.
        /// </summary>
        /// <param name="quorum">An integer, representing the number of nodes to use for the quorum.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The quorum value must be either a positive integer, 0, or between [-5,-2] for special cases.
        /// </exception>
        public Quorum(int quorum)
        {
            if (quorum >= 0)
            {
                quorumValue = quorum;
            }
            else
            {
                if (quorum >= -5 && quorum <= -2)
                {
                    quorumValue = quorum;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("quorum");
                }
            }
        }

#pragma warning disable 3019
        [CLSCompliant(false)]
        internal Quorum(uint quorum)
            : this((int)quorum)
        {
        }
#pragma warning restore 3019

        /// <summary>
        /// Cast the value of this <see cref="Quorum"/> to an <see cref="Int32"/>.
        /// </summary>
        /// <param name="quorum">The <see cref="Quorum"/> value to cast to an <see cref="Int32"/>.</param>
        /// <returns>An <see cref="Int32"/> based on the value of the this <see cref="Quorum"/>.</returns>
        public static implicit operator int(Quorum quorum)
        {
            return (int)quorum.quorumValue;
        }

        /// <summary>
        /// Cast the value of this <see cref="Int32"/> to a <see cref="Quorum"/>.
        /// </summary>
        /// <param name="quorum">The <see cref="Int32"/> value to cast to a <see cref="Quorum"/>.</param>
        /// <returns>A <see cref="Quorum"/> based on the value of the this <see cref="Int32"/>.</returns>
        public static explicit operator Quorum(int quorum)
        {
            return new Quorum(quorum);
        }

        /// <summary>
        /// Cast the value of this <see cref="Quorum"/> to a <see cref="String"/>.
        /// </summary>
        /// <param name="quorum">The <see cref="Quorum"/> value to cast to a <see cref="String"/>.</param>
        /// <returns>A <see cref="String"/> based on the value of the this <see cref="Quorum"/>.</returns>
        public static implicit operator string(Quorum quorum)
        {
            return quorum.ToString();
        }

        /// <summary>
        /// Cast the value of this <see cref="String"/> to a <see cref="Quorum"/>.
        /// </summary>
        /// <param name="quorum">The <see cref="String"/> value to cast to a <see cref="Quorum"/>.</param>
        /// <returns>A <see cref="Quorum"/> based on the value of the this <see cref="String"/>.</returns>
        public static explicit operator Quorum(string quorum)
        {
            return new Quorum(quorum);
        }

        /// <summary>
        /// Cast the value of this <see cref="Quorum"/> to a <see cref="UInt32"/>.
        /// </summary>
        /// <param name="quorum">The <see cref="Quorum"/> value to cast to a <see cref="UInt32"/>.</param>
        /// <returns>A <see cref="UInt32"/> based on the value of the this <see cref="Quorum"/>.</returns>
        [CLSCompliant(false)]
        public static implicit operator uint(Quorum quorum)
        {
            /*
             * NB: this is the default since the defaultValue attribute for quorum values
             * is default(uint) as well.
             * See DtUpdateReq, for instance.
             */
            uint rv = default(uint);

            if (quorum != null)
            {
                rv = (uint)quorum.quorumValue;
            }

            return rv;
        }

        /// <summary>
        /// Cast the value of this <see cref="UInt32"/> to a <see cref="Quorum"/>.
        /// </summary>
        /// <param name="quorum">The <see cref="UInt32"/> value to cast to a <see cref="Quorum"/>.</param>
        /// <returns>A <see cref="Quorum"/> based on the value of the this <see cref="UInt32"/>.</returns>
        [CLSCompliant(false)]
        public static explicit operator Quorum(uint quorum)
        {
            return new Quorum(quorum);
        }

        /// <summary>
        /// Returns a string that represents the Quorum value.
        /// </summary>
        /// <returns>
        /// A string that represents the Quorum value. 
        /// Well known strings such as "one", "quorum", "all", and "default" are returned if possible.
        /// If value is not a well known string, it's <see cref="Int32.ToString()"/> value will be used.
        /// </returns>
        public override string ToString()
        {
            string tmp;
            if (QuorumIntMap.TryGetValue(quorumValue, out tmp))
            {
                return tmp;
            }
            else
            {
                return quorumValue.ToString();
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Quorum);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><b>true</b> if the specified object is equal to the current object, otherwise, <b>false</b>.</returns>
        public bool Equals(Quorum other)
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
            return quorumValue.GetHashCode();
        }

        /// <summary>
        /// A collection of well known static quorum values, pre-initialized for use.
        /// </summary>
        public static class WellKnown
        {
            private static readonly Quorum OneStatic = new Quorum(Quorum.OneAsInt);
            private static readonly Quorum QuorumStatic = new Quorum(Quorum.QuorumAsInt);
            private static readonly Quorum AllStatic = new Quorum(Quorum.AllAsInt);
            private static readonly Quorum DefaultStatic = new Quorum(Quorum.DefaultAsInt);

            /// <summary>
            /// The "one" Quorum instance.
            /// Only one replica must respond to a read or write request before it is considered successful.
            /// </summary>
            public static Quorum One
            {
                get { return OneStatic; }
            }

            /// <summary>
            /// The "quorum" Quorum instance.
            /// A majority of replicas must respond to a read or write request before it is considered successful.
            /// </summary>
            public static Quorum Quorum
            {
                get { return QuorumStatic; }
            }

            /// <summary>
            /// The "all" Quorum instance.
            /// All replicas that must respond to a read or write request before it is considered successful.
            /// </summary>
            public static Quorum All
            {
                get { return AllStatic; }
            }

            /// <summary>
            /// The "default" Quorum instance.
            /// The default number of replicas must respond to a read or write request before it is considered successful.
            /// Riak will use the bucket (or global) default value if this <see cref="Quorum" /> is used.
            /// The true default value can be found in a bucket's properties, and varies for different parameters.
            /// </summary>
            public static Quorum Default
            {
                get { return DefaultStatic; }
            }
        }
    }
}