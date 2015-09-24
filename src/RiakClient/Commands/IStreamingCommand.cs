namespace RiakClient.Commands
{
    /// <summary>
    /// Represents a command that streams from Riak.
    /// </summary>
    public interface IStreamingCommand
    {
        bool Done { get; }
    }
}