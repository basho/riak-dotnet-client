namespace CorrugatedIron.Config
{
    public interface IRiakConnectionConfiguration
    {
        string HostAddress { get; }
        int HostPort { get; }
        int PoolSize { get; }
        int AcquireTimeout { get; }
        int IdleTimeout { get; }
    }
}
