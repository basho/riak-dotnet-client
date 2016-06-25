namespace RiakClient.Models.MapReduce.KeyFilters
{
    using System.Linq;
    using Newtonsoft.Json;

    internal abstract class RiakCompositeKeyFilterToken : RiakKeyFilterToken
    {
        protected RiakCompositeKeyFilterToken(string functionName, params object[] args)
            : base(functionName, args)
        {
        }

        protected override void WriteArguments(JsonWriter writer)
        {
            foreach (IRiakKeyFilterToken keyFilterToken in Arguments.Cast<IRiakKeyFilterToken>())
            {
                WriteArgumentAsArray(keyFilterToken, writer);
            }
        }

        protected void WriteArgumentAsArray(IRiakKeyFilterToken argument, JsonWriter writer)
        {
            /*
             * TODO: is StartArray really not needed? 
             * writer.WriteStartArray();
             */

            writer.WriteRawValue(argument.ToJsonString());

            // writer.WriteEndArray();
        }
    }
}
