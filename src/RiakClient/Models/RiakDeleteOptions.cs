namespace RiakClient.Models
{
    using Messages;

    /// <summary>
    /// A collection of optional settings for deleting objects from Riak.
    /// </summary>
    public class RiakDeleteOptions : RiakOptions<RiakDeleteOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakDeleteOptions" /> class.
        /// Uses the "default" quorum settings for R, W, PR, PW, DW, and RW settings.
        /// </summary>
        public RiakDeleteOptions()
        {
            R = Quorum.WellKnown.Default;
            W = Quorum.WellKnown.Default;
            Pr = Quorum.WellKnown.Default;
            Pw = Quorum.WellKnown.Default;
            Dw = Quorum.WellKnown.Default;
            Rw = Quorum.WellKnown.Default;
        }

        /// <summary>
        /// The Vclock of the version that is being deleted. 
        /// Use this to prevent deleting objects that have been modified since the last get request.
        /// </summary>
        /// <value>
        /// The vclock.
        /// </value>
        /// <remarks>
        /// Review the information at http://wiki.basho.com/Vector-Clocks.html for additional information on how vector clocks 
        /// are used in Riak.
        /// </remarks>
        public byte[] Vclock { get; set; }

        internal void Populate(RpbDelReq request)
        {
            request.r = R;
            request.pr = Pr;
            request.rw = Rw;
            request.w = W;
            request.pw = Pw;
            request.dw = Dw;
            request.timeout = (uint)Timeout.TotalMilliseconds;
            if (Vclock != null)
            {
                request.vclock = Vclock;
            }
        }
    }
}
