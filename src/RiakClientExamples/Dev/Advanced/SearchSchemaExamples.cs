namespace RiakClientExamples.Dev.Using.Advanced
{
    using System.IO;
    using NUnit.Framework;
    using RiakClient.Models.Search;

    /*
     * http://docs.basho.com/riak/latest/dev/advanced/search-schema/
     */
    public sealed class SearchSchemaExamples : ExampleBase
    {
        [Test, Ignore("Requires cartoons.xml to be present")]
        public void CustomSchema()
        {
            var xml = File.ReadAllText("cartoons.xml");
            var schema = new SearchSchema("cartoons", xml);
            var rslt = client.PutSearchSchema(schema);
            CheckResult(rslt);
        }
    }
}