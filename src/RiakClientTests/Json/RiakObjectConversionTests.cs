namespace RiakClientTests.Json.RiakObjectConversionTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Extensions;
    using RiakClient.Models;

    [TestFixture, UnitTest]
    public class WhenStoringDataIntoRiakObjectsAsJson
    {
        [Test]
        public void ObjectsAreConvertedProperly()
        {
            var testPerson = new Person
            {
                DateOfBirth = new DateTime(1978, 12, 5, 0, 0, 0, DateTimeKind.Utc),
                Email = "oj@buffered.io",
                Name = new Name
                {
                    FirstName = "OJ",
                    Surname = "Reeves"
                },
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber
                    {
                        Number = "12345678",
                        NumberType = PhoneNumberType.Home
                    }
                }
            };

            var obj = new RiakObject("bucket", "key");
            obj.SetObject(testPerson);
            obj.Value.ShouldNotBeNull();
            obj.ContentType.ShouldEqual(RiakConstants.ContentTypes.ApplicationJson);

            var json = obj.Value.FromRiakString();
            json.ShouldEqual("{\"Name\":{\"FirstName\":\"OJ\",\"Surname\":\"Reeves\"},\"PhoneNumbers\":[{\"Number\":\"12345678\",\"NumberType\":1}],\"DateOfBirth\":\"1978-12-05T00:00:00Z\",\"Email\":\"oj@buffered.io\"}");

            var deserialisedPerson = obj.GetObject<Person>();
            deserialisedPerson.ShouldEqual(testPerson);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void NonJsonObjectsCantBeDeserialisedFromJson()
        {
            var obj = new RiakObject("bucket", "key", "{\"Name\":{\"FirstName\":\"OJ\",\"Surname\":\"Reeves\"},\"PhoneNumbers\":[{\"Number\":\"12345678\",\"NumberType\":1}],\"DateOfBirth\":\"1978-12-05T00:00:00Z\",\"Email\":\"oj@buffered.io\"}", RiakConstants.ContentTypes.TextPlain);
            obj.GetObject<Person>();
        }

        [Test]
        [Ignore("Only run this if you're interested in some perf stats for Json conversion")]
        public void JsonConversionTimerTest()
        {
            var testPerson = new Person
            {
                DateOfBirth = new DateTime(1978, 12, 5, 0, 0, 0, DateTimeKind.Utc),
                Email = "oj@buffered.io",
                Name = new Name
                {
                    FirstName = "OJ",
                    Surname = "Reeves"
                },
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber
                    {
                        Number = "12345678",
                        NumberType = PhoneNumberType.Home
                    }
                }
            };
            var obj = new RiakObject("bucket", "key");

            var sw = new Stopwatch();
            sw.Start();
            const int iterations = 1000000;

            for (var i = 0; i < iterations; ++i)
            {
                obj.SetObject(testPerson);
            }
            sw.Stop();
            Console.WriteLine("Serialisation took a total of {0} - {1} per iteration", sw.Elapsed, TimeSpan.FromTicks(sw.ElapsedTicks / iterations));

            sw.Reset();
            sw.Start();

            for (var i = 0; i < iterations; ++i)
            {
                // Don't capture result to avoid compiler warning
                obj.GetObject<Person>();
            }
            sw.Stop();
            Console.WriteLine("Deserialisation took a total of {0} - {1} per iteration", sw.Elapsed, TimeSpan.FromTicks(sw.ElapsedTicks / iterations));
        }

        [Test]
        public void CustomSerializerWillSerializeJson()
        {
            var testPerson = new Person
            {
                DateOfBirth = new DateTime(1978, 12, 5, 0, 0, 0, DateTimeKind.Utc),
                Email = "oj@buffered.io",
                Name = new Name
                {
                    FirstName = "OJ",
                    Surname = "Reeves"
                },
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber
                    {
                        Number = "12345678",
                        NumberType = PhoneNumberType.Home
                    }
                }
            };

            var sots = new SerializeObjectToString<Person>(JsonConvert.SerializeObject);

            var obj = new RiakObject("bucket", "key");
            obj.SetObject(testPerson, RiakConstants.ContentTypes.ApplicationJson, sots);
            obj.Value.ShouldNotBeNull();
            obj.ContentType.ShouldEqual(RiakConstants.ContentTypes.ApplicationJson);

            var json = obj.Value.FromRiakString();
            json.ShouldEqual("{\"Name\":{\"FirstName\":\"OJ\",\"Surname\":\"Reeves\"},\"PhoneNumbers\":[{\"Number\":\"12345678\",\"NumberType\":1}],\"DateOfBirth\":\"1978-12-05T00:00:00Z\",\"Email\":\"oj@buffered.io\"}");

            var deserialisedPerson = obj.GetObject<Person>();
            deserialisedPerson.ShouldEqual(testPerson);
        }
    }
}
