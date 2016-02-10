namespace RiakClient
{
    using System;
    using System.Collections.Generic;
    using Messages;

    /// <summary>
    /// A collection of commonly used Riak string constants.
    /// </summary>
    public static class RiakConstants
    {
        /// <summary>
        /// Represents the value for the Default Bucket Type when using the .Net Client APIs.
        /// </summary>
        public const string DefaultBucketType = null;

        /// <summary>
        /// Represents Riak Enterprise Edition-only features.
        /// </summary>
        public static class RiakEnterprise
        {
            /// <summary>
            /// Represents the possible stats that Riak Replication could be configured for.
            /// </summary>
            public enum ReplicationMode
            {
                /// <summary>
                /// No replication is enabled.
                /// </summary>
                False = RpbBucketProps.RpbReplMode.FALSE,

                /// <summary>
                /// Realtime replication is enabled.
                /// </summary>
                Realtime = RpbBucketProps.RpbReplMode.REALTIME,
                
                /// <summary>
                /// Fullsync replication is enabled.
                /// </summary>
                FullSync = RpbBucketProps.RpbReplMode.FULLSYNC,
                
                /// <summary>
                /// Realtime and fullsync replication are both enabled.
                /// </summary>
                True = RpbBucketProps.RpbReplMode.TRUE
            }
        }

        /// <summary>
        /// A collection of known content-types.
        /// </summary>
        public static class ContentTypes
        {
            /// <summary>
            /// A wildcard content-type.
            /// </summary>
            public const string Any = @"*/*";
            
            /// <summary>
            /// The binary, or octet-stream content-type.
            /// </summary>
            public const string ApplicationOctetStream = @"application/octet-stream";
            
            /// <summary>
            /// The JSON content-type.
            /// </summary>
            public const string ApplicationJson = @"application/json";
            
            /// <summary>
            /// The plain text content-type.
            /// </summary>
            public const string TextPlain = @"text/plain";

            /// <summary>
            /// The HTML content-type.
            /// </summary>
            public const string TextHtml = @"text/html";

            /// <summary>
            /// The multipart/mixed message content-type.
            /// </summary>
            public const string MultipartMixed = @"multipart/mixed";

            /// <summary>
            /// The Jpeg image content-type.
            /// </summary>
            public const string ImageJpg = @"image/jpeg";

            /// <summary>
            /// The Gif image content-type.
            /// </summary>
            public const string ImageGif = @"image/gif";

            /// <summary>
            /// The Png image content-type.
            /// </summary>
            public const string ImagePng = @"image/png";

            /// <summary>
            /// The Erlang Binary content-type.
            /// </summary>
            public const string ErlangBinary = @"application/x-erlang-binary";

            /// <summary>
            /// The XML content-type.
            /// </summary>
            public const string Xml = @"application/xml";

            /// <summary>
            /// The Protocol Buffers content-type.
            /// </summary>
            public const string ProtocolBuffers = ApplicationOctetStream; // @"application/x-protobuf";
        }

        /// <summary>
        /// Represents the Secondary Index name suffixes used to determine the index's type.
        /// </summary>
        public static class IndexSuffix
        {
            /// <summary>
            /// The suffix for integer indexes.
            /// </summary>
            public const string Integer = @"_int";
            
            /// <summary>
            /// The suffix for string(binary) indexes.
            /// </summary>
            public const string Binary = @"_bin";
        }

        /// <summary>
        /// Represents the collection of possible MapReduce job languages. 
        /// </summary>
        public static class MapReduceLanguage
        {
            /// <summary>
            /// The string for JavaScript MapReduce jobs.
            /// </summary>
            public const string JavaScript = "javascript";
            
            /// <summary>
            /// The string for JSON(JavaScript) MapReduce jobs.
            /// </summary>
            public const string Json = JavaScript;
            
            /// <summary>
            /// The string for Erlang MapReduce jobs.
            /// </summary>
            public const string Erlang = "erlang";
        }

        /// <summary>
        /// Represents the collection of possible MapReduce job phases.
        /// </summary>
        public static class MapReducePhaseType
        {
            /// <summary>
            /// The string for a Map phase.
            /// </summary>
            public const string Map = @"map";
            
            /// <summary>
            /// The string for a Reduce phase.
            /// </summary>
            public const string Reduce = @"reduce";
            
            /// <summary>
            /// The string for a Link phase.
            /// </summary>
            public const string Link = @"link";
        }

        /// <summary>
        /// Represents some common Character sets.
        /// </summary>
        public static class CharSets
        {
            /// <summary>
            /// The charset string for the UTF-8 string data.
            /// </summary>
            public const string Utf8 = @"UTF-8";
            
            /// <summary>
            /// The charset string used when dealing with Binary data. 
            /// </summary>
            public const string Binary = null;
        }

        /// <summary>
        /// Represents some defaults for different Riak Client options.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// The default content-type. Defaults to <see cref="ContentTypes.ApplicationJson"/>.
            /// </summary>
            public const string ContentType = ContentTypes.ApplicationJson;
            
            /// <summary>
            /// The default character set. Defaults to <see cref="CharSets.Utf8"/>.
            /// </summary>
            public const string CharSet = CharSets.Utf8;

            /// <summary>
            /// Represents some Riak Search 2.0+ (Yokozuna) defaults.
            /// </summary>
            public static class YokozunaIndex
            {
                /// <summary>
                /// The default Schema Name to use for 2.0+ search.
                /// </summary>
                public const string DefaultSchemaName = "_yz_default";
                
                /// <summary>
                /// The default NVal to use for 2.0+ search index operations.
                /// </summary>
                public const int NVal = 3;
            }
        }

        /// <summary>
        /// A collection of MapReduce key filter transformation string constants.
        /// See http://docs.basho.com/riak/latest/dev/using/keyfilters/ 
        /// and http://docs.basho.com/riak/latest/dev/references/keyfilters/ for more information.
        /// </summary>
        public static class KeyFilterTransforms
        {
            /// <summary>
            /// Turns an integer (previously extracted with string_to_int), into a string.
            /// </summary>
            public const string IntToString = @"int_to_string";
            
            /// <summary>
            /// Turns a string into an integer.
            /// </summary>
            public const string StringToInt = @"string_to_int";

            /// <summary>
            /// Turns a floating point number (previously extracted with string_to_float), into a string.
            /// </summary>
            public const string FloatToString = @"float_to_string";

            /// <summary>
            /// Turns a string into a floating point number.
            /// </summary>
            public const string StringToFloat = @"string_to_float";

            /// <summary>
            /// Changes all letters to uppercase.
            /// </summary>
            public const string ToUpper = @"to_upper";

            /// <summary>
            /// Changes all letters to lowercase.
            /// </summary>
            public const string ToLower = @"to_lower";

            /// <summary>
            /// Splits the input on the string given as the first argument and returns the nth token specified by the second argument.
            /// </summary>
            public const string Tokenize = @"tokenize";
        }

        /// <summary>
        /// A collection of special secondary index names.
        /// </summary>
        public static class SystemIndexKeys
        {
            /// <summary>
            /// The index name for the special system key index.
            /// This index holds all the different keys for a bucket.
            /// </summary>
            public const string RiakKeysIndex = "$key";
            
            /// <summary>
            /// The index name for the special system bucket index.
            /// This index holds all the different bucket values.
            /// </summary>
            public const string RiakBucketIndex = "$bucket";

            /// <summary>
            /// A collection of all the system binary index names.
            /// </summary>
            public static readonly HashSet<string> SystemBinKeys = new HashSet<string> { RiakKeysIndex, RiakBucketIndex };
            
            /// <summary>
            /// A collection of all the system integer index names.
            /// </summary>
            public static readonly HashSet<string> SystemIntKeys = new HashSet<string>();
        }

        /// <summary>
        /// A collection of string constants for riak search result documents.
        /// These constants represent keys for known fields within the document. 
        /// </summary>
        public static class SearchFieldKeys
        {
            /// <summary>
            /// Key for the "Bucket Type" field. Riak 2.0+
            /// </summary>
            public const string BucketType = "_yz_rt";
            
            /// <summary>
            /// Key for the "Bucket" field. Riak 2.0+
            /// </summary>
            public const string Bucket = "_yz_rb";
            
            /// <summary>
            /// Key for the "Key" field. Riak 2.0+
            /// </summary>
            public const string Key = "_yz_rk";

            /// <summary>
            /// Key for the "Id" field. Riak 2.0+
            /// </summary>
            public const string Id = "_yz_id";
            
            /// <summary>
            /// Key for the "Score" field. Riak 2.0+
            /// </summary>
            public const string Score = "score";

            /// <summary>
            /// Key for the Riak 1.0 Search "Id" field.
            /// </summary>
            [Obsolete("Legacy Search is deprecated, this key will be removed in the future.")]
            public const string LegacySearchId = "id";
        }
    }
}
