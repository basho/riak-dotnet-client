using System;
using NUnit.Framework;
using CorrugatedIron.Tests.Extensions;

namespace CorrugatedIron.Tests
{
    [TestFixture]
    public class UnitTestExtensionsTest
    {
        [Ignore("Run this to test the WaitUntil test helper's output")]
        [Test]
        public void ThisTestShouldFail()
        {
            Func<RiakResult> alwaysFail = () => RiakResult.Error(ResultCode.InvalidRequest, "Nope.", true);
            Func<RiakResult> alwaysThrow = () => { throw new ApplicationException("Whoopsie"); };
            var failResult = alwaysFail.WaitUntil(2);
            alwaysThrow.WaitUntil(2);
            failResult.IsSuccess.ShouldBeFalse();
        }
    }
}

