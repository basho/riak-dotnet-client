namespace RiakClientTests.Models
{
    using NUnit.Framework;
    using RiakClient.Models;

    [TestFixture, UnitTest]
    public class RiakLinkTests
    {
        private static readonly RiakLink Link = new RiakLink("bucket", "key", "tag");

        [Test]
        public void RiakLinkCanBeCreatedFromJsonString()
        {
            const string jsonString = @"[""bucket"", ""key"", ""tag""]";
            var jsonLink = RiakLink.FromJsonString(jsonString);

            Assert.AreEqual(Link, jsonLink);
        }
    }
}
