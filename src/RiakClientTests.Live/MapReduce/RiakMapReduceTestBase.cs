namespace RiakClientTests.Live.MapReduce
{
    using RiakClient;

    public abstract class RiakMapReduceTestBase : LiveRiakConnectionTestBase
    {
        protected const string MrContentType = RiakConstants.ContentTypes.ApplicationJson;
        protected const string EmptyBody = "{}";
        protected string Bucket = "fluent_key_bucket";
    }
}