namespace Test.Integration.CRDT
{
    using System;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;

    public class GitHub_277 : TestBase
    {
        protected override RiakString BucketType
        {
            get { return new RiakString("write_once"); }
        }

        protected override RiakString Bucket
        {
            get { return new RiakString("github_277"); }
        }

        [Test]
        public void Putting_A_Value_To_A_Write_Once_Bucket_Works()
        {
            string key = Guid.NewGuid().ToString();
            string value = "test value";

            var id = new RiakObjectId(BucketType, Bucket, key);
            var obj = new RiakObject(id, value, RiakConstants.ContentTypes.TextPlain);

            RiakResult<RiakObject> rslt = client.Put(obj);
            Assert.True(rslt.IsSuccess, rslt.ErrorMessage);
        }
    }
}