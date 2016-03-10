namespace RiakClient.Commands
{
    using Messages;

    /// <summary>
    /// Represents a command to execute against Riak
    /// </summary>
    public interface IRiakCommand
    {
        MessageCode ExpectedCode { get; }

        RpbReq ConstructPbRequest();

        void OnSuccess(RpbResp rpbResp);
    }
}