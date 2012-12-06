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

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public static class RiakIndex
    {
        public static RiakIndexInput Match(string bucket, string index, string key)
        {
            return new RiakBinIndexEqualityInput(bucket, index, key);
        }

        public static RiakIndexInput Range(string bucket, string index, string start, string end)
        {
            return new RiakBinIndexRangeInput(bucket, index, start, end);
        }

        public static RiakIndexInput Match(string bucket, string index, int key)
        {
            return new RiakIntIndexEqualityInput(bucket, index, key);
        }

        public static RiakIndexInput Range(string bucket, string index, int start, int end)
        {
            return new RiakIntIndexRangeInput(bucket, index, start, end);
        }
    }
}
