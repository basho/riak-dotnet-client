// <copyright file="MessageCode.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

#pragma warning disable 1591
namespace RiakClient.Messages
{
    /// <summary>
    /// An enumeration of the different protocol buffer message codes.
    /// </summary>
    public enum MessageCode : byte
    {
        /// <summary>
        /// Generic Error Response.
        /// </summary>
        RpbErrorResp = 0,

        /// <summary>
        /// Ping Request.
        /// </summary>
        RpbPingReq = 1,

        /// <summary>
        /// Ping Response.
        /// </summary>
        RpbPingResp = 2,

        /// <summary>
        /// Get Client Id Request.
        /// </summary>
        /// <remarks>Not used anymore.</remarks>
        RpbGetClientIdReq = 3,

        /// <summary>
        /// Get Client Id Response.
        /// </summary>
        /// <remarks>Not used anymore.</remarks>
        RpbGetClientIdResp = 4,

        /// <summary>
        /// Set Client Id Request.
        /// </summary>
        /// <remarks>Not used anymore.</remarks>
        RpbSetClientIdReq = 5,

        /// <summary>
        /// Set Client Id Response.
        /// </summary>
        /// <remarks>Not used anymore.</remarks>
        RpbSetClientIdResp = 6,

        /// <summary>
        /// Get Server Info Request.
        /// </summary>
        RpbGetServerInfoReq = 7,

        /// <summary>
        /// Get Server Info Response.
        /// </summary>
        RpbGetServerInfoResp = 8,

        /// <summary>
        /// Get Riak Object Request.
        /// </summary>
        RpbGetReq = 9,

        /// <summary>
        /// Get Riak Object Response.
        /// </summary>
        RpbGetResp = 10,

        /// <summary>
        /// Put Riak Object Request.
        /// </summary>
        RpbPutReq = 11,

        /// <summary>
        /// Put Riak Object Response.
        /// </summary>
        RpbPutResp = 12,

        /// <summary>
        /// Delete Riak Object Request.
        /// </summary>
        RpbDelReq = 13,

        /// <summary>
        /// Delete Riak Object Response.
        /// </summary>
        RpbDelResp = 14,

        /// <summary>
        /// List Buckets Request.
        /// </summary>
        RpbListBucketsReq = 15,

        /// <summary>
        /// List Buckets Response.
        /// </summary>
        RpbListBucketsResp = 16,

        /// <summary>
        /// List Keys Request.
        /// </summary>
        RpbListKeysReq = 17,

        /// <summary>
        /// List Keys Response.
        /// </summary>
        RpbListKeysResp = 18,

        /// <summary>
        /// Get Bucket Properties Request.
        /// </summary>
        RpbGetBucketReq = 19,

        /// <summary>
        /// Get Bucket Properties Response.
        /// </summary>
        RpbGetBucketResp = 20,

        /// <summary>
        /// Set Bucket Properties Request.
        /// </summary>
        RpbSetBucketReq = 21,

        /// <summary>
        /// Set Bucket Properties Response.
        /// </summary>
        RpbSetBucketResp = 22,

        /// <summary>
        /// MapReduce Query Request.
        /// </summary>
        RpbMapRedReq = 23,

        /// <summary>
        /// MapReduce Query Response.
        /// </summary>
        RpbMapRedResp = 24,

        /// <summary>
        /// Secondary Index Query Request.
        /// </summary>
        RpbIndexReq = 25,

        /// <summary>
        /// Secondary Index Query Response.
        /// </summary>
        RpbIndexResp = 26,

        /// <summary>
        /// Search Query Request.
        /// </summary>
        RpbSearchQueryReq = 27,

        /// <summary>
        /// Search Query Response.
        /// </summary>
        RpbSearchQueryResp = 28,

        /// <summary>
        /// Reset Bucket Properties Request.
        /// </summary>
        RpbResetBucketReq = 29,

        /// <summary>
        /// Reset Bucket Properties Response.
        /// </summary>
        RpbResetBucketResp = 30,

        /// <summary>
        /// Get Bucket Type Request.
        /// </summary>
        RpbGetBucketTypeReq = 31,

        /// <summary>
        /// Set Bucket Type Request.
        /// </summary>
        /// <remarks>Not used.</remarks>
        RpbSetBucketTypeReq = 32,

        /// <summary>
        /// Get bucket key preflist request.
        /// </summary>
        RpbGetBucketKeyPreflistReq = 33,

        /// <summary>
        /// Get bucket key preflist response.
        /// </summary>
        RpbGetBucketKeyPreflistResp = 34,

        /// <summary>
        /// Riak CS Bucket Request.
        /// </summary>
        RpbCSBucketReq = 40,

        /// <summary>
        /// Riak CS Bucket Response.
        /// </summary>
        RpbCSBucketResp = 41,

        /// <summary>
        /// Riak 1.4 Counter Update Request.
        /// </summary>
        RpbCounterUpdateReq = 50,

        /// <summary>
        /// Riak 1.4 Counter Update Response.
        /// </summary>
        RpbCounterUpdateResp = 51,

        /// <summary>
        /// Riak 1.4 Counter Get Request.
        /// </summary>
        RpbCounterGetReq = 52,

        /// <summary>
        /// Riak 1.4 Counter Get Response.
        /// </summary>
        RpbCounterGetResp = 53,

        /// <summary>
        /// Yokozuna Index Get Request.
        /// </summary>
        RpbYokozunaIndexGetReq = 54,

        /// <summary>
        /// Yokozuna Index Get Response.
        /// </summary>
        RpbYokozunaIndexGetResp = 55,

        /// <summary>
        /// Yokozuna Index Put Request.
        /// </summary>
        RpbYokozunaIndexPutReq = 56,

        /// <summary>
        /// Yokozuna Index Delete Request.
        /// </summary>
        RpbYokozunaIndexDeleteReq = 57,

        /// <summary>
        /// Yokozuna Schema Get Request.
        /// </summary>
        RpbYokozunaSchemaGetReq = 58,

        /// <summary>
        /// Yokozuna Schema Get Response.
        /// </summary>
        RpbYokozunaSchemaGetResp = 59,

        /// <summary>
        /// Yokozuna Schema Put Request.
        /// </summary>
        RpbYokozunaSchemaPutReq = 60,

        /// <summary>
        /// DataType Fetch Request.
        /// </summary>
        DtFetchReq = 80,

        /// <summary>
        /// DataType Fetch Response.
        /// </summary>
        DtFetchResp = 81,

        /// <summary>
        /// DataType Update Request.
        /// </summary>
        DtUpdateReq = 82,

        /// <summary>
        /// DataType Update Response.
        /// </summary>
        DtUpdateResp = 83,

        /// <summary>
        /// Authentication Request.
        /// </summary>
        RpbAuthReq = 253,

        /// <summary>
        /// Authentication Response.
        /// </summary>
        RpbAuthResp = 254,

        /// <summary>
        /// Start TLS Session Request.
        /// </summary>
        RpbStartTls = 255
    }
}