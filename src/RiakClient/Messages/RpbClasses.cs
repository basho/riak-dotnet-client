namespace RiakClient.Messages
{
    using System;
    using System.IO;
    using Erlang;
    using Exceptions;
    using ProtoBuf;
    using Util;

    public class RiakReq
    {
        private readonly MessageCode messageCode;
        private readonly bool isMessageCodeOnly = false;

        public RiakReq()
        {
            messageCode = MessageCodeTypeMapBuilder.GetMessageCodeFor(GetType());
        }

        public RiakReq(MessageCode messageCode)
        {
            this.messageCode = messageCode;
            isMessageCodeOnly = true;
        }

        public MessageCode MessageCode
        {
            get { return messageCode; }
        }

        public bool IsMessageCodeOnly
        {
            get { return isMessageCodeOnly; }
        }

        public virtual void WriteTo(Stream s)
        {
            Serializer.Serialize(s, this);
        }
    }

    public interface IRpbGeneratedKey
    {
        bool HasKey { get; }

        byte[] key { get; }
    }

    public abstract class RiakResp { }

    [CLSCompliant(false)]
    public sealed partial class RpbErrorResp { }

    public sealed partial class RpbGetServerInfoResp : RiakResp { }

    public sealed partial class RpbPair { }

    public sealed partial class RpbGetBucketReq { }

    [CLSCompliant(false)]
    public sealed partial class RpbGetBucketResp : RiakResp { }

    [CLSCompliant(false)]
    public sealed partial class RpbSetBucketReq : RiakReq { }

    public sealed partial class RpbSetBucketResp : RiakResp { }

    public sealed partial class RpbResetBucketReq { }

    public sealed partial class RpbGetBucketTypeReq : RiakReq { }

    [CLSCompliant(false)]
    public sealed partial class RpbSetBucketTypeReq : RiakReq { }

    public sealed partial class RpbModFun { }

    public sealed partial class RpbCommitHook { }

    [CLSCompliant(false)]
    public sealed partial class RpbBucketProps { }

    public sealed partial class RpbAuthReq { }

    public sealed partial class MapField { }

    public sealed partial class MapEntry { }

    [CLSCompliant(false)]
    public sealed partial class DtFetchReq : RiakReq { }

    public sealed partial class DtValue { }

    public sealed partial class DtFetchResp : RiakResp { }

    public sealed partial class CounterOp { }

    public sealed partial class SetOp { }

    public sealed partial class GSetOp { }

    public sealed partial class MapUpdate { }

    public sealed partial class MapOp { }

    public sealed partial class HllOp { }

    public sealed partial class DtOp { }

    [CLSCompliant(false)]
    public sealed partial class DtUpdateReq : RiakReq { }

    public sealed partial class DtUpdateResp : RiakResp, IRpbGeneratedKey
    {
        public bool HasKey
        {
            get { return EnumerableUtil.NotNullOrEmpty(key); }
        }
    }

    public sealed partial class RpbGetClientIdResp { }

    public sealed partial class RpbSetClientIdReq { }

    [CLSCompliant(false)]
    public sealed partial class RpbGetReq { }

    [CLSCompliant(false)]
    public sealed partial class RpbGetResp { }

    [CLSCompliant(false)]
    public sealed partial class RpbPutReq { }

    [CLSCompliant(false)]
    public sealed partial class RpbPutResp { }

    [CLSCompliant(false)]
    public sealed partial class RpbDelReq { }

    [CLSCompliant(false)]
    public sealed partial class RpbListBucketsReq { }

    public sealed partial class RpbListBucketsResp { }

    [CLSCompliant(false)]
    public sealed partial class RpbListKeysReq { }

    public sealed partial class RpbListKeysResp { }

    public sealed partial class RpbMapRedReq { }

    [CLSCompliant(false)]
    public sealed partial class RpbMapRedResp { }

    [CLSCompliant(false)]
    public sealed partial class RpbIndexReq { }

    public sealed partial class RpbIndexResp { }

    [CLSCompliant(false)]
    public sealed partial class RpbCSBucketReq { }

    [CLSCompliant(false)]
    public sealed partial class RpbCSBucketResp { }

    [CLSCompliant(false)]
    public sealed partial class RpbIndexObject { }

    [CLSCompliant(false)]
    public sealed partial class RpbContent { }

    public sealed partial class RpbLink { }

    [CLSCompliant(false)]
    public sealed partial class RpbCounterUpdateReq { }

    public sealed partial class RpbCounterUpdateResp { }

    [CLSCompliant(false)]
    public sealed partial class RpbCounterGetReq { }

    public sealed partial class RpbCounterGetResp { }

    public sealed partial class RpbSearchDoc { }

    [CLSCompliant(false)]
    public sealed partial class RpbSearchQueryReq { }

    [CLSCompliant(false)]
    public sealed partial class RpbSearchQueryResp { }

    [CLSCompliant(false)]
    public sealed partial class RpbYokozunaIndex { }

    public sealed partial class RpbYokozunaIndexGetReq { }

    [CLSCompliant(false)]
    public sealed partial class RpbYokozunaIndexGetResp { }

    [CLSCompliant(false)]
    public sealed partial class RpbYokozunaIndexPutReq { }

    public sealed partial class RpbYokozunaIndexDeleteReq { }

    public sealed partial class RpbYokozunaSchema { }

    public sealed partial class RpbYokozunaSchemaPutReq { }

    public sealed partial class RpbYokozunaSchemaGetReq { }

    public sealed partial class RpbYokozunaSchemaGetResp { }

    public sealed partial class RpbGetBucketKeyPreflistReq : RiakReq { }

    public sealed partial class RpbGetBucketKeyPreflistResp : RiakResp { }

    public sealed partial class RpbBucketKeyPreflistItem { }

    [CLSCompliant(false)]
    public sealed partial class RpbIndexBodyResp {}

    [CLSCompliant(false)]
    public sealed partial class RpbCoverageReq {}

    [CLSCompliant(false)]
    public sealed partial class RpbCoverageResp {}

    [CLSCompliant(false)]
    public sealed partial class RpbCoverageEntry {}

    [CLSCompliant(false)]
    public sealed partial class TsListKeysReq { }

    public sealed partial class TsListKeysResp { }

    [CLSCompliant(false)]
    public sealed partial class TsGetReq : RiakReq, ITsByKeyReq { }

    public sealed partial class TsGetResp : RiakResp { }

    public sealed partial class TsPutReq : RiakReq { }

    public sealed partial class TsPutResp : RiakResp { }

    public sealed partial class TsQueryReq : RiakReq { }

    public sealed partial class TsQueryResp : RiakResp, IRpbStreamingResp { }

    public sealed partial class TsListKeysReq : RiakReq { }

    public sealed partial class TsListKeysResp : RiakResp, IRpbStreamingResp { }

    [CLSCompliant(false)]
    public sealed partial class TsDelReq : RiakReq, ITsByKeyReq { }

    public sealed partial class TsDelResp : RiakResp { }

    [CLSCompliant(false)]
    public sealed partial class TsCoverageEntry { }

    public sealed partial class TsCoverageReq { }

    [CLSCompliant(false)]
    public sealed partial class TsCoverageResp { }

    public sealed partial class TsRange { }

    public sealed partial class TsInterpolation { }

    public sealed class TsTtbMsg : RiakReq
    {
        private readonly byte[] buffer;

        public TsTtbMsg(byte[] buffer)
        {
            if (EnumerableUtil.IsNullOrEmpty(buffer))
            {
                throw new ArgumentNullException("buffer");
            }

            this.buffer = buffer;
        }

        public override void WriteTo(Stream s)
        {
            s.Write(buffer, 0, buffer.Length);
        }
    }

    public sealed class TsTtbResp : RiakResp
    {
        private readonly byte[] response;

        public TsTtbResp(byte[] response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            this.response = response;
            RiakException ex = TtbErrorDecoder.MaybeRiakError(response);
            if (ex != null)
            {
                throw ex;
            }
        }

        public byte[] Response
        {
            get { return response; }
        }
    }
}
