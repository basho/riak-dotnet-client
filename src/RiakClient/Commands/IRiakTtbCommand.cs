namespace RiakClient.Commands
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a TTB command to execute against Riak
    /// </summary>
    public interface IRiakTtbCommand : IRiakCommand
    {
        Action<MemoryStream> GetSerializer();
    }
}