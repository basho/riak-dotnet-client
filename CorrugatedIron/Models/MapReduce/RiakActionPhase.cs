// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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

using System;
using CorrugatedIron.Extensions;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce
{
    public abstract class RiakActionPhase : RiakPhase
    {
        private PhaseLanguage _language = PhaseLanguage.Javascript;
        private string _name;
        private string _source;
        private string _argument;

        public RiakActionPhase Langauge(PhaseLanguage language)
        {
            _language = language;
            return this;
        }

        public RiakActionPhase Name(string name)
        {
            _name = name;
            return this;
        }

        public RiakActionPhase Source(string source)
        {
            _source = source;
            return this;
        }

        public RiakActionPhase Argument(string argument)
        {
            _argument = argument;
            return this;
        }

        protected override void WriteJson(JsonWriter writer)
        {
            if (string.IsNullOrEmpty(_name) && string.IsNullOrEmpty(_source))
            {
                throw new Exception("One of Name or Source must be supplied");
            }

            if (!string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_source))
            {
                throw new Exception("Only one of Name and Source may be supplied");
            }

            writer.WriteProperty("language", ToString(_language))
                .WriteSpecifiedProperty("name", _name)
                .WriteSpecifiedProperty("source", _source)
                .WriteSpecifiedProperty("arg", _argument);
        }
    }
}
