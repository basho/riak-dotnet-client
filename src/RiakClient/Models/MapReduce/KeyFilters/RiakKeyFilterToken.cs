namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    internal abstract class RiakKeyFilterToken : IRiakKeyFilterToken
    {
        private readonly Tuple<string, object, object> keyFilterDefinition;

        protected RiakKeyFilterToken()
        {
        }

        protected RiakKeyFilterToken(string functionName, params object[] args)
        {
            keyFilterDefinition = new Tuple<string, object, object>(functionName, args[0], args[1]);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public object[] Arguments
        {
            get { return new[] { keyFilterDefinition.Item2, keyFilterDefinition.Item3 }; }
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

                WriteArguments(jw);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }

        protected virtual void WriteArguments(JsonWriter writer)
        {
            writer.WriteValue(keyFilterDefinition.Item2);
            writer.WriteValue(keyFilterDefinition.Item3);
        }
    }
}
