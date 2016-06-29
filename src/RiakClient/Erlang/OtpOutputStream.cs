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

    /// <summary>
    /// Provides a stream for encoding Erlang terms to external format, for transmission or storage.
    /// </summary>
    internal class OtpOutputStream : MemoryStream
    {
        private static readonly Encoding Latin1Encoding =
            Encoding.GetEncoding("ISO-8859-1", new EncoderExceptionFallback(), new DecoderExceptionFallback());

        private static readonly byte[] TrueString = Latin1Encoding.GetBytes("true");

        private static readonly byte[] FalseString = Latin1Encoding.GetBytes("false");

        /// <summary>
        /// Initializes a new instance of the <see cref="OtpOutputStream" /> class
        /// with the default initial <see cref="MemoryStream"/>size.
        /// </summary>
        public OtpOutputStream()
        {
        }

        /// <summary>
        /// Write one byte to the stream.
        /// </summary>
        /// <param name="b">The byte to write</param>
        public void Write(byte b)
        {
            WriteByte(b);
        }

        /// <summary>
        /// Write the low two bytes of a value to the stream in big endian order.
        /// </summary>
        /// <param name="n">The value to use.</param>
        public void Write2BE(long n)
        {
            Write((byte)((n & 0xff00) >> 8));
            Write((byte)(n & 0xff));
        }

        /// <summary>
        /// Write the low four bytes of a value to the stream in big endian order.
        /// </summary>
        /// <param name="n">The value to use.</param>
        public void Write4BE(long n)
        {
            Write((byte)((n & 0xff000000) >> 24));
            Write((byte)((n & 0xff0000) >> 16));
            Write((byte)((n & 0xff00) >> 8));
            Write((byte)(n & 0xff));
        }

        /// <summary>
        /// Write the low eight (all) bytes of a value to the stream in big endian order.
        /// </summary>
        /// <param name="n">The value to use.</param>
        public void Write8BE(long n)
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

        /// <summary>
        /// Write any number of bytes in little endian format.
        /// </summary>
        /// <param name="n">The value to use.</param>
        /// <param name="b">The number of bytes to write from the little end.</param>
        public void WriteLE(long n, int b)
        {
            long v = n;
            for (int i = 0; i < b; i++)
            {
                Write((byte)(v & 0xff));
                v >>= 8;
            }
        }

        /// <summary>
        /// Write the low two bytes of a value to the stream in little endian order.
        /// </summary>
        /// <param name="n">The value to use.</param>
        public void Write2LE(long n)
        {
            Write((byte)(n & 0xff));
            Write((byte)((n & 0xff00) >> 8));
        }

        /// <summary>
        /// Write the low four bytes of a value to the stream in little endian order.
        /// </summary>
        /// <param name="n">The value to use.</param>
        public void Write4LE(long n)
        {
            Write((byte)(n & 0xff));
            Write((byte)((n & 0xff00) >> 8));
            Write((byte)((n & 0xff0000) >> 16));
            Write((byte)((n & 0xff000000) >> 24));
        }

        /// <summary>
        /// Write the low eight bytes of a value to the stream in little endian order.
        /// </summary>
        /// <param name="n">The value to use.</param>
        public void Write8LE(long n)
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

        /// <summary>
        /// Write a string to the stream as an Erlang atom. Assumes ISO-8859-1 encoding.
        /// This will throw an exception if the string can't be converted.
        /// </summary>
        /// <param name="atom">The string to write.</param>
        public void WriteAtom(string atom)
        {
            // NB: will throw exception if atom can't be converted to ISO-8859-1
            byte[] bytes = Latin1Encoding.GetBytes(atom);
            Write(OtpExternal.AtomTag);
            Write2BE(bytes.Length);
        }

        /// <summary>
        /// Write an array of bytes to the stream as an Erlang binary.
        /// </summary>
        /// <param name="bin">The array of bytes to write.</param>
        public void WriteBinary(byte[] bin)
        {
            Write(OtpExternal.BinTag);
            Write4BE(bin.Length);
            Write(bin, 0, bin.Length);
        }

        /// <summary>
        /// Write a string as a UTF-8 encoded array of bytes to the stream as an Erlang binary.
        /// </summary>
        /// <param name="s">The string to write.</param>
        public void WriteStringAsBinary(string s)
        {
            WriteBinary(Encoding.UTF8.GetBytes(s));
        }

        /// <summary>
        /// Write a bool value to the stream as the Erlang atom 'true' or 'false'.
        /// </summary>
        /// <param name="b">The boolean value to write.</param>
        public void WriteBoolean(bool b)
        {
            Write(OtpExternal.AtomTag);
            if (b)
            {
                Write2BE(TrueString.Length);
                Write(TrueString, 0, TrueString.Length);
            }
            else
            {
                Write2BE(FalseString.Length);
                Write(FalseString, 0, FalseString.Length);
            }
        }

        /// <summary>
        /// Write a double value to the stream.
        /// </summary>
        /// <param name="d">The double to use.</param>
        public void WriteDouble(double d)
        {
            Write(OtpExternal.NewFloatTag);
            Write8BE(BitConverter.DoubleToInt64Bits(d));
        }

        /// <summary>
        /// Write a long to the stream.
        /// </summary>
        /// <param name="l">The long to use.</param>
        public void WriteLong(long l)
        {
            if (l <= byte.MaxValue)
            {
                // will fit in one byte
                Write(OtpExternal.SmallIntTag);
                Write((byte)l);
            }
            else
            {
                // note that l != 0L
                if (l < OtpExternal.ErlMin || l > OtpExternal.ErlMax)
                {
                    // some kind of bignum
                    long abs = l < 0 ? -l : l;
                    byte sign = l < 0 ? OtpExternal.Positive : OtpExternal.Negative;
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
                    Write4BE(l);
                }
            }
        }

        /// <summary>
        /// Write an Erlang list header to the stream. After calling this method, you
        /// must write 'arity' elements to the stream followed by nil, or it will not
        /// be possible to decode it later.
        /// </summary>
        /// <param name="arity">The number of elements in the list.</param>
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

        /// <summary>
        /// Write an empty Erlang list to the stream.
        /// </summary>
        public void WriteNil()
        {
            Write(OtpExternal.NilTag);
        }

        /// <summary>
        /// Write an Erlang tuple header to the stream. After calling this method,
        /// you must write 'arity' elements to the stream or it will not be possible
        /// to decode it later.
        /// </summary>
        /// <param name="arity">The number of elements in the tuple.</param>
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
    }
}