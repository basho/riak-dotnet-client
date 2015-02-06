// <copyright file="MessageCode.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

namespace RiakClient.Messages
{
    public enum MessageCode : byte
    {
        RpbErrorResp = 0,
        RpbPingReq = 1,
        RpbPingResp = 2,
        RpbGetClientIdReq = 3,     // Not used any more
        RpbGetClientIdResp = 4,    // Not used any more
        RpbSetClientIdReq = 5,     // Not used any more
        RpbSetClientIdResp = 6,    // Not used any more
        RpbGetServerInfoReq = 7,
        RpbGetServerInfoResp = 8,
        RpbGetReq = 9,
        RpbGetResp = 10,
        RpbPutReq = 11,
        RpbPutResp = 12,
        RpbDelReq = 13,
        RpbDelResp = 14,
        RpbListBucketsReq = 15,
        RpbListBucketsResp = 16,
        RpbListKeysReq = 17,
        RpbListKeysResp = 18,
        RpbGetBucketReq = 19,
        RpbGetBucketResp = 20,
        RpbSetBucketReq = 21,
        RpbSetBucketResp = 22,
        RpbMapRedReq = 23,
        RpbMapRedResp = 24,
        RpbIndexReq = 25,
        RpbIndexResp = 26,
        RpbSearchQueryReq = 27,
        RpbSearchQueryResp = 28,
        RpbResetBucketReq = 29,
        RpbResetBucketResp = 30,
        RpbGetBucketTypeReq = 31,
        RpbSetBucketTypeReq = 32,
        RpbCSBucketReq = 40,
        RpbCSBucketResp = 41,
        RpbCounterUpdateReq = 50,
        RpbCounterUpdateResp = 51,
        RpbCounterGetReq = 52,
        RpbCounterGetResp = 53,
        RpbYokozunaIndexGetReq = 54,
        RpbYokozunaIndexGetResp = 55,
        RpbYokozunaIndexPutReq = 56,
        RpbYokozunaIndexDeleteReq = 57,
        RpbYokozunaSchemaGetReq = 58,
        RpbYokozunaSchemaGetResp = 59,
        RpbYokozunaSchemaPutReq = 60,
        DtFetchReq = 80,
        DtFetchResp = 81,
        DtUpdateReq = 82,
        DtUpdateResp = 83,
        RpbAuthReq = 253,
        RpbAuthResp = 254,
        RpbStartTls = 255
    }
}
