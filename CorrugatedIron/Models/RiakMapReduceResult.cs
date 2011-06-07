using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorrugatedIron.Messages;

namespace CorrugatedIron.Models
{
    public class RiakMapReduceResult
    {
        public bool Done { get; set; }
        public byte[] Response { get; set; }

        public RiakMapReduceResult()
        {

        }

        internal RiakMapReduceResult(RpbMapRedResp response)
        {
            Done = response.Done;
            Response = response.Response;

        }
    }
}
