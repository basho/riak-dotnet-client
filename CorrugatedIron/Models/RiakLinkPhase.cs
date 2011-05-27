using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CorrugatedIron.Models
{
    public class RiakLinkPhase : IRiakMapReducePhase
    {
        public string MapReducePhaseType { get; set; }
        public bool Keep { get; set; }
        public string Bucket { get; set; }
        public string Tag { get; set; }

        public override string ToString()
        {
            return ToJsonString();
        }

        public string ToJsonString()
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WriteStartObject();

                if (!string.IsNullOrEmpty(Bucket))
                {
                    jw.WritePropertyName("bucket");
                    jw.WriteValue(Bucket);
                }

                if (!string.IsNullOrEmpty(Tag))
                {
                    jw.WritePropertyName("tag");
                    jw.WritePropertyName(Tag);
                }

                jw.WritePropertyName("keep");
                jw.WriteValue(Keep);

                jw.WriteEndObject();
            }

            return sb.ToString();
        }
    }
}