namespace Test.Unit.Core
{
    using NUnit.Framework;
    using Riak.Core;

    [TestFixture, UnitTest]
    public class StateManagerTests
    {
        public enum States : byte
        {
            STATE_ZERO = 0,
            STATE_ONE,
            STATE_TWO,
        }

        public enum Ints : int
        {
            INT_NONE = -1,
            INT_ZERO = 0,
            INT_ONE = 1
        }

        public enum UInts : uint
        {
            UINT_ZERO = 0,
            UINT_ONE = 1,
            UINT_TWO = 2
        }

        [Test]
        public void Creating_StateManager_With_Enum_Works()
        {
            var sm = StateManager.FromEnum<States>(this);

            Assert.AreEqual(States.STATE_ZERO, (States)sm.GetState());
            Assert.AreEqual("STATE_ZERO", sm.ToString());
            Assert.True(sm.IsCurrentState((byte)States.STATE_ZERO));

            sm.SetState((byte)States.STATE_TWO);

            Assert.AreEqual(States.STATE_TWO, (States)sm.GetState());
            Assert.AreEqual("STATE_TWO", sm.ToString());
            Assert.True(sm.IsCurrentState((byte)States.STATE_TWO));
        }
    }
}
