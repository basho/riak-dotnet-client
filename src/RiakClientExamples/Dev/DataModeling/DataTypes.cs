namespace RiakClientExamples.Dev.DataModeling
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    /*
     * http://docs.basho.com/riak/latest/dev/data-modeling/data-types/
     */
    public sealed class DataTypes : ExampleBase
    {
        [Test]
        public void UserExample()
        {
            var interests = new HashSet<string> { "distributed systems", "Erlang" };
            var joe = new User("Joe", "Armstrong", interests);

            var entityManager = new EntityManager(client);
            entityManager.Add(joe);
            var repo = new UserRepository(client);
            repo.Save(joe);

            joe.VisitPage();

            joe.AddInterest("riak");

            repo.UpgradeAccount(joe);

            var joeFetched = repo.Get(joe.ID);

            Assert.GreaterOrEqual(joe.PageVisits, 0);
            Assert.Contains("riak", joeFetched.Interests.ToArray());

            PrintObject(joeFetched);

            repo.DowngradeAccount(joe);

            joeFetched = repo.Get(joe.ID);
            PrintObject(joeFetched);
        }
    }
}