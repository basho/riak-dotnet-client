namespace Riak.Core
{
    using System;
    using System.IO;
    using ProtoBuf;
    using RiakClient.Commands;
    using RiakClient.Messages;

    internal class MessageDecoder
    {
        private readonly IRCommand command;
        private readonly byte[] data;

        public MessageDecoder(IRCommand command, byte[] data)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data", Properties.Resources.Riak_Core_MessageDataCantBeNull);
            }

            if (data.Length == 0)
            {
                throw new ConnectionException(Properties.Resources.Riak_Core_ConnectionZeroLengthResponseException);
            }

            this.command = command;
            this.data = data;
        }

        public RError MaybeError()
        {
            RError error = null;

            if (data[0] == (byte)MessageCode.RpbErrorResp)
            {
                RpbErrorResp rpbErrorResp = null;

                int dataSize = data.Length - MessageConstants.PbMsgCodeSize;
                using (var memStream = new MemoryStream(data, MessageConstants.PbMsgCodeSize, dataSize))
                {
                    rpbErrorResp = Serializer.Deserialize<RpbErrorResp>(memStream);
                }

                error = new RError(rpbErrorResp);
            }

            return error;
        }

        public RpbResp DecodeMessage()
        {
            ValidateResponseCode();

            Type responseType = command.ResponseType;
            if (responseType == null)
            {
                if (data.Length <= MessageConstants.PbMsgCodeSize)
                {
                    return Activator.CreateInstance(command.ResponseType) as RpbResp;
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(
                            Properties.Resources.Riak_Core_CommandHasNoResponseTypeButDataPresentException_fmt,
                            command.GetType().Name,
                            data.Length));
                }
            }

            if (data.Length <= MessageConstants.PbMsgCodeSize)
            {
                throw new InvalidOperationException(
                    string.Format(
                        Properties.Resources.Riak_Core_CommandHasResponseTypeButNoDataPresentException_fmt,
                        command.GetType().Name,
                        data.Length));
            }
            else
            {
                using (var memStream = new MemoryStream(data, MessageConstants.PbMsgCodeSize, data.Length - MessageConstants.PbMsgCodeSize))
                {
                    return Serializer.NonGeneric.Deserialize(responseType, memStream) as RpbResp;
                }
            }
        }

        private void ValidateResponseCode()
        {
            byte expectedCode = (byte)command.ResponseCode;
            if (expectedCode == 0)
            {
                throw new InvalidOperationException(
                    string.Format(Properties.Resources.Riak_Core_MessageResponseCodeIsZeroException_fmt, command.GetType().Name));
            }

            byte actualCode = data[0];
            if (expectedCode != actualCode)
            {
                var message = string.Format(
                    Properties.Resources.Riak_Core_MessageUnexpectedResponseCodeException_fmt,
                    expectedCode,
                    actualCode);
                throw new ConnectionException(message);
            }
        }
    }
}
