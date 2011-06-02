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
