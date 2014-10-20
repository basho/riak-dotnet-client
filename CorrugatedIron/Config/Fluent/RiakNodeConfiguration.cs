// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Extensions;
using System;

namespace CorrugatedIron.Config.Fluent
{
    public class RiakNodeConfiguration : IRiakNodeConfiguration
    {
        private string _name;
        private string _hostAddress;
        private int _pbcPort = 8088;
        private string _restScheme = "http";
        private int _restPort = 8098;
        private int _poolSize = 30;
        private int _idleTimeout = 2000;
        private int _networkReadTimeout = 2000;
        private int _networkWriteTimeout = 2000;
        private bool _vnodeVclocks = true;
        private int _bufferSize = 2097152;

        public string Name
        {
            get { return _name; }
        }

        public string HostAddress
        {
            get { return _hostAddress; }
        }

        public int PbcPort
        {
            get { return _pbcPort; }
        }

        public string RestScheme
        {
            get { return _restScheme; }
        }

        public int RestPort
        {
            get { return _restPort; }
        }

        public int PoolSize
        {
            get { return _poolSize; }
        }

        public int BufferSize
        {
            get { return _bufferSize; } 
        }

        public int IdleTimeout
        {
            get { return _idleTimeout; }
        }

        public int NetworkReadTimeout
        {
            get { return _networkReadTimeout; }
        }

        public int NetworkWriteTimeout
        {
            get { return _networkWriteTimeout; }
        }

        public bool VnodeVclocks
        {
            get { return _vnodeVclocks; }
        }

        public RiakNodeConfiguration SetName(string name)
        {
            _name = name;
            return this;
        }

        public RiakNodeConfiguration SetHostAddress(string hostAddress)
        {
            _hostAddress = hostAddress;
            return this;
        }

        public RiakNodeConfiguration SetPbcPort(int pbcPort)
        {
            _pbcPort = pbcPort;
            return this;
        }

        public RiakNodeConfiguration SetRestScheme(string restScheme)
        {
            _restScheme = restScheme.ToLower();

            if(_restScheme != "http" && _restScheme != "https")
            {
                throw new NotSupportedException("Riak's REST interface doesn't support the scheme '{0}'. Please specify 'http' or 'https'.".Fmt(restScheme));
            }

            return this;
        }

        public RiakNodeConfiguration SetRestPort(int restPort)
        {
            _restPort = restPort;
            return this;
        }

        public RiakNodeConfiguration SetPoolSize(int poolSize)
        {
            _poolSize = poolSize;
            return this;
        }

        public RiakNodeConfiguration SetNetworkReadTimeout(int networkReadTimeout)
        {
            _networkReadTimeout = networkReadTimeout;
            return this;
        }

        public RiakNodeConfiguration SetIdleTimeout(int idleTimeout)
        {
            _idleTimeout = idleTimeout;
            return this;
        }

        public RiakNodeConfiguration SetNetworkWriteTimeout(int networkWriteTimeout)
        {
            _networkWriteTimeout = networkWriteTimeout;
            return this;
        }

        public RiakNodeConfiguration SetVnodeVclocks(bool vnodeVclocks)
        {
            _vnodeVclocks = vnodeVclocks;
            return this;
        }

        public RiakNodeConfiguration SetBufferSize(int bufferSize)
        {
            _bufferSize = bufferSize;
            return this;
        }
    }
}
