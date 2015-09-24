namespace Riak.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using RiakClient.Commands;
    using RiakClient.Messages;

    internal class MessageReader : MessageBase
    {
        public MessageReader(IRiakCommand command, Stream stream)
            : base(command, stream)
        {
        }

        public async Task<Result> ReadAsync()
        {
            bool done = false;
            IStreamingCommand streamingCommand = Command as IStreamingCommand;

            while (!done)
            {
                byte[] data = await ReadPbMessageAsync();

                var decoder = new MessageDecoder(Command, data);

                RError err = decoder.MaybeError();
                if (err != null)
                {
                    Command.OnError(err);
                    return new Result(err);
                }

                RpbResp decoded = decoder.DecodeMessage();
                Command.OnSuccess(decoded);

                done = true;

                if (streamingCommand != null)
                {
                    done = streamingCommand.Done;
                }
            }

            return new Result();
        }

        private static void CheckRead(int actualRead, uint expectedRead)
        {
            if (actualRead != expectedRead)
            {
                string errorMessage = string.Format(
                    Properties.Resources.Riak_Core_ConnectionDidNotReadFullMessageException_fmt,
                        expectedRead,
                        actualRead);
                throw new ConnectionReadException(errorMessage);
            }
        }

        private async Task<byte[]> ReadPbMessageAsync()
        {
            var sizeBuf = new byte[MessageConstants.PbMsgSizeLen];
            int actualRead = await ReadFullAsync(sizeBuf);
            CheckRead(actualRead, MessageConstants.PbMsgSizeLen);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(sizeBuf);
            }

            uint expectedMessageSize = BitConverter.ToUInt32(sizeBuf, 0);

            var messageBuf = new byte[expectedMessageSize];
            actualRead = await ReadFullAsync(messageBuf);
            CheckRead(actualRead, expectedMessageSize);

            // First byte is message code, rest is data.
            return messageBuf;
        }

        private async Task<int> ReadFullAsync(byte[] buffer)
        {
            int totalBytesReceived = 0;
            int remainingBytesToReceive = buffer.Length;

            while (remainingBytesToReceive > 0)
            {
                if (!Stream.CanRead)
                {
                    throw new ConnectionReadException(Properties.Resources.Riak_Core_ConnectionCanReadIsFalseException);
                }

                int bytesReceived = await Stream.ReadAsync(buffer, totalBytesReceived, remainingBytesToReceive);
                if (bytesReceived == 0)
                {
                    // NB: Based on the docs, this isn't necessarily an exception
                    // http://msdn.microsoft.com/en-us/library/system.net.sockets.networkstream.read(v=vs.110).aspx
                    break;
                }

                totalBytesReceived += bytesReceived;
                remainingBytesToReceive -= bytesReceived;
            }

            return totalBytesReceived;
        }
    }
}