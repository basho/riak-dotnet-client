namespace ChaosMonkeyApp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using RiakClient;
    using RiakClient.Models;
    using RiakClient.Util;

    public static class Program
    {
        private static readonly IRiakEndPoint cluster;
        private static readonly CancellationTokenSource cts = new CancellationTokenSource();
        private static readonly CancellationToken ct = cts.Token;

        private static readonly TimeSpan storeDataInterval = TimeSpan.FromMilliseconds(120);
        private static readonly TimeSpan fetchDataInterval = TimeSpan.FromMilliseconds(120);

        private static volatile int key = 0;

        static Program()
        {
            cluster = RiakCluster.FromConfig("riakConfig");
        }

        static void Main(string[] args)
        {
            var tf = new TaskFactory(ct);
            var s = tf.StartNew(StoreData, ct);
            var f = tf.StartNew(FetchData, ct);
            var tasks = new[] { s, f };

            Console.WriteLine("Hit any key to stop.");
            Console.ReadLine();
            cts.Cancel();

            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException ex)
            {
                foreach (var iex in ex.InnerExceptions)
                {
                    var msg = string.Format("[ChaosMonkeyApp] tasks canceled (ex: {0})", iex.Message);
                    Console.WriteLine(msg);
                }
            }
            catch (TaskCanceledException ex)
            {
                var msg = string.Format("[ChaosMonkeyApp] tasks canceled (ex: {0})", ex.Message);
                Console.WriteLine(msg);
            }

            cluster.Dispose();
            cts.Dispose();
            Console.WriteLine("Stopped.");
        }

        private static void StoreData()
        {
            Console.WriteLine("[ChaosMonkeyApp] store thread starting");
            IRiakClient client = cluster.CreateClient();
            try
            {
                while (true)
                {
                    var id = new RiakObjectId("chaos-monkey", key.ToString());
                    var obj = new RiakObject(id, Guid.NewGuid().ToString());
                    obj.ContentEncoding = RiakConstants.CharSets.Utf8;
                    obj.ContentType = RiakConstants.ContentTypes.TextPlain;

                    var rslt = client.Put(obj);
                    if (rslt.IsSuccess)
                    {
                        Console.WriteLine("[ChaosMonkeyApp] stored key: {0}", key);
                    }
                    else
                    {
                        Console.WriteLine("[ChaosMonkeyApp] error storing key {0}, {1}", key, rslt.ErrorMessage);
                    }

                    ++key;
                    Thread.Sleep(storeDataInterval);
                    ct.ThrowIfCancellationRequested();
                }
            }
            finally
            {
                Console.WriteLine("[ChaosMonkeyApp] store thread stopping");
            }
        }

        private static void FetchData()
        {
            Console.WriteLine("[ChaosMonkeyApp] fetch thread starting");
            IRiakClient client = cluster.CreateClient();
            var r = new Random((int)DateTimeUtil.ToUnixTimeMillis(DateTime.Now));
            try
            {
                while (true)
                {
                    int k = r.Next(0, key);
                    var id = new RiakObjectId("chaos-monkey", k.ToString());
                    var rslt = client.Get(id);
                    if (rslt.IsSuccess)
                    {
                        Console.WriteLine("[ChaosMonkeyApp] got key: {0}", k);
                    }
                    else
                    {
                        Console.WriteLine("[ChaosMonkeyApp] error getting key {0}, {1}", k, rslt.ErrorMessage);
                    }
                    Thread.Sleep(fetchDataInterval);
                    ct.ThrowIfCancellationRequested();
                }
            }
            finally
            {
                Console.WriteLine("[ChaosMonkeyApp] fetch thread stopping");
            }
        }
    }
}
