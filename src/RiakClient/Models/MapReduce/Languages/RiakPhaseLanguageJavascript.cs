namespace RiakClient.Models.MapReduce.Languages
{
    using Extensions;
    using Newtonsoft.Json;

    internal class RiakPhaseLanguageJavascript : IRiakPhaseLanguage
    {
        private string name;
        private string source;
        private string bucket;
        private string key;

        /// <summary>
        /// Specify a name of the known JavaScript function to execute for this phase.
        /// </summary>
        /// <param name="name">The name of the function to execute.</param>
        public void Name(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Specify the source code of the JavaScript function to dynamically load and execute for this phase.
        /// </summary>
        /// <param name="source">The source code of the function to execute.</param>
        public void Source(string source)
        {
            this.source = source;
        }

        /// <summary>
        /// Specify a bucket and key where a stored JavaScript function can be dynamically loaded from Riak and executed for this phase
        /// </summary>
        /// <param name="bucket">The bucket name of the JavaScript function's address.</param>
        /// <param name="key">The key of the JavaScript function's address.</param>
        public void BucketKey(string bucket, string key)
        {
            this.bucket = bucket;
            this.key = key;
        }

        /// <inheritdoc/>
        public void WriteJson(JsonWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(bucket), "Bucket should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(key), "Key should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(source), "Source should be empty if Name specified");
            }
            else if (!string.IsNullOrWhiteSpace(source))
            {
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(bucket), "Bucket should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(key), "Key should be empty if Name specified");
                System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(name), "Name should be empty if Name specified");
            }
            else
            {
                System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(bucket), "Bucket should not be empty");
                System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(key), "Key should not be empty");
            }

            writer.WriteSpecifiedProperty("language", RiakConstants.MapReduceLanguage.JavaScript)
                .WriteSpecifiedProperty("source", source)
                .WriteSpecifiedProperty("name", name)
                .WriteSpecifiedProperty("bucket", bucket)
                .WriteSpecifiedProperty("key", key);
        }
    }
}
