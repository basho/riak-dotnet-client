using System;
using System.Threading.Tasks;
using CorrugatedIron.Comms;

namespace CorrugatedIron
{
    public class RiakBatch : IRiakEndPoint
    {
        private readonly IRiakEndPoint _endPoint;
        private readonly IRiakEndPointContext _endPointContext;

        public RiakBatch(IRiakEndPoint endPoint, IRiakEndPointContext riakEndPointContext)
        {
            _endPoint = endPoint;
            _endPointContext = riakEndPointContext;
        }

        public void Dispose()
        {
            if (_endPointContext != null)
            {
                _endPointContext.Dispose();
            }
        }

        public IRiakClient CreateClient()
        {
            return _endPoint.CreateClient();
        }

        public async Task GetSingleResultViaPbc(Func<RiakPbcSocket, Task> useFun)
        {
            await _endPoint.GetSingleResultViaPbc(_endPointContext, useFun).ConfigureAwait(false);
        }

        public async Task<TResult> GetSingleResultViaPbc<TResult>(Func<RiakPbcSocket, Task<TResult>> useFun)
        {
            return await _endPoint.GetSingleResultViaPbc(_endPointContext, useFun).ConfigureAwait(false);
        }

        public async Task GetMultipleResultViaPbc(Func<RiakPbcSocket, Task> useFun)
        {
            await _endPoint.GetMultipleResultViaPbc(_endPointContext, useFun).ConfigureAwait(false);
        }

        public async Task GetSingleResultViaPbc(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task> useFun)
        {
            await _endPoint.GetSingleResultViaPbc(riakEndPointContext, useFun).ConfigureAwait(false);
        }

        public async Task<TResult> GetSingleResultViaPbc<TResult>(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task<TResult>> useFun)
        {
            return await _endPoint.GetSingleResultViaPbc(riakEndPointContext, useFun).ConfigureAwait(false);
        }

        public async Task GetMultipleResultViaPbc(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task> useFun)
        {
            await _endPoint.GetMultipleResultViaPbc(riakEndPointContext, useFun).ConfigureAwait(false);
        }

        public async Task GetSingleResultViaRest(Func<string, Task> useFun)
        {
            await _endPoint.GetSingleResultViaRest(useFun).ConfigureAwait(false);
        }

        public async Task<TResult> GetSingleResultViaRest<TResult>(Func<string, Task<TResult>> useFun)
        {
            return await _endPoint.GetSingleResultViaRest(useFun).ConfigureAwait(false);
        }

        public async Task GetMultipleResultViaRest(Func<string, Task> useFun)
        {
            await _endPoint.GetMultipleResultViaRest(useFun).ConfigureAwait(false);
        }
    }
}
