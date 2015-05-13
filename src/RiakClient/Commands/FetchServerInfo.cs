// <copyright file="FetchServerInfo.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

namespace RiakClient.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Util;

    /// <summary>
    /// Fetches server information from Riak
    /// </summary>
    public class FetchServerInfo : Command<ServerInfoResponse>
    {
        public override MessageCode ExpectedCode
        {
            get { return MessageCode.RpbGetServerInfoResp; }
        }

        public override RpbReq ConstructPbRequest()
        {
            return new RpbReq(MessageCode.RpbGetServerInfoReq);
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