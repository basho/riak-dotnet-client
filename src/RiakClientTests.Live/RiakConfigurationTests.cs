// <copyright file="RiakConfigurationTests.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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

namespace RiakClientTests.Live.RiakConfigurationTests
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using RiakClient.Config;
    using Extensions;

    [TestFixture]
    public class WhenLoadingFromExternalConfiguration
    {
        private const string SampleConfig = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
            <configuration>
              <configSections>
                <section name=""riakConfig"" type=""RiakClient.Config.RiakClusterConfiguration, RiakClient"" />
              </configSections>
              <riakConfig nodePollTime=""5000"" defaultRetryWaitTime=""200"" defaultRetryCount=""3"">
                <nodes>
                  <node name=""node1"" hostAddress=""host1"" pbcPort=""8081"" restScheme=""http"" restPort=""8091"" poolSize=""5"" />
                  <node name=""node2"" hostAddress=""host2"" pbcPort=""8081"" restScheme=""http"" restPort=""8091"" poolSize=""6""
                        networkReadTimeout=""5000"" networkWriteTimeout=""5000"" networkConnectTimeout=""5000"" />
                </nodes>
              </riakConfig>
            </configuration>
            ";

        [Test]
        public void ConfigurationLoadsProperly()
        {
            var twoHundredMillis = TimeSpan.FromMilliseconds(200);
            var fourSecsAsMillis = TimeSpan.FromMilliseconds(4000);
            var fiveSecsAsMillis = TimeSpan.FromMilliseconds(5000);

            var fileName = Path.GetTempFileName();
            try
            {
                File.WriteAllText(fileName, SampleConfig);

                var config = RiakClusterConfiguration.LoadFromConfig("riakConfig", fileName);
                config.DefaultRetryCount.ShouldEqual(3);
                config.DefaultRetryWaitTime.ShouldEqual(twoHundredMillis);
                config.NodePollTime.ShouldEqual(fiveSecsAsMillis);
                config.RiakNodes.Count.ShouldEqual(2);

                IRiakNodeConfiguration node1 = config.RiakNodes[0];
                node1.Name.ShouldEqual("node1");
                node1.HostAddress.ShouldEqual("host1");
                node1.PbcPort.ShouldEqual(8081);
                node1.RestScheme.ShouldEqual("http");
                node1.RestPort.ShouldEqual(8091);
                node1.PoolSize.ShouldEqual(5);
                node1.NetworkConnectTimeout.ShouldEqual(fourSecsAsMillis);
                node1.NetworkReadTimeout.ShouldEqual(fourSecsAsMillis);
                node1.NetworkWriteTimeout.ShouldEqual(fourSecsAsMillis);

                IRiakNodeConfiguration node2 = config.RiakNodes[1];
                node2.Name.ShouldEqual("node2");
                node2.HostAddress.ShouldEqual("host2");
                node2.PbcPort.ShouldEqual(8081);
                node2.RestScheme.ShouldEqual("http");
                node2.RestPort.ShouldEqual(8091);
                node2.PoolSize.ShouldEqual(6);
                node2.NetworkConnectTimeout.ShouldEqual(fiveSecsAsMillis);
                node2.NetworkReadTimeout.ShouldEqual(fiveSecsAsMillis);
                node2.NetworkWriteTimeout.ShouldEqual(fiveSecsAsMillis);
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
