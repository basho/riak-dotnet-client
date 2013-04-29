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

using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorrugatedIron
{
    public interface IRiakBatchAsyncClient
    {
        int RetryCount { get; set; }
        Task<RiakResult> Ping();

        Task<RiakResult<RiakObject>> Get(string bucket, string key, RiakGetOptions options = null);
        Task<RiakResult<RiakObject>> Get(string bucket, string key, uint rVal = RiakConstants.Defaults.RVal);
        Task<RiakResult<RiakObject>> Get(RiakObjectId objectId, uint rVal = RiakConstants.Defaults.RVal);
        Task<IEnumerable<RiakResult<RiakObject>>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, uint rVal = RiakConstants.Defaults.RVal);

        Task<RiakResult<RiakObject>> Put(RiakObject value, RiakPutOptions options = null);
        Task<IEnumerable<RiakResult<RiakObject>>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null);

        Task<RiakResult> Delete(string bucket, string key, RiakDeleteOptions options = null);
        Task<RiakResult> Delete(RiakObjectId objectId, RiakDeleteOptions options = null);
        Task<IEnumerable<RiakResult>> Delete(IEnumerable<RiakObjectId> objectIds, RiakDeleteOptions options = null);

        Task<IEnumerable<RiakResult>> DeleteBucket(string bucket, RiakDeleteOptions options = null);
        Task<IEnumerable<RiakResult>> DeleteBucket(string bucket, uint rwVal = RiakConstants.Defaults.RVal);

        Task<RiakResult<RiakSearchResult>> Search(RiakSearchRequest search);

        Task<RiakResult<RiakMapReduceResult>> MapReduce(RiakMapReduceQuery query);

        Task<RiakResult<RiakStreamedMapReduceResult>> StreamMapReduce(RiakMapReduceQuery query);

        Task<RiakResult<IEnumerable<string>>> ListBuckets();

        Task<RiakResult<IEnumerable<string>>> ListKeys(string bucket);

        Task<RiakResult<IEnumerable<string>>> StreamListKeys(string bucket);

        Task<RiakResult<RiakBucketProperties>> GetBucketProperties(string bucket, bool extended = false);

        Task<RiakResult> SetBucketProperties(string bucket, RiakBucketProperties properties);

        Task<RiakResult> ResetBucketProperties(string bucket);

        Task<RiakResult<IList<RiakObject>>> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks);

        Task<RiakResult<RiakServerInfo>> GetServerInfo();

        Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, int value);
        Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, string value);
        Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, int minValue, int maxValue);
        Task<RiakResult<IList<string>>> IndexGet(string bucket, string indexName, string minValue, string maxValue);

        Task<RiakResult<IList<string>>> ListKeysFromIndex(string bucket);

        //RiakResult<RiakSearchResult> Search(Action<RiakSearchRequest> prepareRequest)
    }
}