namespace RiakClient.Commands
{
    using System;
    using Messages;
    using Riak;

    /// <summary>
    /// Represents a builder for commands.
    /// </summary>
    public interface IRCommandBuilder
    {
        IRCommand Build();
    }
}
