namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Splits the input on the string given as the first argument and returns the nth
    /// token specified by the second argument.
    /// </summary>
    internal class Tokenize : IRiakKeyFilterToken
    {
        private readonly Tuple<string, string, int> keyFilterDefinition;

        public Tokenize(string token, int position)
        {
            keyFilterDefinition = Tuple.Create("tokenize", token, position);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public string Token
        {
            get { return keyFilterDefinition.Item2; }
        }

        public int Position
        {
            get { return keyFilterDefinition.Item3; }
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
                jw.WriteValue(Token);
                jw.WriteValue(Position);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}
