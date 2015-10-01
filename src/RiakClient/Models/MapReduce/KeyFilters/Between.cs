namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Tests that the input is between the first two arguments. 
    /// If the third argument is given, it is whether to treat the range as inclusive. 
    /// If the third argument is omitted, the range is treated as inclusive.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <remarks>It is assumed that left and right supply their own JSON conversion.</remarks>
    internal class Between<T> : IRiakKeyFilterToken
    {
        private readonly Tuple<string, T, T, bool> keyFilterDefinition;

        public Between(T left, T right, bool inclusive = true)
        {
            keyFilterDefinition = Tuple.Create("between", left, right, inclusive);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public T Left
        {
            get { return keyFilterDefinition.Item2; }
        }

        public T Right
        {
            get { return keyFilterDefinition.Item3; }
        }

        public bool Inclusive
        {
            get { return keyFilterDefinition.Item4; }
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
                jw.WriteValue(Left);
                jw.WriteValue(Right);
                jw.WriteValue(Inclusive);

                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}
