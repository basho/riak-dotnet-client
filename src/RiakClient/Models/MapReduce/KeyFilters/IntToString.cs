namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Turns an integer (previously extracted with string_to_int), into a string.
    /// </summary>
    internal class IntToString : IRiakKeyFilterToken
    {
        public string FunctionName
        {
            get { return "int_to_string"; }
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

                jw.WriteEndArray();
            }

            return sb.ToString();
        }
    }
}
