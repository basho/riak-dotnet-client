// <copyright file="ExampleBase.cs" company="Basho Technologies, Inc.">
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

#pragma warning disable 618

namespace RiakClientExamples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;
    using RiakClient.Models.RiakDt;
    using RiakClient.Util;

    public abstract class ExampleBase : IDisposable
    {
        private readonly IRiakEndPoint endpoint;

        protected static readonly SerializeObjectToByteArray<string> Serializer =
            s => Encoding.UTF8.GetBytes(s);
        protected static readonly DeserializeObject<string> Deserializer =
            (b, type) => Encoding.UTF8.GetString(b);

        protected IRiakClient client;
        protected RiakObjectId id;
        protected ICollection<RiakObjectId> ids;

        public ExampleBase()
        {
            endpoint = RiakCluster.FromConfig("riakConfig");
        }

        [SetUp]
        public void CreateClient()
        {
            client = endpoint.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
#if CLEANUP
            if (id != null)
            {
                DeleteObject(id);
            }

            if (EnumerableUtil.NotNullOrEmpty(ids))
            {
                DeleteObjects(ids);
            }
#endif
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && endpoint != null)
            {
                endpoint.Dispose();
            }
        }

        protected void CheckResult(RiakResult riakResult, bool errorIsOK = false)
        {
            if (errorIsOK && !riakResult.IsSuccess)
            {
                Console.WriteLine("Error: {0}", riakResult.ErrorMessage);
            }
            else
            {
                Assert.IsTrue(riakResult.IsSuccess, "Error: {0}", riakResult.ErrorMessage);
            }
        }

        protected void DeleteObject(RiakObjectId id)
        {
            CheckResult(client.Delete(id));
        }

        protected void DeleteObjects(IEnumerable<RiakObjectId> ids)
        {
            foreach (var id in ids)
            {
                DeleteObject(id);
            }
        }

        protected void WaitForSearch()
        {
            Thread.Sleep(1250);
        }

        protected void PrintMapValues(IEnumerable<RiakDtMapEntry> mapEntries, int depth = 0)
        {
            string spaces = string.Concat(Enumerable.Repeat(" ", depth * 4));
            foreach (RiakDtMapEntry mapEntry in mapEntries)
            {
                var args = new List<string> { spaces };
                RiakDtMapField field = mapEntry.Field;
                args.Add(field.Name);
                switch (field.Type)
                {
                    case RiakDtMapField.RiakDtMapFieldType.Map:
                        Console.WriteLine("{0}Map: {1}", args.ToArray());
                        PrintMapValues(mapEntry.MapValue, depth + 1);
                        break;
                    case RiakDtMapField.RiakDtMapFieldType.Register:
                        args.Add(Deserializer(mapEntry.RegisterValue));
                        Console.WriteLine("{0}{1}: {2}", args.ToArray());
                        break;
                    case RiakDtMapField.RiakDtMapFieldType.Flag:
                        args.Add(mapEntry.FlagValue.Value.ToString());
                        Console.WriteLine("{0}{1}: {2}", args.ToArray());
                        break;
                    case RiakDtMapField.RiakDtMapFieldType.Counter:
                        args.Add(mapEntry.Counter.Value.ToString());
                        Console.WriteLine("{0}{1}: {2}", args.ToArray());
                        break;
                    case RiakDtMapField.RiakDtMapFieldType.Set:
                        foreach (var setValue in mapEntry.SetValue)
                        {
                            var setArgs = new List<string>();
                            setArgs.AddRange(args);
                            setArgs.Add(Deserializer(setValue));
                            Console.WriteLine("{0}{1}: {2}", setArgs.ToArray());
                        }
                        break;
                    default:
                        Assert.Fail("Unexpected field type: {0}", field.Type);
                        Debug.Fail("Map Error",
                            string.Format("Unexpected field type: {0}", field.Type));
                        break;
                }
            }
        }
    }
}

#pragma warning restore 618