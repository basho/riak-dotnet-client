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
    using System.IO;
    using System.Text;
    /**
     * Provides a stream for encoding Erlang terms to external format, for
     * transmission or storage.
     *
     * <p>
     * Note that this class is not synchronized, if you need synchronization you
     * must provide it yourself.
     *
     */
    public class OtpOutputStream : MemoryStream
    {
        private static readonly Encoding Latin1Encoding = Encoding.GetEncoding("ISO-8859-1",
                new EncoderExceptionFallback(),
                new DecoderExceptionFallback());
        private static readonly byte[] TrueString = Latin1Encoding.GetBytes("true");
        private static readonly byte[] FalseString = Latin1Encoding.GetBytes("false");

        /** The default initial size of the stream. * */
        public static readonly int DefaultInitialSize = 2048;

        /**
         * The default increment used when growing the stream (increment at least this much).
         */
        public static readonly int DefaultIncrement = 2048;

        /**
         * Create a stream with the default initial size (2048 bytes).
         */
        public OtpOutputStream()
            : this(DefaultInitialSize)
        {
        }

        /**
         * Create a stream with the specified initial size.
         */
        public OtpOutputStream(int size)
            : base(size)
        {
        }

        /**
         * Write a string to the stream as an Erlang atom. Assumes ISO-8859-1 encoding.
         *
         * @param atom
         *            the string to write.
         */
        public void WriteAtom(string atom)
        {
            // NB: will throw exception if atom can't be converted to ISO-8859-1
            byte[] bytes = Latin1Encoding.GetBytes(atom);
            Write(OtpExternal.AtomTag);
            Write2BE(bytes.Length);
        }

        /**
         * Write an array of bytes to the stream as an Erlang binary.
         *
         * @param bin
         *            the array of bytes to write.
         */
        public void WriteBinary(byte[] bin)
        {
            Write(OtpExternal.BinTag);
            Write4BE(bin.Length);
            Write(bin, 0, bin.Length);
        }

        public void WriteStringAsBinary(string s)
        {
            WriteBinary(Encoding.UTF8.GetBytes(s));
        }

        /**
         * Write a bool value to the stream as the Erlang atom 'true' or 'false'.
         *
         * @param b
         *            the bool value to write.
         */
        public void WriteBoolean(bool b)
        {
            Write(OtpExternal.AtomTag);
            if (b)
            {
                Write(TrueString, 0, TrueString.Length);
            }
            else
            {
                Write(FalseString, 0, FalseString.Length);
            }
        }

        /**
         * Write a double value to the stream.
         *
         * @param d
         *            the double to use.
         */
        public void WriteDouble(double d)
        {
            Write(OtpExternal.NewFloatTag);
            // TODO: endianness?
            Write8BE(BitConverter.DoubleToInt64Bits(d));
        }

        /**
         * Write a long to the stream.
         *
         * @param l
         *            the long to use.
         */
        public void WriteLong(long l)
        {
            WriteLong1(l);
        }

        /**
         * Write an integer to the stream.
         *
         * @param i
         *            the integer to use.
         */
        public void WriteInt(int i) {
            WriteLong1(i);
        }

        /**
         * Write a short to the stream.
         *
         * @param s
         *            the short to use.
         */
        public void WriteShort(short s)
        {
            WriteLong1(s);
        }

        /**
         * Write an Erlang list header to the stream. After calling this method, you
         * must write 'arity' elements to the stream followed by nil, or it will not
         * be possible to decode it later.
         *
         * @param arity
         *            the number of elements in the list.
         */
        public void WriteListHead(int arity)
        {
            if (arity == 0)
            {
                WriteNil();
            }
            else
            {
                Write(OtpExternal.ListTag);
                Write4BE(arity);
            }
        }

        /**
         * Write an empty Erlang list to the stream.
         */
        public void WriteNil()
        {
            Write(OtpExternal.NilTag);
        }

        /**
         * Write an Erlang tuple header to the stream. After calling this method,
         * you must write 'arity' elements to the stream or it will not be possible
         * to decode it later.
         *
         * @param arity
         *            the number of elements in the tuple.
         */
        public void WriteTupleHead(int arity)
        {
            if (arity < byte.MaxValue)
            {
                Write(OtpExternal.SmallTupleTag);
                Write((byte)arity);
            }
            else
            {
                Write(OtpExternal.LargeTupleTag);
                Write4BE(arity);
            }
        }

        private void WriteLong1(long v)
        {
            /*
             * If v<0 and unsigned==true the value
             * java.lang.Long.MAX_VALUE-java.lang.Long.MIN_VALUE+1+v is written, i.e
             * v is regarded as unsigned two's complement.
             */
            if (v <= byte.MaxValue)
            {
                // will fit in one byte
                Write(OtpExternal.SmallIntTag);
                Write((byte)v);
            }
            else
            {
                // note that v != 0L
                if (v < OtpExternal.ErlMin || v > OtpExternal.ErlMax)
                {
                    // some kind of bignum
                    long abs = v < 0 ? -v : v;
                    byte sign = v < 0 ? OtpExternal.Positive : OtpExternal.Negative;
                    byte n;
                    long mask;
                    for (mask = 0xFFFFffffL, n = 4; (abs & mask) != abs; n++, mask = mask << 8 | 0xffL)
                    {
                        // count nonzero bytes
                    }
                    Write(OtpExternal.SmallBigTag);
                    Write(n); // length
                    Write(sign); // sign
                    WriteLE(abs, n); // value. obs! little endian
                }
                else
                {
                    Write(OtpExternal.IntTag);
                    Write4BE(v);
                }
            }
        }

        /**
         * Write any number of bytes in little endian format.
         *
         * @param n
         *            the value to use.
         * @param b
         *            the number of bytes to write from the little end.
         */
        private void WriteLE(long n, int b)
        {
            long v = n;
            for (int i = 0; i < b; i++)
            {
                Write((byte)(v & 0xff));
                v >>= 8;
            }
        }

        /**
         * Write the low two bytes of a value to the stream in little endian order.
         *
         * @param n
         *            the value to use.
         */
        private void Write2LE(long n)
        {
            Write((byte)(n & 0xff));
            Write((byte)((n & 0xff00) >> 8));
        }

        /**
         * Write the low four bytes of a value to the stream in little endian order.
         *
         * @param n
         *            the value to use.
         */
        private void Write4LE(long n)
        {
            Write((byte)(n & 0xff));
            Write((byte)((n & 0xff00) >> 8));
            Write((byte)((n & 0xff0000) >> 16));
            Write((byte)((n & 0xff000000) >> 24));
        }

        /**
         * Write the low eight bytes of a value to the stream in little endian
         * order.
         *
         * @param n
         *            the value to use.
         */
        private void Write8LE(long n)
        {
            Write((byte)(n & 0xff));
            Write((byte)(n >> 8 & 0xff));
            Write((byte)(n >> 16 & 0xff));
            Write((byte)(n >> 24 & 0xff));
            Write((byte)(n >> 32 & 0xff));
            Write((byte)(n >> 40 & 0xff));
            Write((byte)(n >> 48 & 0xff));
            Write((byte)(n >> 56 & 0xff));
        }

        /**
         * Write the low eight (all) bytes of a value to the stream in big endian
         * order.
         *
         * @param n
         *            the value to use.
         */
        private void Write8BE(long n)
        {
            Write((byte)(n >> 56 & 0xff));
            Write((byte)(n >> 48 & 0xff));
            Write((byte)(n >> 40 & 0xff));
            Write((byte)(n >> 32 & 0xff));
            Write((byte)(n >> 24 & 0xff));
            Write((byte)(n >> 16 & 0xff));
            Write((byte)(n >> 8 & 0xff));
            Write((byte)(n & 0xff));
        }

        /**
         * Write the low four bytes of a value to the stream in big endian order.
         *
         * @param n
         *            the value to use.
         */
        private void Write4BE(long n)
        {
            Write((byte)((n & 0xff000000) >> 24));
            Write((byte)((n & 0xff0000) >> 16));
            Write((byte)((n & 0xff00) >> 8));
            Write((byte)(n & 0xff));
        }

        /**
         * Write the low two bytes of a value to the stream in big endian order.
         *
         * @param n
         *            the value to use.
         */
        private void Write2BE(long n)
        {
            Write((byte)((n & 0xff00) >> 8));
            Write((byte)(n & 0xff));
        }

        /**
         * Write one byte to the stream.
         *
         * @param b
         *            the byte to write.
         *
         */
        private void Write(byte b)
        {
            WriteByte(b);
        }

        private bool Is8bitstring(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            foreach (char c in s)
            {
                if (c < 0 || c > 255)
                {
                    return false;
                }
            }

            return true;
        }
    }
}