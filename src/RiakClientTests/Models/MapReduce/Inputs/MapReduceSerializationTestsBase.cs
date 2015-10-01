namespace RiakClientTests.Models.MapReduce.Inputs
{
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    public abstract class MapReduceSerializationTestsBase
    {
        protected static string Serialize(Func<JsonWriter, JsonWriter> doWrite)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                doWrite(writer);
            }

            return sb.ToString();
        }
    }
}
