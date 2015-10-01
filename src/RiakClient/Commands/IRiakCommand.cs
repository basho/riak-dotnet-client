namespace RiakClient.Commands
{
    using System;
    using Messages;
    using Riak;

    /// <summary>
    /// Represents a command to execute against Riak
    /// </summary>
    public interface IRiakCommand // TODO 3.0 rename?
    {
        MessageCode RequestCode { get; }

        MessageCode ResponseCode { get; }

        Type ResponseType { get; }

        RpbReq ConstructPbRequest();

        void OnSuccess(RpbResp rpbResp);

        void OnError(RError error);
    }
}
