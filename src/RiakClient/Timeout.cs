// <copyright file="Timeout.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient
{
    using System;

    public class Timeout : IEquatable<Timeout>
    {
        private readonly TimeSpan timeout = default(TimeSpan);

        public Timeout(TimeSpan timeout)
        {
            this.timeout = timeout;
        }

        public Timeout(int milliseconds)
        {
            if (milliseconds < 0)
            {
                throw new ArgumentOutOfRangeException("milliseconds must be greater than or equal to zero");
            }

            this.timeout = TimeSpan.FromMilliseconds(milliseconds);
        }

        public static explicit operator Timeout(int timeout)
        {
            return new Timeout(timeout);
        }

        public static explicit operator int(Timeout timeout)
        {
            return (int)timeout.timeout.TotalMilliseconds;
        }

        public static implicit operator TimeSpan(Timeout timeout)
        {
            return timeout.timeout;
        }

        public static implicit operator string(Timeout timeout)
        {
            return timeout.timeout.TotalMilliseconds.ToString();
        }

        [CLSCompliant(false)]
        public static explicit operator uint(Timeout timeout)
        {
            return (uint)timeout.timeout.TotalMilliseconds;
        }

        public override string ToString()
        {
            return timeout.TotalMilliseconds.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Timeout);
        }

        public bool Equals(Timeout other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (object.ReferenceEquals(other, this))
            {
                return true;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return timeout.GetHashCode();
        }
    }
}