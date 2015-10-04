namespace RiakClient.Commands
{
    using System;
    using Messages;
    using Riak;
    using Riak.Core;

    /// <summary>
    /// Represents a command to execute against Riak
    /// </summary>
    public interface IRCommand
    {
        // TODO 3.0 make this a generic interfact with TResult, Name only
        string Name
        {
            get;
        }

        MessageCode RequestCode { get; }

        MessageCode ResponseCode { get; }

        Type ResponseType { get; }

        RpbReq ConstructPbRequest();

        void OnSuccess(RpbResp rpbResp);

        void OnError(RError error);

        void SetLastNode(Node node);
    }
}
