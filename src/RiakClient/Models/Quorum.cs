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

        /*
        private static readonly HashSet<string> validQuorumStrings = new HashSet<string> { "all", "quorum", "one", "default" };

        private static readonly Dictionary<string, int> QuorumOptionsLookup = new Dictionary<string, int>
        {
            { "one",     One },
            { "quorum",  Quorum },
            { "all",     All },
            { "default", Default }
        };
         */
    }
}