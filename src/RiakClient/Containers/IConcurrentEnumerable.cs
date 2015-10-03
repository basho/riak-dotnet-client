namespace RiakClient.Containers
{
    internal interface IConcurrentEnumerable<T>
    {
        IConcurrentEnumerator<T> GetEnumerator();
    }
}
