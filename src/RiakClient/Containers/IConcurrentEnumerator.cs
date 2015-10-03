namespace RiakClient.Containers
{
    internal interface IConcurrentEnumerator<T>
    {
        bool TryMoveNext(out T next);
    }
}
