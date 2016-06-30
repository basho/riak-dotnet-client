namespace RiakClient.Commands
{
    using Messages;

    /// <summary>
    /// Represents a command to execute against Riak
    /// </summary>
    public interface IRiakCommand
    {
        MessageCode ExpectedCode { get; }

        RiakReq ConstructRequest();

        RpbResp DecodeResponse(byte[] buffer);

        void OnSuccess(RpbResp rpbResp);
    }
}