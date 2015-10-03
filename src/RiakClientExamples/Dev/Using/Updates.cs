namespace RiakClientExamples.Dev.Using
{
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;

    /*
     * http://docs.basho.com/riak/latest/dev/using/updates/
     */
    public sealed class Updates : ExampleBase
    {
        [Test]
        public void ExamplePut()
        {
            id = PutCoach("seahawks", "Pete Carroll");
        }

        [Test]
        public void UpdateCoachExample()
        {
            id = PutCoach("packers", "Old Coach");
            UpdateCoach("packers", "Vince Lombardi");

            id = new RiakObjectId("siblings", "coaches", "packers");
            var getResult = client.Get(id);

            RiakObject packers = getResult.Value;
            Assert.AreEqual("Vince Lombardi", Encoding.UTF8.GetString(packers.Value));
            Assert.AreEqual(0, packers.Siblings.Count);
        }

        private RiakObjectId PutCoach(string team, string coach)
        {
            var id = new RiakObjectId("siblings", "coaches", team);
            var obj = new RiakObject(id, coach,
                RiakConstants.ContentTypes.TextPlain);
            var rslt = client.Put(obj);
            CheckResult(rslt);

            return id;
        }

        private void UpdateCoach(string team, string newCoach)
        {
            var id = new RiakObjectId("siblings", "coaches", team);
            var getResult = client.Get(id);
            CheckResult(getResult);

            RiakObject obj = getResult.Value;
            obj.SetObject<string>(newCoach, RiakConstants.ContentTypes.TextPlain);
            var rslt = client.Put(obj);
            CheckResult(rslt);
        }
    }
}
