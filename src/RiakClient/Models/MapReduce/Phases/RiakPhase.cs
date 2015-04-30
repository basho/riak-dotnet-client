// <copyright file="RiakPhase.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.MapReduce.Phases
{
    using System.IO;
    using System.Text;
    using Extensions;
    using Newtonsoft.Json;

    internal abstract class RiakPhase
    {
        private bool keep;

        /// <summary>
        /// Property to hold this phase's type string.
        /// </summary>
        public abstract string PhaseType { get; }

        /// <summary>
        /// Returns a JSON string that represents the RiakPhase.
        /// </summary>
        /// <returns>A JSON string representation of the phase.</returns>
        public override string ToString()
        {
            return ToJsonString();
        }

        /// <summary>
        /// The option to keep the results of this phase, or just pass them onto the next phase.
        /// </summary>
        /// <param name="keep"><b>true</b> to keep the phase results for the final result set, <b>false</b> to omit them. </param>
        public void Keep(bool keep)
        {
            this.keep = keep;
        }

        /// <summary>
        /// Serialize this phase to JSON.
        /// </summary>
        /// <returns>The phase as a JSON string.</returns>
        public string ToJsonString()
        {
            /*
             * NB: JsonTextWriter is guaranteed to close the StringWriter
             * https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonTextWriter.cs#L150-L160
             */
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                writer.WritePropertyName(PhaseType);

                // phase start
                writer.WriteStartObject();

                WriteJson(writer);
                writer.WriteProperty("keep", keep);
                writer.WriteEndObject();

                // phase end
                writer.WriteEndObject();
            }

            return sb.ToString();
        }

        protected abstract void WriteJson(JsonWriter writer);
    }
}
