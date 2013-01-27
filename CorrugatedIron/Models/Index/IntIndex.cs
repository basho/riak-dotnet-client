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

using CorrugatedIron.Util;
using System.Collections.Generic;
using System.Linq;

namespace CorrugatedIron.Models.Index
{
    public class IntIndex : SecondaryIndex<IntIndex, int>
    {
        internal IntIndex(RiakObject container, string name)
            : base(container, name)
        {
        }

        protected override IntIndex TypedThis
        {
            get { return this; }
        }

        protected override string IndexSuffix
        {
            get { return RiakConstants.IndexSuffix.Integer; }
        }

        public IntIndex Set(params string[] values)
        {
            return Set(values.Select(int.Parse));
        }

        public IntIndex Set(IEnumerable<string> values)
        {
            return Set(values.Select(int.Parse));
        }

        public IntIndex Add(params string[] values)
        {
            return Add(values.Select(int.Parse));
        }

        public IntIndex Add(IEnumerable<string> values)
        {
            return Add(values.Select(int.Parse));
        }

        public IntIndex Remove(params string[] values)
        {
            return Remove(values.Select(int.Parse));
        }

        public IntIndex Remove(IEnumerable<string> values)
        {
            return Remove(values.Select(int.Parse));
        }

        public RiakObject Delete()
        {
            Container.IntIndexes.Remove(Name);
            return Container;
        }
    }
}