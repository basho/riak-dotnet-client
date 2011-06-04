using CorrugatedIron.Comms;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Comms.RiakClientGetTests
{
    public abstract class RiakClientGetTestBase : RiakClientTestBase<RpbGetResp>
    {
        protected RiakResult<RiakObject> Result;
        protected string Bucket = "test-bucket";
        protected string Key = "test-key";
        protected uint? Rval;

        protected RiakClientGetTestBase(RiakResult<RpbGetResp> result)
            : base(result)
        {
        }

        [SetUp]
        public void SetUp()
        {
            Result = Rval.HasValue ? Client.Get(Bucket, Key, Rval.Value) : Client.Get(Bucket, Key);
        }
    }

    public class WhenGettingObjectFromRiak : RiakClientGetTestBase
    {
        public WhenGettingObjectFromRiak()
            : base(RiakResult<RpbGetResp>.Success(new RpbGetResp
            {
                VectorClock = System.Text.Encoding.Default.GetBytes("somerandom"),
                Content = new List<RpbContent>
                {
                }
            }))
        {
        }

        [Test]
        public void RvalIsSetToTheProperDefault()
        {
            
        }
    }
}
