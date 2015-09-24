namespace Riak.Core
{
    public static class MessageConstants
    {
        public const byte PbMsgSizeLen = sizeof(uint);
        public const byte PbMsgCodeSize = sizeof(byte);
        public const byte PbMsgHeaderSize = PbMsgSizeLen + PbMsgCodeSize;
    }
}