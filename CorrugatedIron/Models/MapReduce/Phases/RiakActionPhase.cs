// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Extensions;
using CorrugatedIron.Models.MapReduce.Languages;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Phases
{
    internal abstract class RiakActionPhase<TLanguage> : RiakPhase
        where TLanguage : IRiakPhaseLanguage, new()
    {
        private object _argument;
        public TLanguage Language { get; private set; }

        protected RiakActionPhase()
        {
            Language = new TLanguage();
        }

        public void Argument<T>(T argument)
        {
            _argument = argument;
        }

        protected override void WriteJson(JsonWriter writer)
        {
            Language.WriteJson(writer);

            if (_argument != null)
            {
                var json = JsonConvert.SerializeObject(_argument);
                writer.WritePropertyName("arg");
                writer.WriteRawValue(json);
            }
        }
    }
}