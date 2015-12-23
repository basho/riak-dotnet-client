namespace Riak.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;
    using RiakClient.Commands;

    public sealed class LeastExecutingNodeManager : NodeManager, INodeManager, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger<LeastExecutingNodeManager>();

        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();
        private readonly bool shuffle = false;

        public LeastExecutingNodeManager(bool shuffle) : base(Log)
        {
            this.shuffle = shuffle;
        }

        public async Task<ExecuteResult> ExecuteAsyncOnNode(IList<INode> nodes, IRCommand cmd, INode previous = null)
        {
            Validate(nodes, cmd);

            var n = new List<INode>(nodes);
            n.Sort((a, b) => a.ExecuteCount - b.ExecuteCount);

            if (shuffle)
            {
                var j = 0;
                for (var i = 0; i < (n.Count - 1); i++)
                {
                    j = i + 1;
                    if (n[j].ExecuteCount > n[i].ExecuteCount)
                    {
                        break;
                    }
                }

                if (j > 1)
                {
                    var s = ShuffleArray(n.Take(j).ToList());
                    n = s.Concat(n.Skip(j)).ToList();
                }
            }

            var rslt = new ExecuteResult(executed: false);

            foreach (var node in n)
            {
                rslt = await ExecuteAsync(node, cmd);
                if (rslt.Executed)
                {
                    break;
                }
            }

            return rslt;
        }

        public void Dispose()
        {
            sync.Dispose();
        }

        /*
         * Randomize array element order in-place.
         * Using Durstenfeld shuffle algorithm.
         * http://stackoverflow.com/a/12646864
         */
        private static IList<T> ShuffleArray<T>(IList<T> array)
        {
            var r = new Random((int)DateTime.Now.ToBinary());

            for (var i = array.Count - 1; i > 0; i--)
            {
                int j = (int)Math.Floor(r.NextDouble() * (i + 1));
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }

            return array;
        }
    }
}
