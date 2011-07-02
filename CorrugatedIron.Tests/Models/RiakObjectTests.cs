using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorrugatedIron.Models;
using CorrugatedIron.Tests.Extensions;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Models
{
    [TestFixture]
    public class RiakObjectTests
    {
        private const string Bucket = "bucket";
        private const string Key = "key";

        [Test]
        public void ToRiakObjectIdProducesAValidRiakObjectId()
        {
            var riakObject = new RiakObject(Bucket, Key, "value");
            var riakObjectId = riakObject.ToRiakObjectId();

            riakObjectId.Bucket.ShouldEqual(Bucket);
            riakObjectId.Key.ShouldEqual(Key);
        }
    }
}
