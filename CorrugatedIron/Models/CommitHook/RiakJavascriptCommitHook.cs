using CorrugatedIron.Extensions;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.CommitHook
{
    public class RiakJavascriptCommitHook : RiakCommitHook, IRiakPreCommitHook
    {
        public string Name { get; private set; }

        public RiakJavascriptCommitHook(string name)
        {
            Name = name;
        }

        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("name", Name);
            writer.WriteEndObject();
        }
    }
}
