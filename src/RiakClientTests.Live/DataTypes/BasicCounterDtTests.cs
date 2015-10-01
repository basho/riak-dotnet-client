#pragma warning disable 618

namespace RiakClientTests.Live.DataTypes
{
    using NUnit.Framework;
    using RiakClient.Models;

    [TestFixture, IntegrationTest]
    public class BasicCounterDtTests : DataTypeTestsBase
    {
        [Test]
        public void TestCounterOperations()
        {
            string key = GetRandomKey();

            var id = new RiakObjectId(BucketTypeNames.Counters, Bucket, key);

            // Fetch empty
            var initialCounter = Client.DtFetchCounter(id);

            Assert.IsFalse(initialCounter.Value.HasValue);

            // Increment one
            var updatedCounter1 = Client.DtUpdateCounter(id, 1);
            Assert.AreEqual(1, updatedCounter1.Value);

            // Increment many
            var updatedCounter2 = Client.DtUpdateCounter(id, 4);
            Assert.AreEqual(5, updatedCounter2.Value);

            // Fetch non-empty counter
            var incrementedCounter = Client.DtFetchCounter(id);
            Assert.AreEqual(5, incrementedCounter.Value);

            // Decrement one
            var updatedCounter3 = Client.DtUpdateCounter(id, -1);
            Assert.AreEqual(4, updatedCounter3.Value);

            // Decrement many
            var updatedCounter4 = Client.DtUpdateCounter(id, -4);
            Assert.AreEqual(0, updatedCounter4.Value);
        }

    }
}

#pragma warning restore 618