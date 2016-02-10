namespace RiakClientExamples.Dev
{

    public interface IRepository<T> where T : IModel
    {
        T Get(string key, bool notFoundOK = false);
        string Save(T model);
    }
}