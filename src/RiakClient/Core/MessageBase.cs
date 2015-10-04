namespace Riak.Core
{
    using System;
    using System.IO;
    using RiakClient.Commands;

    internal abstract class MessageBase
    {
        protected readonly IRCommand Command;
        protected readonly Stream Stream;

        public MessageBase(IRCommand command, Stream stream)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            this.Command = command;
            this.Stream = stream;
        }
    }
}
