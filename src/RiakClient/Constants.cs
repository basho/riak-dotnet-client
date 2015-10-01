namespace Riak
{
    using RiakClient;

    public static class Constants
    {
        public static readonly Timeout DefaultConnectTimeout = new Timeout(30000);
        public static readonly Timeout DefaultRequestTimeout = new Timeout(5000);
    }
}