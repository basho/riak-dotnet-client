namespace CorrugatedIron.Util
{
    public static class Constants
    {
        public static class ContentTypes
        {
            public const string Any = @"*/*";
            public const string ApplicationOctetStream = @"application/octet-stream";
            public const string ApplicationJson = @"application/json";
            public const string TextPlain = @"text/plain";
            public const string TextHtml = @"text/html";
            public const string MultipartMixed = @"multipart/mixed";
            public const string ImageJpg = @"image/jpeg";
            public const string ImageGif = @"image/gif";
            public const string ImagePng = @"image/png";
			public const string ErlangBinary = @"application/x-erlang-binary";
        }

        public static class CharSets
        {
            public const string Utf8 = @"UTF-8";
        }

        public const int MinClientIdLength = 4;

        public static class Defaults
        {
            public const uint RVal = 2;
            public const string ContentType = ContentTypes.ApplicationOctetStream;
            public const string CharSet = CharSets.Utf8;
        }
    }
}
