using System.Collections.Generic;
using System.Net;

namespace CorrugatedIron.Models
{
    public class RiakRestResponse
    {
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public System.Text.Encoding ContentEncoding { get; set; }
        public string Body { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string ErrorMessage { get; set; }
    }
}
