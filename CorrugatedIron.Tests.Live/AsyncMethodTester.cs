using System;
using System.Threading;

namespace CorrugatedIron.Tests.Live
{
    public class AsyncMethodTester<TCallbackResult>
    {
        private readonly AutoResetEvent _eventHandle;
        private TCallbackResult _result;

        public TCallbackResult Result
        {
            get
            {
                _eventHandle.WaitOne();
                return _result;
            }
        }

        public Action<TCallbackResult> HandleResult
        {
            get
            {
                return result =>
                    {
                        _result = result;
                        _eventHandle.Set();
                    };
            }
        }

        public AsyncMethodTester()
        {
            _eventHandle = new AutoResetEvent(false);
        }
    }
}
