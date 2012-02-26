# Release Notes

## CorrugatedIron 1.0 Release Notes

### RiakSearch MapReduce Input

RiakSearch has been added as a [MapReduce input type](https://github.com/DistributedNonsense/CorrugatedIron/issues/35). 

### DeleteBucket() accepts RiakDeleteOptions

This closes [Issue 32: DeleteBucket() overload which takes a RiakDeleteOptions instance](https://github.com/DistributedNonsense/CorrugatedIron/issues/32)

`DeleteBucket(string Bucket)` is a convenience method provided to make it easy to clear out a bucket by performing a list keys operation followed by a delete. This shouldn't be used in production, but it has been found to be exceptionally convenient in test scenarios. DeleteBucket now accepts a RiakDeleteOptions parameter allowing a developer to specify the usual range of R, W, DW values for a delete operation.

### Removed Either<uint, string> Construct

The Either<TLeft, TRight> Construct existed to allow users to pass in either an integer or string for quorum commands. Riak only allows a uint to be passed in. Rather than resort to string parsing, we've removed the Either<TLeft, TRight> construct and replaced it with good, old-fashioned constants. Please see `RiakConstants.QuorumOptions`.

### Custom Serializers

This closes [Issue 16: Implement GetObject<T> and SetObject<T> with custom SerDe](https://github.com/DistributedNonsense/CorrugatedIron/issues/16) and [Issue 34: option to use ServiceStack json serializer](https://github.com/DistributedNonsense/CorrugatedIron/issues/34).

Custom serializers have been added to the `GetObject<T>` and `SetObject<T>` methods of the `RiakObject`. Specifically, several delegates have been provided that make it easy for developers to provide custom methods to convert objects. We're aware of performance problems with the Newtonsoft JSON implementation for large JSON objects and believe that this offers a good way forward for developers without breaking any existing applications. Existing code will still use the Newtonsoft JSON libraries and new code can opt to use the delegates. The three delegates are:

    public delegate string SerializeObjectToString<in T>(T theObject);
	public delegate byte[] SerializeObjectToByteArray<in T>(T theObject);
	public delegate T DeserializeObject<out T>(byte[] theObject, string contentType);

An example of how to use these delegates can be found in either the `CorrugatedIron.Tests.Json.RiakObjectConversionTests` namespace or reproduced below:

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
		json.ShouldEqual("{\"Name\":{\"FirstName\":\"OJ\",\"Surname\":\"Reeves\"},\"PhoneNumbers\":[{\"Number\":\"12345678\",\"NumberType\":1}],\"DateOfBirth\":\"\\/Date(281664000000)\\/\",\"Email\":\"oj@buffered.io\"}");

		var deserialisedPerson = obj.GetObject<Person>();
		deserialisedPerson.ShouldEqual(testPerson);
	}

### Updated Library Support for NuGet

Closes [Issue 33: Nuget pulls Newtonsoft.Json 4.0.5.0](https://github.com/DistributedNonsense/CorrugatedIron/issues/33).

NuGet is, as Bryan Hunter put it, an eager beaver when it comes to pulling in new versions of libraries. We have pinned CorrugatedIron to Newtonsoft.Json 4.0.5.0 and protobuf-net 2.0.0.480.

### Riak 1.0 && 1.1 Support

We have verified that CorrugatedIron runs with Riak 1.0 and 1.1 - this includes support for the legacy vector clock behavior as well as current vector clock behavior and all new feature (e.g. MapReduce jobs no long require a Map or Reduce phase and can simply be a Secondary Index query).

### RiakSearch and SOLR

SOLR support was originally implemented, scrapped, and implemented again. Then it was scrapped in favor of RiakSearch. 

We have focused primarily on the Protocol Buffer API for performance reasons and will continue to do so. Support is present for HTTP-based requests to Riak but, at the present, it is not on our roadmap to implement full SOLR support for Riak. We can always be persuaded through pleading.