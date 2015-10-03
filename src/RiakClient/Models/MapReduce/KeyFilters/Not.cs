namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    /// Negates the result of key-filter operations.
    /// </summary>
    internal class Not : IRiakKeyFilterToken
    {
        private readonly Tuple<string, List<IRiakKeyFilterToken>> keyFilterDefinition;

        public Not(List<IRiakKeyFilterToken> arg)
        {
            keyFilterDefinition = Tuple.Create("not", arg);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public List<IRiakKeyFilterToken> Argument
        {
            get { return keyFilterDefinition.Item2; }
        }

        public override string ToString()
        {
            return ToJsonString();
        }

        public string ToJsonString()
        {
            /*
             * NB: JsonTextWriter is guaranteed to close the StringWriter
             * https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/JsonTextWriter.cs#L150-L160
             */
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartArray();

                jw.WriteValue(FunctionName);

                jw.WriteRawFilterTokenArray(Argument);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}
