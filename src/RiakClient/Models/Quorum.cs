// <copyright file="Quorum.cs" company="Basho Technologies, Inc.">
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

        public static implicit operator int(Quorum quorum)
        {
            return (int)quorum.quorumValue;
        }

        public static explicit operator Quorum(int quorum)
        {
            return new Quorum(quorum);
        }

        public static implicit operator string(Quorum quorum)
        {
            return quorum.ToString();
        }

        public static explicit operator Quorum(string quorum)
        {
            return new Quorum(quorum);
        }

        [CLSCompliant(false)]
        public static implicit operator uint(Quorum quorum)
        {
            return (uint)quorum.quorumValue;
        }

        [CLSCompliant(false)]
        public static explicit operator Quorum(uint quorum)
        {
            return new Quorum(quorum);
        }

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

        public override bool Equals(object obj)
        {
            return Equals(obj as Quorum);
        }

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

        public override int GetHashCode()
        {
            return quorumValue.GetHashCode();
        }

        public static class WellKnown
        {
            private static readonly Quorum OneStatic = new Quorum(Models.Quorum.OneAsInt);
            private static readonly Quorum QuorumStatic = new Quorum(Models.Quorum.QuorumAsInt);
            private static readonly Quorum AllStatic = new Quorum(Models.Quorum.AllAsInt);
            private static readonly Quorum DefaultStatic = new Quorum(Models.Quorum.DefaultAsInt);

            public static Quorum One
            {
                get { return OneStatic; }
            }

            public static Quorum Quorum
            {
                get { return QuorumStatic; }
            }

            public static Quorum All
            {
                get { return AllStatic; }
            }

            public static Quorum Default
            {
                get { return DefaultStatic; }
            }
        }
    }
}