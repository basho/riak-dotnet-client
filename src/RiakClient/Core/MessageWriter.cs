namespace Riak.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using ProtoBuf;
    using RiakClient.Commands;
    using RiakClient.Messages;

    internal class MessageWriter : MessageBase
    {
        public MessageWriter(IRiakCommand command, Stream stream)
            : base(command, stream)
        {
        }

        public static async Task SerializeAndStreamAsync(object obj, MessageCode requestCode, Stream stream)
        {
            using (var mstream = new MemoryStream())
            {
                // add a buffer to the start of the array to put the size and message code
                mstream.Position += MessageConstants.PbMsgHeaderSize;

                if (obj != null)
                {
                    // TODO 3.0 evaluate using await Task.Run for this...
                    Serializer.NonGeneric.Serialize(mstream, obj);
                }

                uint dataSize = (uint)(mstream.Position - MessageConstants.PbMsgHeaderSize) + MessageConstants.PbMsgCodeSize;
                byte[] headerBuf = BuildHeader(requestCode, dataSize);

                mstream.Seek(0, SeekOrigin.Begin);
                await mstream.WriteAsync(headerBuf, 0, MessageConstants.PbMsgHeaderSize);

                mstream.Seek(0, SeekOrigin.Begin);
                await mstream.CopyToAsync(stream);
            }
        }

        public static byte[] BuildHeader(MessageCode requestCode, uint dataSize)
        {
            byte[] headerBuf = new byte[MessageConstants.PbMsgHeaderSize];
            
            byte[] dataSizeBuf = BitConverter.GetBytes(dataSize);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(dataSizeBuf);
            }

            Debug.Assert(dataSizeBuf.Length == MessageConstants.PbMsgSizeLen, "Converting uint to byte[] resulted in incorrectly-sized buffer.");

            Buffer.BlockCopy(dataSizeBuf, 0, headerBuf, 0, dataSizeBuf.Length);

            headerBuf[MessageConstants.PbMsgSizeLen] = (byte)requestCode;

            return headerBuf;
        }

        public async Task WriteAsync()
        {
            RpbReq request = Command.ConstructPbRequest();
            if (request == null)
            {
                byte[] headerBuf = BuildHeader(Command.RequestCode, MessageConstants.PbMsgCodeSize);
                await Stream.WriteAsync(headerBuf, 0, MessageConstants.PbMsgHeaderSize);
            }
            else
            {
                await SerializeAndStreamAsync(request, Command.RequestCode, Stream);
            }
        }
    }
}