using System;
using System.Threading.Tasks;
using CorrugatedIron.Comms;
using CorrugatedIron.Exceptions;

namespace CorrugatedIron
{
    public class RiakNodeEndpoint : IRiakEndPoint
    {
        private readonly IRiakNode _node;
        private bool _disposing;

        public RiakNodeEndpoint(IRiakNode node)
        {
            _node = node;
        }

        public void Dispose()
        {
            _disposing = true;
        }

        public IRiakClient CreateClient()
        {
            return new RiakClient(this, new RiakConnection());
        }

        public async Task GetSingleResultViaPbc(Func<RiakPbcSocket, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            await _node.GetSingleResultViaPbc(useFun).ConfigureAwait(false);
        }

        public async Task GetSingleResultViaPbc(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            if (riakEndPointContext.Node == null)
            {
                riakEndPointContext.Node = _node;
            }

            if (riakEndPointContext.Socket == null)
            {
                riakEndPointContext.Socket = await riakEndPointContext.Node.CreateSocket();
            }

            await riakEndPointContext.Node.GetSingleResultViaPbc(riakEndPointContext.Socket, useFun).ConfigureAwait(false);
        }

        public async Task<TResult> GetSingleResultViaPbc<TResult>(Func<RiakPbcSocket, Task<TResult>> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            var result = await _node.GetSingleResultViaPbc(useFun).ConfigureAwait(false);

            return result;
        }

        public async Task<TResult> GetSingleResultViaPbc<TResult>(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task<TResult>> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            if (riakEndPointContext.Node == null)
            {
                riakEndPointContext.Node = _node;
            }

            if (riakEndPointContext.Socket == null)
            {
                riakEndPointContext.Socket = await riakEndPointContext.Node.CreateSocket();
            }

            var result = await riakEndPointContext.Node.GetSingleResultViaPbc(riakEndPointContext.Socket, useFun).ConfigureAwait(false);
            return result;
        }

        public async Task GetMultipleResultViaPbc(Func<RiakPbcSocket, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            await _node.GetMultipleResultViaPbc(useFun).ConfigureAwait(false);
        }

        public async Task GetMultipleResultViaPbc(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            if (riakEndPointContext.Node == null)
            {
                riakEndPointContext.Node = _node;
            }

            if (riakEndPointContext.Socket == null)
            {
                riakEndPointContext.Socket = await riakEndPointContext.Node.CreateSocket();
            }

            await riakEndPointContext.Node.GetMultipleResultViaPbc(riakEndPointContext.Socket, useFun).ConfigureAwait(false);
        }

        public async Task GetSingleResultViaRest(Func<string, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            await _node.GetSingleResultViaRest(useFun).ConfigureAwait(false);
        }

        public async Task<TResult> GetSingleResultViaRest<TResult>(Func<string, Task<TResult>> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            var result = await _node.GetSingleResultViaRest(useFun).ConfigureAwait(false);
            return result;
        }

        public async Task GetMultipleResultViaRest(Func<string, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            await _node.GetMultipleResultViaRest(useFun).ConfigureAwait(false);
        }
    }
}
