// <copyright file="RiakFluentActionPhaseErlang.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.MapReduce.Fluent
{
    using Models.MapReduce.Languages;
    using Models.MapReduce.Phases;

    public class RiakFluentActionPhaseErlang
    {
        private readonly RiakActionPhase<RiakPhaseLanguageErlang> phase;

        internal RiakFluentActionPhaseErlang(RiakActionPhase<RiakPhaseLanguageErlang> phase)
        {
            this.phase = phase;
        }

        public RiakFluentActionPhaseErlang Keep(bool keep)
        {
            phase.Keep(keep);
            return this;
        }

        public RiakFluentActionPhaseErlang Argument<T>(T argument)
        {
            phase.Argument(argument);
            return this;
        }

        public RiakFluentActionPhaseErlang ModFun(string module, string function)
        {
            phase.Language.ModFun(module, function);
            return this;
        }
    }
}
