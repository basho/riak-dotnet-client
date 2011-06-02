using CorrugatedIron.Extensions;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce
{
    public class RiakLinkPhase : RiakPhase
    {
        private string _bucket;
        private string _tag;
        private string _key;

        public override string PhaseType
        {
            get { return "link"; }
        }

        public RiakLinkPhase Bucket(string bucket)
        {
            _bucket = bucket;
            return this;
        }

        public RiakLinkPhase Tag(string tag)
        {
            _tag = tag;
            return this;
        }

        public RiakLinkPhase Key(string key)
        {
            _key = key;
            return this;
        }

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteSpecifiedProperty("bucket", _bucket)
                .WriteSpecifiedProperty("tag", _tag)
                .WriteSpecifiedProperty("key", _key);
        }
    }
}
