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

using CorrugatedIron.Config;
using CorrugatedIron.Tests.Extensions;
using NUnit.Framework;
using System.IO;

namespace CorrugatedIron.Tests.Live.RiakConfigurationTests
{
    [TestFixture]
    public class WhenLoadingFromExternalConfiguration
    {
        private const string SampleConfig = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration><configSections><section name=""riakConfig"" type=""CorrugatedIron.Config.RiakClusterConfiguration, CorrugatedIron"" />
</configSections><riakConfig nodePollTime=""5000"" defaultRetryWaitTime=""200"" defaultRetryCount=""3"">
<nodes><node name=""node"" hostAddress=""host"" pbcPort=""8081"" restScheme=""http"" restPort=""8091"" poolSize=""5"" /></nodes></riakConfig></configuration>";

        [Test]
        public void ConfigurationLoadsProperly()
        {
            var fileName = Path.GetTempFileName();
            try
            {
                File.WriteAllText(fileName, SampleConfig);

                var config = RiakClusterConfiguration.LoadFromConfig("riakConfig", fileName);
                config.DefaultRetryCount.ShouldEqual(3);
                config.DefaultRetryWaitTime.ShouldEqual(200);
                config.NodePollTime.ShouldEqual(5000);
                config.RiakNodes.Count.ShouldEqual(1);
                config.RiakNodes[0].HostAddress.ShouldEqual("host");
                config.RiakNodes[0].Name.ShouldEqual("node");
                config.RiakNodes[0].PbcPort.ShouldEqual(8081);
                config.RiakNodes[0].RestScheme.ShouldEqual("http");
                config.RiakNodes[0].RestPort.ShouldEqual(8091);
                config.RiakNodes[0].PoolSize.ShouldEqual(5);
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
