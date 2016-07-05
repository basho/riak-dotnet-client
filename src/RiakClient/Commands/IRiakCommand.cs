namespace RiakClient.Commands
{
    using Messages;

    /// <summary>
    /// Represents a command to execute against Riak
    /// </summary>
    public interface IRiakCommand
    {
        // TODO FUTURE
        // interface should not be concerned with encoding or Riak msg
        MessageCode ExpectedCode { get; }

        RiakReq ConstructRequest(bool useTtb);

        RiakResp DecodeResponse(byte[] buffer);

        void OnSuccess(RiakResp rpbResp);
    }
}