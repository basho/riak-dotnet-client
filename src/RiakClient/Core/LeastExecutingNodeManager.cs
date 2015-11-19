namespace Riak.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;
    using RiakClient.Commands;

    public class LeastExecutingNodeManager : NodeManager, INodeManager, IDisposable
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
                    var s = shuffleArray(n, 0, j);
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

        private static IList<T> shuffleArray<T>(IList<T> a, int start, int end)
        {
            var r = new Random((int)DateTime.Now.ToBinary());

            for (var i = a.Count - 1; i > 0; i--)
            {
                // TODO between 0 and 1 ?? compare with Node.js random()
                var j = Math.Floor(r.NextDouble() * (i + 1));
            }
        }
    }
}
