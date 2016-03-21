namespace RiakClient.Messages
{
    using System;
    using System.Collections.Generic;

    [CLSCompliant(false)]
    public interface ITsByKeyReq
    {
        byte[] table { get; set; }

        List<TsCell> key { get; }

        uint timeout { get; set; }

        bool timeoutSpecified { get; set; }
    }
}
