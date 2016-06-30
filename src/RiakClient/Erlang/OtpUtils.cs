namespace RiakClient.Erlang
{
    using System.Text;

    internal static class OtpUtils
    {
        public static readonly Encoding Latin1Encoding =
            Encoding.GetEncoding("ISO-8859-1", new EncoderExceptionFallback(), new DecoderExceptionFallback());

        public static readonly byte[] TrueString = Latin1Encoding.GetBytes("true");

        public static readonly byte[] FalseString = Latin1Encoding.GetBytes("false");
    }
}