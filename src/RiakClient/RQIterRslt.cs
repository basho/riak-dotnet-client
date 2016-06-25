namespace Riak
{
    internal class RQIterRslt
    {
        private readonly bool @break;
        private readonly bool requeue;

        public RQIterRslt(bool @break, bool requeue)
        {
            this.@break = @break;
            this.requeue = requeue;
        }

        public bool Break
        {
            get { return @break; }
        }

        public bool ReQueue
        {
            get { return requeue; }
        }
    }
}
