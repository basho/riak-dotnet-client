// <copyright file="RiakFluentActionPhaseErlang.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.MapReduce.Fluent
{
    using Models.MapReduce.Languages;
    using Models.MapReduce.Phases;

    /// <summary>
    /// A fluent builder class for defining Erlang Map and Reduce phases.
    /// </summary>
    public class RiakFluentActionPhaseErlang
    {
        private readonly RiakActionPhase<RiakPhaseLanguageErlang> phase;

        internal RiakFluentActionPhaseErlang(RiakActionPhase<RiakPhaseLanguageErlang> phase)
        {
            this.phase = phase;
        }

        /// <summary>
        /// The option to keep the results of this phase, or just pass them onto the next phase.
        /// </summary>
        /// <param name="keep"><b>true</b> to keep the phase results for the final result set, <b>false</b> to omit them. </param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentActionPhaseErlang Keep(bool keep)
        {
            phase.Keep(keep);
            return this;
        }

        /// <summary>
        /// The arguments to pass to the Erlang function that's defined in the <paramref name="ModFun"/> argument.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argument">The Erlang ModFun argument.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentActionPhaseErlang Argument<T>(T argument)
        {
            phase.Argument(argument);
            return this;
        }

        /// <summary>
        /// The Erlang function to execute for this phase.
        /// </summary>
        /// <param name="module">The module containing the <paramref name="function"/> to execute.</param>
        /// <param name="function">The function to execute for this mapreduce phase.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakFluentActionPhaseErlang ModFun(string module, string function)
        {
            phase.Language.ModFun(module, function);
            return this;
        }
    }
}
