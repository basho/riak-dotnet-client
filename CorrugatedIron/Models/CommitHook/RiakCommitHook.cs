using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.CommitHook
{
    public interface IRiakCommitHook
    {
        string ToJsonString();
        void WriteJson(JsonWriter writer);
    }

    public abstract class RiakCommitHook : IRiakCommitHook
    {
        protected RiakCommitHook()
        {
        }

        public string ToJsonString()
        {
            var sb = new StringBuilder();
            
            using(var sw = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                WriteJson(writer);
            }
            
            return sb.ToString();
        }

        public abstract void WriteJson(JsonWriter writer);
    }
}
