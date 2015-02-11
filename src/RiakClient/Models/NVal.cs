// <copyright file="NVal.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models
{
    using System;

    public class NVal : IEquatable<NVal>
    {
        private readonly uint nval = 0;

        public NVal(int nval)
            : this((uint)nval)
        {
        }

        internal NVal(uint nval)
        {
            if (nval <= 0)
            {
                throw new ArgumentOutOfRangeException("nval must be greater than zero");
            }

            this.nval = (uint)nval;
        }

        public static explicit operator NVal(int nval)
        {
            return new NVal(nval);
        }

        public static explicit operator int(NVal nval)
        {
            return (int)nval.nval;
        }

        [CLSCompliant(false)]
        public static implicit operator uint(NVal nval)
        {
            return nval.nval;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NVal);
        }

        public bool Equals(NVal other)
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
            return nval.GetHashCode();
        }
    }
}