namespace Riak.Core
{
    using System;
    using System.IO;
    using RiakClient.Commands;

    internal abstract class MessageBase
    {
        protected readonly IRiakCommand Command;
        protected readonly Stream Stream;

        public MessageBase(IRiakCommand command, Stream stream)
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