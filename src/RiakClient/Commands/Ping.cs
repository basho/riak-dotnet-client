namespace RiakClient.Commands
{
    using System;
    using Messages;

    /// <summary>
    /// Ping Riak
    /// </summary>
    public class Ping : Command<Response<bool>>
    {
        public override MessageCode RequestCode
        {
            get { return MessageCode.RpbPingReq; }
        }

        public override MessageCode ResponseCode
        {
            get { return MessageCode.RpbPingResp; }
        }

        public override Type ResponseType
        {
            get { return null; }
        }

        public override RpbReq ConstructPbRequest()
        {
            return null; // NB: message code only
        }

        public override void OnSuccess(RpbResp response)
        {
            Response = new Response<bool>(true);
        }

        public class Builder : CommandBuilder<Builder, Ping>
        {
            public override IRCommand Build()
            {
                return new Ping();
            }
        }
    }
}
