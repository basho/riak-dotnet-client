/*
 *
 * Copyright Ericsson AB 2000-2016. All Rights Reserved.
 * Copyright Basho Technologies, Inc. 2016. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

namespace RiakClient.Erlang
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Provides a stream for decoding Erlang terms from external format.
    /// </summary>
    public class OtpInputStream : MemoryStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OtpInputStream" /> class
        /// from a buffer containing encoded Erlang terms.
        /// </summary>
        /// <param name="buffer">The buffer containing Erlang terms.</param>
        public OtpInputStream(byte[] buffer)
            : base(buffer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OtpInputStream" /> class
        /// from a buffer containing encoded Erlang terms at the given
        /// offset and length.
        /// </summary>
        /// <param name="buffer">The buffer containing Erlang terms.</param>
        /// <param name="index">The index into buffer at which the stream begins.</param>
        /// <param name="count">The length of the stream in bytes.</param>
        public OtpInputStream(byte[] buffer, int index, int count)
            : base(buffer, index, count)
        {
        }

        /// <summary>
        /// Read an array of bytes from the stream into the buffer.
        /// The method reads at most buffer.Length bytes from the input stream.
        /// </summary>
        /// <param name="buffer">The buffer into which to read data.</param>
        /// <returns>The number of bytes read.</returns>
        public int ReadN(byte[] buffer)
        {
            return ReadN(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Read an array of bytes from the stream. The method reads at most len
        /// bytes from the input stream into offset off of the buffer.
        /// </summary>
        /// <param name="buffer">The buffer into which to read data.</param>
        /// <param name="offset">The offset in the buffer into which to read data.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes read.</returns>
        public int ReadN(byte[] buffer, int offset, int count)
        {
            if ((count == 0) && (Position < Length))
            {
                return 0;
            }

            int i = Read(buffer, offset, count);
            if (i <= 0)
            {
                throw new Exception("Cannot read from input stream");
            }

            if (i != count)
            {
                string ex = string.Format(
                    "Expected to read {0} bytes, only read {1}",
                    count,
                    i);
                throw new Exception(ex);
            }

            return i;
        }

        /// <summary>
        /// Look ahead one position in the stream without consuming the byte found there.
        /// </summary>
        /// <returns>The next byte in the stream.</returns>
        public byte Peek()
        {
            return Peek1();
        }

        /// <summary>
        /// Look ahead one position in the stream without consuming the byte found there.
        /// </summary>
        /// <returns>The next byte in the stream.</returns>
        public byte Peek1()
        {
            try
            {
                return Read1();
            }
            finally
            {
                Position--;
            }
        }

        /// <summary>
        /// Look ahead one position in the stream, skipping the version tag if it's read.
        /// </summary>
        /// <returns>The next byte from the stream.</returns>
        public byte Peek1SkipVersion()
        {
            try
            {
                byte tag = Read1();
                if (tag == OtpExternal.VersionTag)
                {
                    tag = Read1();
                }

                return tag;
            }
            finally
            {
                Position--;
            }
        }

        /// <summary>
        /// Read one byte from the stream.
        /// </summary>
        /// <returns>The next byte from the stream.</returns>
        public byte Read1()
        {
            int i = ReadByte();
            if (i < 0)
            {
                throw new Exception("Cannot read from input stream");
            }

            return (byte)i;
        }

        /// <summary>
        /// Read one byte from the stream, skipping the version tag if it's read.
        /// </summary>
        /// <returns>The next byte from the stream.</returns>
        public byte Read1SkipVersion()
        {
            byte tag = Read1();
            if (tag == OtpExternal.VersionTag)
            {
                tag = Read1();
            }

            return tag;
        }

        /// <summary>
        /// Read a two byte big endian integer from the stream.
        /// </summary>
        /// <returns>The bytes read, converted from big endian to an integer.</returns>
        public int Read2BE()
        {
            byte b0 = Read1();
            byte b1 = Read1();
            return (b0 << 8) + b1;
        }

        /// <summary>
        /// Read a four byte big endian integer from the stream.
        /// </summary>
        /// <returns>The bytes read, converted from big endian to an integer.</returns>
        public int Read4BE()
        {
            byte b0 = Read1();
            byte b1 = Read1();
            byte b2 = Read1();
            byte b3 = Read1();
            return (b0 << 24) + (b1 << 16) + (b2 << 8) + b3;
        }

        /// <summary>
        /// Read a bigendian integer from the stream.
        /// </summary>
        /// <param name="n">The number of bytes to read</param>
        /// <returns>The bytes read, converted from big endian to an integer.</returns>
        public long ReadBE(int n)
        {
            byte b = 0;
            long v = 0;
            for (int i = 0; i < n; i++)
            {
                b = Read1();
                v = v << 8 | b;
            }

            return v;
        }

        /// <summary>
        /// Read an Erlang atom from the stream and interpret the value as a boolean.
        /// </summary>
        /// <returns>true if the atom at the current position in the stream contains
        /// the value 'true' (ignoring case), false otherwise.</returns>
        public bool ReadBoolean()
        {
            string atom = ReadAtom();
            return bool.Parse(atom);
        }

        /// <summary>
        /// Read an Erlang atom from the stream.
        /// </summary>
        /// <returns>string containing the value of the atom.</returns>
        public string ReadAtom()
        {
            string atom;
            byte tag = Read1SkipVersion();
            switch (tag)
            {
                case OtpExternal.AtomTag:
                    int len = Read2BE();
                    byte[] buffer = new byte[len];
                    ReadN(buffer);
                    atom = OtpUtils.Latin1Encoding.GetString(buffer);
                    if (atom.Length > OtpExternal.MaxAtomLength)
                    {
                        /*
                         * Throwing an exception would be better I think, but truncation
                         * seems to be the way it has been done in other parts of OTP...
                         */
                        atom = atom.Substring(0, OtpExternal.MaxAtomLength);
                    }

                    break;
                default:
                    throw OnBadTag(tag, OtpExternal.AtomTag);
            }

            return atom;
        }

        /// <summary>
        /// Read an Erlang binary from the stream and converts to a UTF-8 string.
        /// </summary>
        /// <returns> A string containing the value of the binary.</returns>
        public string ReadBinaryAsString()
        {
            byte[] bin = ReadBinary();
            return Encoding.UTF8.GetString(bin);
        }

        /// <summary>
        /// Read an Erlang binary from the stream.
        /// </summary>
        /// <returns> A byte array containing the value of the binary.</returns>
        public byte[] ReadBinary()
        {
            byte tag = Read1SkipVersion();
            if (tag != OtpExternal.BinTag)
            {
                throw OnBadTag(tag, OtpExternal.BinTag);
            }

            int len = Read4BE();
            byte[] bin = new byte[len];
            ReadN(bin);
            return bin;
        }

        /// <summary>
        /// Read an Erlang float from the stream.
        /// </summary>
        /// <returns>The float value, as a double.</returns>
        public double ReadDouble()
        {
            double d;
            byte tag = Read1SkipVersion();
            switch (tag)
            {
                case OtpExternal.FloatTag:
                    byte[] b = new byte[OtpExternal.FloatByteLength];
                    ReadN(b);
                    string ds = Encoding.ASCII.GetString(b);
                    NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite;
                    if (!double.TryParse(ds, style, NumberFormatInfo.InvariantInfo, out d))
                    {
                        throw new InvalidOperationException(string.Format("invalid float encoding: \"{0}\"", ds));
                    }

                    break;
                case OtpExternal.NewFloatTag:
                    d = BitConverter.Int64BitsToDouble(ReadBE(8));
                    break;
                default:
                    throw OnBadTag(tag, OtpExternal.NewFloatTag);
            }

            return d;
        }

        /// <summary>
        /// Can the tag be parsed as a long
        /// </summary>
        /// <param name="tag">the tag to check</param>
        /// <returns>boolean indicating if tag can be parsed as a long.</returns>
        public bool IsLongTag(byte tag)
        {
            switch (tag)
            {
                case OtpExternal.SmallIntTag:
                case OtpExternal.IntTag:
                case OtpExternal.SmallBigTag:
                case OtpExternal.LargeBigTag:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Read an array of bytes
        /// </summary>
        /// <returns>the value as a long.</returns>
        public long ReadLong()
        {
            byte[] nb;
            byte tag = Read1SkipVersion();
            switch (tag)
            {
                case OtpExternal.SmallIntTag:
                    return Read1();
                case OtpExternal.IntTag:
                    nb = new byte[4];
                    ReadN(nb); // Big endian
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(nb);
                    }

                    return BitConverter.ToInt32(nb, 0);
                case OtpExternal.SmallBigTag:
                case OtpExternal.LargeBigTag:
                    int arity;
                    byte sign;
                    if (tag == OtpExternal.SmallBigTag)
                    {
                        arity = Read1();
                        sign = Read1();
                    }
                    else
                    {
                        arity = Read4BE();
                        sign = Read1();
                    }

                    if (arity > 8)
                    {
                        // FUTURE: support BigInteger
                        throw new Exception("integer is too big to decode");
                    }

                    nb = new byte[8];

                    // Value is little endian.
                    ReadN(nb, 0, arity);

                    if (!BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(nb);
                    }

                    ulong val = BitConverter.ToUInt64(nb, 0);

                    if (sign == OtpExternal.Negative)
                    {
                        return -(long)val;
                    }

                    return (long)val;
                default:
                    throw new Exception("Not valid integer tag: " + tag);
            }
        }

        /// <summary>
        /// Read a list header from the stream.
        /// </summary>
        /// <returns>the arity of the list.</returns>
        public int ReadListHead()
        {
            int arity = 0;
            byte tag = Read1SkipVersion();
            switch (tag)
            {
                case OtpExternal.NilTag:
                    arity = 0;
                    break;
                case OtpExternal.ListTag:
                    arity = Read4BE();
                    break;
                default:
                    throw OnBadTag(
                        tag,
                        OtpExternal.NilTag,
                        OtpExternal.ListTag);
            }

            return arity;
        }

        /// <summary>
        /// Read a tuple header from the stream.
        /// </summary>
        /// <returns>the arity of the tuple.</returns>
        public int ReadTupleHead()
        {
            int arity = 0;
            byte tag = Read1SkipVersion();

            // decode the tuple header and get arity
            switch (tag)
            {
                case OtpExternal.SmallTupleTag:
                    arity = Read1();
                    break;
                case OtpExternal.LargeTupleTag:
                    arity = Read4BE();
                    break;
                default:
                    throw OnBadTag(
                        tag,
                        OtpExternal.SmallTupleTag,
                        OtpExternal.LargeTupleTag);
            }

            return arity;
        }

        // Read an empty list from the stream.
        // @return zero (the arity of the list).
        public int ReadNil()
        {
            int arity = 0;
            byte tag = Read1SkipVersion();

            switch (tag)
            {
                case OtpExternal.NilTag:
                    arity = 0;
                    break;
                default:
                    throw OnBadTag(tag, OtpExternal.NilTag);
            }

            return arity;
        }

        private static Exception OnBadTag(byte got, params byte[] want)
        {
            string exmsg = null;
            if (want.Length == 1)
            {
                exmsg = string.Format(
                    "wrong tag encountered, expected {0}, got {1}",
                    want,
                    got);
            }
            else
            {
                exmsg = string.Format(
                    "wrong tag encountered, expected one of {0}, got {1}",
                    string.Join(", ", want),
                    got);
            }

            return new Exception(exmsg);
        }
    }
}
