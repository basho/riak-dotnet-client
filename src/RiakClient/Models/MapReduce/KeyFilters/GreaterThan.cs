namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Tests that the input is greater than the argument.
    /// </summary>
    /// <typeparam name="T">Key filter token type</typeparam>
    internal class GreaterThan<T> : IRiakKeyFilterToken
    {
        private readonly Tuple<string, T> keyFilterDefinition;

        public GreaterThan(T arg)
        {
            keyFilterDefinition = Tuple.Create("greater_than", arg);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public T Argument
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
                jw.WriteValue(Argument);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}
