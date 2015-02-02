// <copyright file="RiakActionPhase.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.MapReduce.Phases
{
    using Models.MapReduce.Languages;
    using Newtonsoft.Json;

    internal abstract class RiakActionPhase<TLanguage> : RiakPhase
        where TLanguage : IRiakPhaseLanguage, new()
    {
        private readonly TLanguage language;
        private object argument;

        protected RiakActionPhase()
        {
            this.language = new TLanguage();
        }

        public TLanguage Language
        {
            get { return language; }
        }

        public void Argument<T>(T argument)
        {
            this.argument = argument;
        }

        protected override void WriteJson(JsonWriter writer)
        {
            Language.WriteJson(writer);

            if (argument != null)
            {
                var json = JsonConvert.SerializeObject(argument);
                writer.WritePropertyName("arg");
                writer.WriteRawValue(json);
            }
        }
    }
}
