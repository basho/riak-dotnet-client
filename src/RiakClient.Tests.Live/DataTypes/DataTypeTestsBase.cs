using System.Text;
using RiakClient.Models;

namespace RiakClient.Tests.Live.DataTypes
{
    public class DataTypeTestsBase : LiveRiakConnectionTestBase
    {
        protected const string Bucket = "riak_dt_bucket";
        protected readonly DeserializeObject<string> Deserializer = (b, type) => Encoding.UTF8.GetString(b);
        protected readonly SerializeObjectToByteArray<string> Serializer = s => Encoding.UTF8.GetBytes(s);
    }
}