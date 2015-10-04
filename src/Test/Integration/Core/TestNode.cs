namespace Test.Integration.Core
{
    using System;
    using System.Runtime.InteropServices;
    using Riak.Core;

    [ComVisible(false)]
    public class TestNode : Node
    {
        private readonly Action<State> stateObserver;

        public TestNode(NodeOptions opts, Action<State> stateObserver)
            : base(opts)
        {
            this.stateObserver = stateObserver;
        }

        protected override void SetState(State s)
        {
            base.SetState(s);
            if (stateObserver != null)
            {
                stateObserver(s);
            }
        }
    }
}
