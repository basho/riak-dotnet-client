namespace RiakClientTests.Models
{
    using System;
    using NUnit.Framework;
    using RiakClient;

    [TestFixture, UnitTest]
    public class NValTests
    {
        private static readonly Random r = new Random();

        [Test]
        public void WhenUsingInvalidNVal_ThrowsArgumentException()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() => new NVal(-1));
            Assert.Catch<ArgumentOutOfRangeException>(() => new NVal(0));
        }

        [Test]
        public void WhenUsingValidNVal_CanBeCastToUint()
        {
            int random = r.Next();
            var nval = new NVal(random);
            Assert.AreEqual((uint)random, (uint)nval);
        }
    }
}
