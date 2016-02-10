namespace RiakClientExamples
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Models;
    using RiakClient.Util;

    [Examples]
    public abstract class ExampleBase : IDisposable
    {
        private readonly IRiakEndPoint endpoint;

        protected IRiakClient client;
        protected RiakObjectId id;
        protected ICollection<RiakObjectId> ids;

        protected FetchCommandOptions options;

        public ExampleBase()
        {
            endpoint = RiakCluster.FromConfig("riakConfig");
        }

        [SetUp]
        public void CreateClient()
        {
            client = endpoint.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
#if CLEANUP
            if (id != null)
            {
                DeleteObject(id);
            }

            if (EnumerableUtil.NotNullOrEmpty(ids))
            {
                DeleteObjects(ids);
            }

            if (options != null)
            {
                id = new RiakObjectId(options.BucketType, options.Bucket, options.Key);
                DeleteObject(id);
            }
#endif
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && endpoint != null)
            {
                endpoint.Dispose();
            }
        }

        protected void CheckResult(RiakResult riakResult, bool errorIsOK = false)
        {
            if (errorIsOK && !riakResult.IsSuccess)
            {
                Console.WriteLine("Error: {0}", riakResult.ErrorMessage);
            }
            else
            {
                Assert.IsTrue(riakResult.IsSuccess, "Error: {0}", riakResult.ErrorMessage);
            }
        }

        protected void DeleteObject(RiakObjectId id)
        {
            CheckResult(client.Delete(id));
        }

        protected void DeleteObjects(IEnumerable<RiakObjectId> ids)
        {
            foreach (var id in ids)
            {
                DeleteObject(id);
            }
        }

        protected void WaitForSearch()
        {
            Thread.Sleep(1250);
        }

        protected static void PrintObject(object obj)
        {
            var converter = new ByteArrayAsStringConverter();
            Console.WriteLine("Object: {0}", JsonConvert.SerializeObject(obj, converter));
        }
    }
}