using System;
using System.Diagnostics;
using System.Text;
using RiakClient.Models;

namespace RiakClient.Tests.Live.DataTypes
{
    public class DataTypeTestsBase : LiveRiakConnectionTestBase
    {
        protected const string Bucket = "riak_dt_bucket";
        protected readonly DeserializeObject<string> Deserializer = (b, type) => Encoding.UTF8.GetString(b);
        protected readonly SerializeObjectToByteArray<string> Serializer = s => Encoding.UTF8.GetBytes(s);

        //TODO: use CallerMemberNameAttribute when we move to .Net 4.5
        protected string GetRandomKey(string memberName = "")
        {
            if (string.IsNullOrEmpty(memberName))
            {
                var frame = new StackFrame(1);
                memberName = frame.GetMethod().Name;
            }
            var key = string.Format("{0}_{1}", memberName, Random.Next());
            Console.WriteLine("Using {0} for {1}() key", key, memberName);
            return key;
        }
    }
}