using System;
using System.Threading.Tasks;
using CorrugatedIron.Messages;
using CorrugatedIron.Models.Rest;

namespace CorrugatedIron.Comms
{
    public interface IRiakConnection
    {
        // PBC interface
        Task<TResult> PbcRead<TResult>(IRiakEndPoint endPoint)
            where TResult : class, new();

        Task PbcRead(IRiakEndPoint endPoint, MessageCode expectedMessageCode);

        Task PbcWrite<TRequest>(IRiakEndPoint endPoint, TRequest request)
            where TRequest : class;

        Task PbcWrite(IRiakEndPoint endPoint, MessageCode messageCode);

        Task<TResult> PbcWriteRead<TRequest, TResult>(IRiakEndPoint endPoint, TRequest request)
            where TRequest : class
            where TResult : class, new();

        Task<TResult> PbcWriteRead<TResult>(IRiakEndPoint endPoint, MessageCode messageCode)
            where TResult : class, new();

        Task PbcWriteRead<TRequest>(IRiakEndPoint endPoint, TRequest request, MessageCode expectedMessageCode)
            where TRequest : class;

        Task PbcWriteRead(IRiakEndPoint endPoint, MessageCode messageCode, MessageCode expectedMessageCode);

        IObservable<TResult> PbcRepeatRead<TResult>(IRiakEndPoint endPoint, Func<TResult, bool> repeatRead)
            where TResult : class, new();

        IObservable<TResult> PbcWriteRead<TResult>(IRiakEndPoint endPoint, MessageCode messageCode, Func<TResult, bool> repeatRead)
            where TResult : class, new();

        IObservable<TResult> PbcWriteRead<TRequest, TResult>(IRiakEndPoint endPoint, TRequest request, Func<TResult, bool> repeatRead)
            where TRequest : class
            where TResult : class, new();

        IObservable<TResult> PbcStreamRead<TResult>(IRiakEndPoint endPoint, Func<TResult, bool> repeatRead)
            where TResult : class, new();

        IObservable<TResult> PbcWriteStreamRead<TRequest, TResult>(IRiakEndPoint endPoint, TRequest request,
            Func<TResult, bool> repeatRead)
            where TRequest : class
            where TResult : class, new();

        IObservable<TResult> PbcWriteStreamRead<TResult>(IRiakEndPoint endPoint, MessageCode messageCode,
            Func<TResult, bool> repeatRead)
            where TResult : class, new();

        // REST interface
        Task<RiakRestResponse> RestRequest(IRiakEndPoint endPoint, RiakRestRequest request);
    }
}