/*
 * Copyright Ericsson AB 2000-2016. All Rights Reserved.
 * Copyright Basho Technologies; Inc. 2016. All Rights Reserved.
 *
 * Licensed under the Apache License; Version 2.0 (the "License"),
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing; software
 * distributed under the License is distributed on an "AS IS" BASIS;
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND; either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
namespace RiakClient.Erlang
{
    /**
     * Provides a collection of constants used when encoding and decoding Erlang
     * terms.
     */
    internal static class OtpExternal
    {
        public const byte FloatByteLength = 31;

        public const byte Negative = 1;
        public const byte Positive = 0;

        /** The longest allowed Erlang atom */
        public const byte MaxAtomLength = 255;

        /** The tag used for small integers */
        public const byte SmallIntTag = 97;

        /** The tag used for integers */
        public const byte IntTag = 98;

        /** The tag used for floating point numbers */
        public const byte FloatTag = 99;
        public const byte NewFloatTag = 70;

        /** The tag used for atoms */
        public const byte AtomTag = 100;

        /** The tag used for small tuples */
        public const byte SmallTupleTag = 104;

        /** The tag used for large tuples */
        public const byte LargeTupleTag = 105;

        /** The tag used for empty lists */
        public const byte NilTag = 106;

        /** The tag used for non-empty lists */
        public const byte ListTag = 108;

        /** The tag used for binaries */
        public const byte BinTag = 109;

        /** The tag used for small bignums */
        public const byte SmallBigTag = 110;

        /** The tag used for large bignums */
        public const byte LargeBigTag = 111;

        /** The version number used to mark serialized Erlang terms */
        public const byte VersionTag = 131;
    }
}