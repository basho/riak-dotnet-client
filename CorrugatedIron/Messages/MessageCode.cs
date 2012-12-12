// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

namespace CorrugatedIron.Messages
{
    public enum MessageCode : byte
    {
        ErrorResp = 0,
        PingReq = 1,
        PingResp = 2,
        GetClientIdReq = 3,     // Not used any more
        GetClientIdResp = 4,    // Not used any more
        SetClientIdReq = 5,     // Not used any more
        SetClientIdResp = 6,    // Not used any more
        GetServerInfoReq = 7,
        GetServerInfoResp = 8,
        GetReq = 9,
        GetResp = 10,
        PutReq = 11,
        PutResp = 12,
        DelReq = 13,
        DelResp = 14,
        ListBucketsReq = 15,
        ListBucketsResp = 16,
        ListKeysReq = 17,
        ListKeysResp = 18,
        GetBucketReq = 19,
        GetBucketResp = 20,
        SetBucketReq = 21,
        SetBucketResp = 22,
        MapRedReq = 23,
        MapRedResp = 24,
        IndexReq = 25,
        IndexResp = 26,
        SearchQueryReq = 27,
        SearchQueryResp = 28
    }
}
