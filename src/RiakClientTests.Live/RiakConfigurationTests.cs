namespace RiakClientTests.Live.RiakConfigurationTests
{
    using System;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    using Riak.Config;

    [TestFixture, IntegrationTest]
    public class WhenLoadingFromExternalConfiguration
    {
        private const string Cfg1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
            <configuration>
              <configSections>
                <section name=""riakConfig"" type=""Riak.Config.ClusterConfiguration, RiakClient"" />
              </configSections>
              <riakConfig nodePollTime=""5000"" defaultRetryWaitTime=""200"" defaultRetryCount=""3"">
                <nodes>
                  <node name=""node1"" hostAddress=""host1"" pbcPort=""8081"" poolSize=""5"" externalLoadBalancer=""true"" />
                  <node name=""node2"" hostAddress=""host2"" pbcPort=""8081"" poolSize=""6""
                        networkReadTimeout=""5000"" networkWriteTimeout=""5000"" networkConnectTimeout=""5000"" />
                </nodes>
              </riakConfig>
            </configuration>
            ";

        private const string Cfg2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
            <configuration>
              <configSections>
                <section name=""riakConfig"" type=""RiakClient.Config.RiakClusterConfiguration, RiakClient"" />
              </configSections>
              <riakConfig>
                <nodes>
                  <node name=""node"" hostAddress=""127.0.0.1"" />
                </nodes>
              </riakConfig>
            </configuration>
            ";

        [Test]
        public void ConfigurationLoadsProperly()
        {
            TimeSpan twoHundredMillis = TimeSpan.FromMilliseconds(200);
            TimeSpan fourSecsAsMillis = TimeSpan.FromMilliseconds(4000);
            TimeSpan fiveSecsAsMillis = TimeSpan.FromMilliseconds(5000);

            var fileName = Path.GetTempFileName();
            try
            {
                File.WriteAllText(fileName, Cfg1);

                var config = ClusterConfiguration.LoadFromConfig("riakConfig", fileName);
                config.DefaultRetryCount.ShouldEqual(3);
                config.DefaultRetryWaitTime.ShouldEqual(twoHundredMillis);
                config.NodePollTime.ShouldEqual(fiveSecsAsMillis);
                config.RiakNodes.Count().ShouldEqual(2);

                var nodes = config.RiakNodes.ToArray();
                INodeConfiguration node1 = nodes[0];
                node1.Name.ShouldEqual("node1");
                node1.HostAddress.ShouldEqual("host1");
                node1.PbcPort.ShouldEqual(8081);
                node1.PoolSize.ShouldEqual(5);
                node1.NetworkConnectTimeout.ShouldEqual(fourSecsAsMillis);
                node1.NetworkReadTimeout.ShouldEqual(fourSecsAsMillis);
                node1.NetworkWriteTimeout.ShouldEqual(fourSecsAsMillis);

                INodeConfiguration node2 = nodes[1];
                node2.Name.ShouldEqual("node2");
                node2.HostAddress.ShouldEqual("host2");
                node2.PbcPort.ShouldEqual(8081);
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

        [Test]
        public void ConfigurationLoadsDefaults()
        {
            var fileName = Path.GetTempFileName();
            try
            {
                File.WriteAllText(fileName, Cfg2);

                var config = ClusterConfiguration.LoadFromConfig("riakConfig", fileName);
                Assert.AreEqual(3, config.DefaultRetryCount);
                Assert.AreEqual(200, (int)config.DefaultRetryWaitTime.TotalMilliseconds);
                Assert.AreEqual(5000, (int)config.NodePollTime.TotalMilliseconds);
                Assert.AreEqual(1, config.RiakNodes.Count());

                var nodes = config.RiakNodes.ToArray();
                INodeConfiguration node = nodes[0];
                Assert.AreEqual("node", node.Name);
                Assert.AreEqual("127.0.0.1", node.HostAddress);
                Assert.AreEqual(8087, node.PbcPort);
                Assert.AreEqual(30, node.PoolSize);
                Assert.AreEqual(4000, (int)node.NetworkConnectTimeout.TotalMilliseconds);
                Assert.AreEqual(4000, (int)node.NetworkReadTimeout.TotalMilliseconds);
                Assert.AreEqual(4000, (int)node.NetworkWriteTimeout.TotalMilliseconds);
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
