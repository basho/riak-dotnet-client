namespace RiakClientTests.Client
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using RiakClient;
    using RiakClient.Comms;

    public sealed class MockCluster : IRiakEndPoint
    {
        public Mock<IRiakConnection> ConnectionMock = new Mock<IRiakConnection>();

        public MockCluster()
        {
            RetryWaitTime = TimeSpan.FromMilliseconds(200);
        }

        public void Dispose()
        {
        }

        public IRiakClient CreateClient()
        {
            return new Mock<IRiakClient>().Object;
        }

        public IRiakClient CreateClient(string seed)
        {
            return new Mock<IRiakClient>().Object;
        }

        public TimeSpan RetryWaitTime { get; set; }

        public RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts)
        {
            return useFun(ConnectionMock.Object);
        }

        public RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, int retryAttempts)
        {
            return useFun(ConnectionMock.Object);
        }

        public RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult
        {
            throw new NotImplementedException();
        }
    }
}
