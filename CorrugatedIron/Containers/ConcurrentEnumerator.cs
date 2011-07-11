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

namespace CorrugatedIron.Containers
{
    public interface IConcurrentEnumerator<T>
    {
        bool TryMoveNext(out T next);
    }

    public sealed class ConcurrentEnumerator<T> : IConcurrentEnumerator<T>
    {
        private readonly object _lock = new object();
        private readonly IEnumerator<T> _wrapped;

        public ConcurrentEnumerator(IEnumerator<T> wrapped)
        {
            _wrapped = wrapped;
        }

        public bool TryMoveNext(out T next)
        {
            lock (_lock)
            {
                if (_wrapped.MoveNext())
                {
                    next = _wrapped.Current;
                    return true;
                }

                next = default(T);
                return false;
            }
        }
    }
}
