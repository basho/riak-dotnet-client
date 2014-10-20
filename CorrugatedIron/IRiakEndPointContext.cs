using System;
using CorrugatedIron.Comms;

namespace CorrugatedIron
{
    public interface IRiakEndPointContext : IDisposable
    {
        IRiakNode Node { get; set; }
        RiakPbcSocket Socket { get; set; }
    }
}