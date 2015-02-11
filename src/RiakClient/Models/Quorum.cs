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

    public class Quorum
    {
        private const int OneI = 1;
        private const int QuorumI = 2;
        private const int AllI = 3;
        private const int DefaultI = 4;

        private const uint OneU = uint.MaxValue - OneI;
        private const uint QuorumU = uint.MaxValue - QuorumI;
        private const uint AllU = uint.MaxValue - AllI;
        private const uint DefaultU = uint.MaxValue - DefaultI;

        private static readonly IDictionary<string, uint> QuorumStrMap = new Dictionary<string, uint>
        {
            { "one", OneU },
            { "quorum", QuorumU },
            { "all", AllU },
            { "default", DefaultU }
        };

        private static readonly IDictionary<int, uint> QuorumIntMap = new Dictionary<int, uint>
        {
            { OneI, OneU },
            { QuorumI, QuorumU },
            { AllI, AllU },
            { DefaultI, DefaultU }
        };

        private readonly uint quorumValue = 0;

        public Quorum(string quorum)
        {
            if (string.IsNullOrWhiteSpace(quorum))
            {
                throw new ArgumentNullException("quorum");
            }

            uint tmp;
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
            uint tmp;
            if (QuorumIntMap.TryGetValue(quorum, out tmp))
            {
                quorumValue = tmp;
            }
            else
            {
                if (quorum >= 0)
                {
                    quorumValue = (uint)Math.Abs(quorum);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("quorum");
                }
            }
        }

        public static implicit operator int(Quorum quorum)
        {
            return 1;
        }

        [CLSCompliant(false)]
        public static implicit operator uint(Quorum quorum)
        {
            return quorum.quorumValue;
        }

        public static implicit operator string(Quorum quorum)
        {
            return quorum.ToString();
        }
    }
}