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

using System.Collections.Generic;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Util;

namespace CorrugatedIron
{
    public interface IRiakBatchClient
    {
        RiakResult Ping();

        RiakResult<RiakObject> Get(string bucket, string key, uint rVal = RiakConstants.Defaults.RVal);
        RiakResult<RiakObject> Get(RiakObjectId objectId, uint rVal = RiakConstants.Defaults.RVal);
        IEnumerable<RiakResult<RiakObject>> Get(IEnumerable<RiakObjectId> bucketKeyPairs, uint rVal = RiakConstants.Defaults.RVal);

        RiakResult<RiakObject> Put(RiakObject value, RiakPutOptions options = null);
        IEnumerable<RiakResult<RiakObject>> Put(IEnumerable<RiakObject> values, RiakPutOptions options = null);

        RiakResult Delete(string bucket, string key, uint rwVal = RiakConstants.Defaults.RVal);
        RiakResult Delete(RiakObjectId objectId, uint rwVal = RiakConstants.Defaults.RVal);
        IEnumerable<RiakResult> Delete(IEnumerable<RiakObjectId> objectIds, uint rwVal = RiakConstants.Defaults.RVal);

        IEnumerable<RiakResult> DeleteBucket(string bucket, uint rwVal = RiakConstants.Defaults.RVal);

        RiakResult<RiakMapReduceResult> MapReduce(RiakMapReduceQuery query);

        RiakResult<RiakStreamedMapReduceResult> StreamMapReduce(RiakMapReduceQuery query);

        RiakResult<IEnumerable<string>> ListBuckets();

        RiakResult<IEnumerable<string>> ListKeys(string bucket);

        RiakResult<IEnumerable<string>> StreamListKeys(string bucket);

        RiakResult<RiakBucketProperties> GetBucketProperties(string bucket, bool extended = false);

        RiakResult SetBucketProperties(string bucket, RiakBucketProperties properties);

        IList<RiakObject> WalkLinks(RiakObject riakObject, IList<RiakLink> riakLinks);

        RiakResult<RiakServerInfo> GetServerInfo();
    }
}
