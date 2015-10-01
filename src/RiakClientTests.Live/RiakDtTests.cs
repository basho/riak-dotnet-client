namespace RiakClient.Tests.Live
{
    using RiakClient.Tests.Live;
    using NUnit.Framework;

    [TestFixture]
    public class RiakDtTests : LiveRiakConnectionTestBase
    {
        private const string Bucket = "riak_dt_bucket";

        /// <summary>
        /// The tearing of the down, it is done here.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(Bucket);
        }
    }
}
