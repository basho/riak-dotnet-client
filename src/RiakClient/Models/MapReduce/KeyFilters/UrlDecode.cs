namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// URL-decodes the string.
    /// </summary>
    internal class UrlDecode : IRiakKeyFilterToken
    {
        public string FunctionName
        {
            get { return "urldecode"; }
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
