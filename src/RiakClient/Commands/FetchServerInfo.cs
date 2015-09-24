namespace RiakClient.Commands
{
    using System;
    using Messages;

    /// <summary>
    /// Fetches server information from Riak
    /// </summary>
    public class FetchServerInfo : Command<ServerInfoResponse>
    {
        public override MessageCode RequestCode
        {
            get { return MessageCode.RpbGetServerInfoReq; }
        }

        public override MessageCode ResponseCode
        {
            get { return MessageCode.RpbGetServerInfoResp; }
        }

        public override Type ResponseType
        {
            get { return typeof(RpbGetServerInfoResp); }
        }

        public override RpbReq ConstructPbRequest()
        {
            return null; // NB: message code only
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = new ServerInfoResponse();
            }
            else
            {
                RpbGetServerInfoResp resp = (RpbGetServerInfoResp)response;
                var info = new ServerInfo(new RiakString(resp.node), new RiakString(resp.server_version)); 
                Response = new ServerInfoResponse(info);
            }
        }
    }
}