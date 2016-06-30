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

        RiakResp DecodeResponse(byte[] buffer);

        void OnSuccess(RiakResp rpbResp);
    }
}