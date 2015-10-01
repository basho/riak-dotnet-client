namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Tests that the input is contained in the set given as the arguments.
    /// </summary>
    /// <typeparam name="T">Type of key filter token</typeparam>
    internal class SetMember<T> : IRiakKeyFilterToken
    {
        private readonly Tuple<string, List<T>> keyFilterDefinition;

        public SetMember(List<T> set)
        {
            keyFilterDefinition = Tuple.Create("set_member", set);
        }

        public string FunctionName
        {
            get { return keyFilterDefinition.Item1; }
        }

        public List<T> Argument
        {
            get { return Set; }
        }

        public List<T> Set
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

                Set.ForEach(v => jw.WriteValue(v));

                jw.WriteEndArray();
            }
            
            return sb.ToString();
        }
    }
}
