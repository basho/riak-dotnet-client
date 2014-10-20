namespace CorrugatedIron
{
    public enum ResultCode
    {
        Success = 0,
        ShuttingDown,
        NotFound,
        CommunicationError,
        InvalidResponse,
        ClusterOffline,
        NoConnections,
        BatchException,
        NoRetries,
        HttpError,
        InvalidRequest
    }
}