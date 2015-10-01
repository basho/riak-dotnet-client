namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Tests that input is within the Levenshtein distance of the first argument given by the second argument.
    /// </summary>
    /// <typeparam name="T">Type of key filter token</typeparam>
    internal class SimilarTo<T> : IRiakKeyFilterToken
    {
        private readonly Tuple<string, T, int> keyFilterDefinition;

        public SimilarTo(T arg, int distance)
        {
            keyFilterDefinition = Tuple.Create("similar_to", arg, distance);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public T Argument
        {
            get { return keyFilterDefinition.Item2; }
        }

        public int Distance
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
                jw.WriteValue(Argument);
                jw.WriteValue(Distance);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}
