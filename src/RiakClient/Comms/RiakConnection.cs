namespace RiakClient.Comms
{
    using System;
    using System.Collections.Generic;
    using Commands;
    using Exceptions;
    using Messages;
    using Riak.Config;

    internal class RiakConnection : IRiakConnection
    {
        private readonly RiakPbcSocket socket;

        public RiakConnection(INodeConfiguration nodeConfiguration, IAuthenticationConfiguration authConfiguration)
        {
            socket = new RiakPbcSocket(nodeConfiguration, authConfiguration);
        }

        public RiakResult<TResult> PbcRead<TResult>()
            where TResult : class, new()
        {
            try
            {
                var result = socket.Read<TResult>();
                return RiakResult<TResult>.Success(result);
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect();
                }

                if (ex.Message.Contains("Bucket cannot be zero-length")
                    || ex.Message.Contains("Key cannot be zero-length"))
                {
                    return RiakResult<TResult>.FromException(ResultCode.InvalidRequest, ex, ex.NodeOffline);
                }

                return RiakResult<TResult>.FromException(ResultCode.CommunicationError, ex, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect();
                return RiakResult<TResult>.FromException(ResultCode.CommunicationError, ex, true);
            }
        }

        public RiakResult PbcRead(MessageCode expectedMessageCode)
        {
            try
            {
                socket.Read(expectedMessageCode);
                return RiakResult.Success();
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect();
                }

                return RiakResult.FromException(ResultCode.CommunicationError, ex, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect();
                return RiakResult.FromException(ResultCode.CommunicationError, ex, true);
            }
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcRepeatRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead)
            where TResult : class, new()
        {
            var results = new List<RiakResult<TResult>>();
            try
            {
                RiakResult<TResult> result;

                do
                {
                    result = RiakResult<TResult>.Success(socket.Read<TResult>());
                    results.Add(result);
                }
                while (repeatRead(result));

                return RiakResult<IEnumerable<RiakResult<TResult>>>.Success(results);
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect();
                }

                return RiakResult<IEnumerable<RiakResult<TResult>>>.FromException(ResultCode.CommunicationError, ex, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect();
                return RiakResult<IEnumerable<RiakResult<TResult>>>.FromException(ResultCode.CommunicationError, ex, true);
            }
        }

        public RiakResult PbcWrite<TRequest>(TRequest request)
            where TRequest : class
        {
            try
            {
                socket.Write(request);
                return RiakResult.Success();
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect();
                }

                return RiakResult.FromException(ResultCode.CommunicationError, ex, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect();
                return RiakResult.FromException(ResultCode.CommunicationError, ex, true);
            }
        }

        public RiakResult PbcWrite(MessageCode messageCode)
        {
            try
            {
                socket.Write(messageCode);
                return RiakResult.Success();
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect();
                }

                return RiakResult.FromException(ResultCode.CommunicationError, ex, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect();
                return RiakResult.FromException(ResultCode.CommunicationError, ex, true);
            }
        }

        public RiakResult<TResult> PbcWriteRead<TRequest, TResult>(TRequest request)
            where TRequest : class
            where TResult : class, new()
        {
            var writeResult = PbcWrite(request);
            if (writeResult.IsSuccess)
            {
                return PbcRead<TResult>();
            }

            return new RiakResult<TResult>(writeResult);
        }

        public RiakResult PbcWriteRead<TRequest>(TRequest request, MessageCode expectedMessageCode)
            where TRequest : class
        {
            var writeResult = PbcWrite(request);
            if (writeResult.IsSuccess)
            {
                return PbcRead(expectedMessageCode);
            }

            return writeResult;
        }

        public RiakResult<TResult> PbcWriteRead<TResult>(MessageCode messageCode)
            where TResult : class, new()
        {
            var writeResult = PbcWrite(messageCode);
            if (writeResult.IsSuccess)
            {
                return PbcRead<TResult>();
            }

            return new RiakResult<TResult>(writeResult);
        }

        public RiakResult PbcWriteRead(MessageCode messageCode, MessageCode expectedMessageCode)
        {
            var writeResult = PbcWrite(messageCode);
            if (writeResult.IsSuccess)
            {
                return PbcRead(expectedMessageCode);
            }

            return writeResult;
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteRead<TRequest, TResult>(
            TRequest request,
            Func<RiakResult<TResult>, bool> repeatRead)
            where TRequest : class
            where TResult : class, new()
        {
            var writeResult = PbcWrite(request);

            if (writeResult.IsSuccess)
            {
                return PbcRepeatRead(repeatRead);
            }

            return new RiakResult<IEnumerable<RiakResult<TResult>>>(writeResult);
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteRead<TResult>(
            MessageCode messageCode,
            Func<RiakResult<TResult>, bool> repeatRead)
            where TResult : class, new()
        {
            var writeResult = PbcWrite(messageCode);

            if (writeResult.IsSuccess)
            {
                return PbcRepeatRead(repeatRead);
            }

            return new RiakResult<IEnumerable<RiakResult<TResult>>>(writeResult);
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcStreamRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead, Action onFinish)
            where TResult : class, new()
        {
            var streamer = PbcStreamReadIterator(repeatRead, onFinish);
            return RiakResult<IEnumerable<RiakResult<TResult>>>.Success(streamer);
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteStreamRead<TRequest, TResult>(
            TRequest request,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TRequest : class
            where TResult : class, new()
        {
            var streamer = PbcWriteStreamReadIterator(request, repeatRead, onFinish);
            return RiakResult<IEnumerable<RiakResult<TResult>>>.Success(streamer);
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteStreamRead<TResult>(
            MessageCode messageCode,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TResult : class, new()
        {
            var streamer = PbcWriteStreamReadIterator(messageCode, repeatRead, onFinish);
            return RiakResult<IEnumerable<RiakResult<TResult>>>.Success(streamer);
        }

        public RiakResult Execute(IRCommand command)
        {
            RiakResult executeResult = DoExecute(() => socket.Write(command));

            if (executeResult.IsSuccess)
            {
                executeResult = DoExecute(() => socket.Read(command));
            }

            return executeResult;
        }

        public void Dispose()
        {
            socket.Dispose();
            Disconnect();
        }

        public void Disconnect()
        {
            socket.Disconnect();
        }

        private RiakResult DoExecute(Func<RiakResult> socketFunc)
        {
            try
            {
                return socketFunc();
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect();
                }

                return RiakResult.FromException(ResultCode.CommunicationError, ex, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect();
                return RiakResult.FromException(ResultCode.CommunicationError, ex, true);
            }
        }

        private IEnumerable<RiakResult<TResult>> PbcWriteStreamReadIterator<TResult>(
            MessageCode messageCode,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TResult : class, new()
        {
            var writeResult = PbcWrite(messageCode);

            if (writeResult.IsSuccess)
            {
                return PbcStreamReadIterator(repeatRead, onFinish);
            }

            onFinish();

            return new[] { new RiakResult<TResult>(writeResult) };
        }

        private IEnumerable<RiakResult<TResult>> PbcStreamReadIterator<TResult>(Func<RiakResult<TResult>, bool> repeatRead, Action onFinish)
            where TResult : class, new()
        {
            RiakResult<TResult> result;

            do
            {
                result = PbcRead<TResult>();

                if (!result.IsSuccess)
                {
                    break;
                }

                yield return result;
            }
            while (repeatRead(result));

            // clean up first..
            onFinish();

            // then return the failure to the client to indicate failure
            yield return result;
        }

        private IEnumerable<RiakResult<TResult>> PbcWriteStreamReadIterator<TRequest, TResult>(
            TRequest request,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TRequest : class
            where TResult : class, new()
        {
            var writeResult = PbcWrite(request);
            if (writeResult.IsSuccess)
            {
                return PbcStreamReadIterator(repeatRead, onFinish);
            }

            onFinish();

            return new[] { new RiakResult<TResult>(writeResult) };
        }
    }
}
