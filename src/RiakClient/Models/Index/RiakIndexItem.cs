namespace RiakClient.Models.Index
{
    public class RiakIndexItem 
    {
        public string Key { get; private set; }
        public string Term { get; private set; }

        public RiakIndexItem(string key)
        {
            Key = key;
        }

        public RiakIndexItem(string key, string term) : this(key)
        {
            Term = term;
        }
    }
}
