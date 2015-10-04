namespace RiakClient.Comms
{
    using System;
    using System.Collections.Generic;
    using Commands;
    using Messages;

    /// <summary>
    /// The public interface for connections to Riak.
    /// </summary>
    public interface IRiakConnection : IDisposable
    {
        void Disconnect();

        RiakResult<TResult> PbcRead<TResult>()
            where TResult : class, new();

        RiakResult PbcRead(MessageCode expectedMessageCode);

        RiakResult PbcWrite<TRequest>(TRequest request)
            where TRequest : class;

        RiakResult PbcWrite(MessageCode messageCode);

        RiakResult<TResult> PbcWriteRead<TRequest, TResult>(TRequest request)
            where TRequest : class
            where TResult : class, new();

        RiakResult<TResult> PbcWriteRead<TResult>(MessageCode messageCode)
            where TResult : class, new();

        RiakResult PbcWriteRead<TRequest>(TRequest request, MessageCode expectedMessageCode)
            where TRequest : class;

        RiakResult PbcWriteRead(MessageCode messageCode, MessageCode expectedMessageCode);

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcRepeatRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead)
            where TResult : class, new();

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteRead<TResult>(MessageCode messageCode, Func<RiakResult<TResult>, bool> repeatRead)
            where TResult : class, new();

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteRead<TRequest, TResult>(TRequest request, Func<RiakResult<TResult>, bool> repeatRead)
            where TRequest : class
            where TResult : class, new();

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcStreamRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead, Action onFinish)
            where TResult : class, new();

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteStreamRead<TRequest, TResult>(
            TRequest request,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TRequest : class
            where TResult : class, new();

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteStreamRead<TResult>(
            MessageCode messageCode,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TResult : class, new();

        RiakResult Execute(IRCommand command);
    }
}
