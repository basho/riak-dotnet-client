using System.Collections.Generic;
using CorrugatedIron.Util;

namespace CorrugatedIron.Models
{
    public class RiakRestRequest
    {
        public string Uri { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
        public byte[] Body { get; set; }
        public Dictionary<string, string> Headers { get; private set; }
        public int Timeout { get; set; }
        public bool Cache { get; set; }

        public RiakRestRequest(string uri, string method)
        {
            Uri = uri;
            Method = method;
            Headers = new Dictionary<string, string>();
            Timeout = Constants.Defaults.Rest.Timeout;
            Cache = false;
        }

        public RiakRestRequest AddHeader(string key, string value)
        {
            Headers.Add(key, value);
            return this;
        }
    }
}
