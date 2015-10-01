namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Tests that the input ends with the argument (a string).
    /// </summary>
    internal class EndsWith : IRiakKeyFilterToken
    {
        private readonly Tuple<string, string> keyFilterDefinition;

        public EndsWith(string arg)
        {
            keyFilterDefinition = Tuple.Create("ends_with", arg);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public string Argument
        {
            get { return keyFilterDefinition.Item2; }
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
                jw.WriteValue(Argument);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToJsonString();
        }
    }
}
