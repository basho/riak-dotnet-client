// <copyright file="Either.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Containers
{
    public class Either<TLeft, TRight>
    {
        private readonly bool isLeft;
        private readonly TLeft left;
        private readonly TRight right;

        public Either(TLeft left)
        {
            this.left = left;
            isLeft = true;
        }

        public Either(TRight right)
        {
            this.right = right;
            isLeft = false;
        }

        public bool IsLeft
        {
            get { return isLeft; }
        }

        public TLeft Left
        {
            get { return left; }
        }

        public TRight Right
        {
            get { return right; }
        }
    }
}
