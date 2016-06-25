namespace RiakClient.Messages
{
    using System;
    using System.Collections.Generic;

    internal static class MessageCodeTypeMapBuilder // TODO 3.0 - encapsulate in Command classes
    {
        private static readonly Dictionary<MessageCode, Type> MessageCodeToTypeMap;
        private static readonly Dictionary<Type, MessageCode> TypeToMessageCodeMap;

        static MessageCodeTypeMapBuilder()
        {
            MessageCodeToTypeMap = new Dictionary<MessageCode, Type>
            {
                { MessageCode.RpbErrorResp, typeof(RpbErrorResp) },
                { MessageCode.RpbGetClientIdResp, typeof(RpbGetClientIdResp) },
                { MessageCode.RpbSetClientIdReq, typeof(RpbSetClientIdReq) },
                { MessageCode.RpbGetServerInfoResp, typeof(RpbGetServerInfoResp) },
                { MessageCode.RpbGetReq, typeof(RpbGetReq) },
                { MessageCode.RpbGetResp, typeof(RpbGetResp) },
                { MessageCode.RpbPutReq, typeof(RpbPutReq) },
                { MessageCode.RpbPutResp, typeof(RpbPutResp) },
                { MessageCode.RpbDelReq, typeof(RpbDelReq) },
                { MessageCode.RpbListBucketsReq, typeof(RpbListBucketsReq) },
                { MessageCode.RpbListBucketsResp, typeof(RpbListBucketsResp) },
                { MessageCode.RpbListKeysReq, typeof(RpbListKeysReq) },
                { MessageCode.RpbListKeysResp, typeof(RpbListKeysResp) },
                { MessageCode.RpbGetBucketReq, typeof(RpbGetBucketReq) },
                { MessageCode.RpbGetBucketResp, typeof(RpbGetBucketResp) },
                { MessageCode.RpbSetBucketReq, typeof(RpbSetBucketReq) },
                { MessageCode.RpbMapRedReq, typeof(RpbMapRedReq) },
                { MessageCode.RpbMapRedResp, typeof(RpbMapRedResp) },
                { MessageCode.RpbIndexReq, typeof(RpbIndexReq) },
                { MessageCode.RpbIndexResp, typeof(RpbIndexResp) },
                { MessageCode.RpbSearchQueryReq, typeof(RpbSearchQueryReq) },
                { MessageCode.RpbSearchQueryResp, typeof(RpbSearchQueryResp) },
                { MessageCode.RpbResetBucketReq, typeof(RpbResetBucketReq) },
                { MessageCode.RpbCSBucketReq, typeof(RpbCSBucketReq) },
                { MessageCode.RpbCSBucketResp, typeof(RpbCSBucketResp) },
                { MessageCode.RpbIndexBodyResp, typeof(RpbIndexBodyResp) },
                { MessageCode.RpbCounterUpdateReq, typeof(RpbCounterUpdateReq) },
                { MessageCode.RpbCounterUpdateResp, typeof(RpbCounterUpdateResp) },
                { MessageCode.RpbCounterGetReq, typeof(RpbCounterGetReq) },
                { MessageCode.RpbCounterGetResp, typeof(RpbCounterGetResp) },

                { MessageCode.RpbYokozunaIndexGetReq, typeof(RpbYokozunaIndexGetReq) },
                { MessageCode.RpbYokozunaIndexGetResp, typeof(RpbYokozunaIndexGetResp) },
                { MessageCode.RpbYokozunaIndexPutReq, typeof(RpbYokozunaIndexPutReq) },
                { MessageCode.RpbYokozunaIndexDeleteReq, typeof(RpbYokozunaIndexDeleteReq) },
                { MessageCode.RpbYokozunaSchemaGetReq, typeof(RpbYokozunaSchemaGetReq) },
                { MessageCode.RpbYokozunaSchemaGetResp, typeof(RpbYokozunaSchemaGetResp) },
                { MessageCode.RpbYokozunaSchemaPutReq, typeof(RpbYokozunaSchemaPutReq) },

                { MessageCode.RpbCoverageReq, typeof(RpbCoverageReq) },
                { MessageCode.RpbCoverageResp, typeof(RpbCoverageResp) },

                { MessageCode.DtFetchReq, typeof(DtFetchReq) },
                { MessageCode.DtFetchResp, typeof(DtFetchResp) },
                { MessageCode.DtUpdateReq, typeof(DtUpdateReq) },
                { MessageCode.DtUpdateResp, typeof(DtUpdateResp) },

                { MessageCode.TsQueryReq, typeof(TsQueryReq) },
                { MessageCode.TsQueryResp, typeof(TsQueryResp) },
                { MessageCode.TsPutReq, typeof(TsPutReq) },
                { MessageCode.TsPutResp, typeof(TsPutResp) },
                { MessageCode.TsDelReq, typeof(TsDelReq) },
                { MessageCode.TsDelResp, typeof(TsDelResp) },
                { MessageCode.TsGetReq, typeof(TsGetReq) },
                { MessageCode.TsGetResp, typeof(TsGetResp) },
                { MessageCode.TsListKeysReq, typeof(TsListKeysReq) },
                { MessageCode.TsListKeysResp, typeof(TsListKeysResp) },
                { MessageCode.TsCoverageReq, typeof(TsCoverageReq) },
                { MessageCode.TsCoverageResp, typeof(TsCoverageResp) },
                { MessageCode.TsCoverageEntry, typeof(TsCoverageEntry) },
                { MessageCode.TsRange, typeof(TsRange) },

                // TTB pseudo-message
                { MessageCode.TsTtbMsg, typeof(TsTtbMsg) },

                { MessageCode.RpbGetBucketTypeReq, typeof(RpbGetBucketTypeReq) },
                { MessageCode.RpbSetBucketTypeReq, typeof(RpbSetBucketTypeReq) },
                { MessageCode.RpbAuthReq, typeof(RpbAuthReq) },

                { MessageCode.RpbGetBucketKeyPreflistReq, typeof(RpbGetBucketKeyPreflistReq) },
                { MessageCode.RpbGetBucketKeyPreflistResp, typeof(RpbGetBucketKeyPreflistResp) }
            };

            TypeToMessageCodeMap = new Dictionary<Type, MessageCode>();

            foreach (var item in MessageCodeToTypeMap)
            {
                TypeToMessageCodeMap.Add(item.Value, item.Key);
            }
        }

        public static MessageCode GetMessageCodeFor(Type type)
        {
            return TypeToMessageCodeMap[type];
        }

        public static Type GetTypeFor(MessageCode messageCode)
        {
            return MessageCodeToTypeMap[messageCode];
        }

        public static bool Contains(MessageCode messageCode)
        {
            return MessageCodeToTypeMap.ContainsKey(messageCode);
        }

        public static string GetTypeNameFor(MessageCode messageCode)
        {
            return MessageCodeToTypeMap[messageCode].Name;
        }
    }
}
