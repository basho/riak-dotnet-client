using CorrugatedIron.Extensions;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.CommitHook
{
    public class RiakErlangCommitHook : RiakCommitHook, IRiakPreCommitHook, IRiakPostCommitHook
    {
        public string Module { get; private set; }
        public string Function { get; private set; }

        public RiakErlangCommitHook(string module, string function)
        {
            Module = module;
            Function = function;
        }

        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("mod", Module);
            writer.WriteProperty("fun", Function);
            writer.WriteEndObject();
        }
    }
}
