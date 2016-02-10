namespace RiakClientTests.Models.MapReduce.Inputs
{
    using NUnit.Framework;
    using RiakClient.Models.MapReduce.Inputs;

    [TestFixture, UnitTest]
    public class RiakModuleFunctionArgInputSerializationTest : MapReduceSerializationTestsBase
    {
        [Test]
        public void EnsureMFAInputSeralizesCorrectly()
        {
            var input = new RiakModuleFunctionArgInput("my_mod", "my_fun", new[] { "arg1", "arg2", "arg3" });

            var json = Serialize(input.WriteJson);
            Assert.AreEqual("\"inputs\":{\"module\":\"my_mod\",\"function\":\"my_fun\",\"arg\":[\"arg1\",\"arg2\",\"arg3\"]}", json);
        }

        [Test]
        public void EnsureMFAInputWithNoArgsSeralizesCorrectly()
        {
            var input = new RiakModuleFunctionArgInput("my_mod", "my_fun", new string[]{});

            var json = Serialize(input.WriteJson);
            Assert.AreEqual("\"inputs\":{\"module\":\"my_mod\",\"function\":\"my_fun\",\"arg\":[]}", json);
        }
    }
}
