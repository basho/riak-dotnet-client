namespace RiakClient.Erlang
{
    using Exceptions;
    using Util;

    internal static class TtbErrorDecoder
    {
        public const string RpbErrorRespAtom = "rpberrorresp";
        public const string RpbErrorRespEmpty = "No Riak error message or code returned.";

        public static RiakException MaybeRiakError(byte[] response)
        {
            RiakException rv = null;

            if (EnumerableUtil.IsNullOrEmpty(response))
            {
                string errMsg = "TTB request returned null or zero-length data buffer.";
                rv = new RiakException(0, errMsg, false);
            }

            using (var s = new OtpInputStream(response))
            {
                string atom;
                byte tag = s.Peek();
                switch (tag)
                {
                    case OtpExternal.AtomTag:
                        atom = s.ReadAtom();
                        if (atom.Equals(RpbErrorRespAtom))
                        {
                            throw new RiakException(0, RpbErrorRespEmpty, false);
                        }

                        break;
                    case OtpExternal.SmallTupleTag:
                    case OtpExternal.LargeTupleTag:
                        int arity = s.ReadTupleHead();
                        if (arity >= 1)
                        {
                            tag = s.Peek();
                            if (tag == OtpExternal.AtomTag)
                            {
                                atom = s.ReadAtom();
                                if (atom.Equals(RpbErrorRespAtom))
                                {
                                    arity--; // We've read one item in the tuple
                                    string errMsg = RpbErrorRespEmpty;
                                    int errCode = 0;

                                    for (int i = 0; i < arity; ++i)
                                    {
                                        tag = s.Peek();
                                        if (tag == OtpExternal.BinTag)
                                        {
                                            errMsg = s.ReadBinaryAsString();
                                        }
                                        else if (s.IsLongTag(tag))
                                        {
                                            errCode = (int)s.ReadLong();
                                        }
                                        else
                                        {
                                            errMsg = string.Format("Unexpected tag {0} in {1}", tag, RpbErrorRespAtom);
                                            errCode = 0;
                                            break;
                                        }
                                    }

                                    rv = new RiakException(errCode, errMsg, false);
                                }
                            }
                        }

                        break;
                }
            }

            return rv;
        }
    }
}
