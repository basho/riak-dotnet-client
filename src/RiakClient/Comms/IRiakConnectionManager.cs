namespace RiakClient.Comms
{
    using System;

    internal interface IRiakConnectionManager : IDisposable
    {
        Tuple<bool, TResult> Consume<TResult>(Func<IRiakConnection, TResult> consumer);

        Tuple<bool, TResult> DelayedConsume<TResult>(Func<IRiakConnection, Action, TResult> consumer);
    }
}
