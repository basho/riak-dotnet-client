namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    /// Joins two or more key-filter operations with a logical OR operation.
    /// </summary>
    internal class Or : IRiakKeyFilterToken
    {
        private readonly Tuple<string, List<IRiakKeyFilterToken>, List<IRiakKeyFilterToken>> keyFilterDefinition;

        public Or(List<IRiakKeyFilterToken> left, List<IRiakKeyFilterToken> right)
        {
            keyFilterDefinition = Tuple.Create("or", left, right);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public List<IRiakKeyFilterToken> Left
        {
            get { return keyFilterDefinition.Item2; }
        }

        public List<IRiakKeyFilterToken> Right
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

                jw.WriteRawFilterTokenArray(Left);

                jw.WriteRawFilterTokenArray(Right);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}
