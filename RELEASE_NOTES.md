Release Notes
=============

v1.2.1
------

### Fixes

* [Issue 103](https://github.com/DistributedNonsense/CorrugatedIron/issues/103) - Add ListKeysFromIndex to IRiakClient.
* [Issue 100](https://github.com/DistributedNonsense/CorrugatedIron/issues/100) - Slight refactor of Batch() to use TPL.
* [Issue 94](https://github.com/DistributedNonsense/CorrugatedIron/issues/94) - Riak 1.3 Reset Bucket Properties functionality.

v1.2.0
------

### Features

We have made significant improvements in working with indexes as well as adding the following API changes:

* We added two new APIs to the `RiakIndex` for Map/Reduce inputs - `AllKeys` and `Keys`. `AllKeys` uses the `$bucket` index to pass all keys into a Map/Reduce job. `Keys` uses the `$key` index to pass a list of keys in a range into an Map/Reduce job.
* We added four `IndexGet` methods that are wrappers around protocol buffer requests to secondary indices - these make it easy to pull back a list of keys that match a specific index (`int`, `bin`, `int` range, or `bin` range).
* `GetIndex` has been deprecated and will be removed in v1.3, adjust your client calls accordingly to use `IndexGet`.
* `RiakGetOptions` and `RiakPutOptions` accept either an unsigned integer or one of a set of known strings for R, W, DW, etc. This brings these objects in line with other areas of the API.

### Fixes

In addition to the issues listed below, we also fixed a bug where some Riak MapReduce results were being discarded by CorrugatedIron; this made it appear like Riak was exhibiting non-deterministic behavior on all Map/Reduce queries. 

We were incorrectly handling Map/Reduce results when multiple tuples were returned within the same protocol buffer message from Riak. CorrugatedIron would only consume a single tuple from each message. This has been fixed within CI.

* [Issue 91](https://github.com/DistributedNonsense/CorrugatedIron/issues/91) - Remove evidence of LastModifiedDate munging.
* [Issue 90](https://github.com/DistributedNonsense/CorrugatedIron/issues/90) - Remove minor perf issue in `WalkLinks`.
* [Issue 89](https://github.com/DistributedNonsense/CorrugatedIron/issues/89) - Remove unused code `CondenseResult`.
* [Issue 80](https://github.com/DistributedNonsense/CorrugatedIron/issues/80) - `$key` is auto-mucked to `$key_bin` .
* [Issue 78](https://github.com/DistributedNonsense/CorrugatedIron/issues/78) - Enabling search via `SetBucketProperties()` now correctly enabled the search pre-commit hook.
* [Issue 72](https://github.com/DistributedNonsense/CorrugatedIron/issues/72) - XML documentation is now included in the Nuget package for better support from Intellisense.
* [Issue 71](https://github.com/DistributedNonsense/CorrugatedIron/issues/71) - Getting extended properties on a new bucket no longer throws exceptions.
* [Issue 67](https://github.com/DistributedNonsense/CorrugatedIron/issues/67) - Finally added support for the `Either<TLeft, TRight>` on all options.

### Thanks

We have had some great discussion from members of the community which is starting to drive out more interesting ideas and some extended goals for the future of **CorrugatedIron**. We're grateful for this involvement and hope to see more of it.

v1.1.0
------

### Fixes

 * [Issue 69](https://github.com/DistributedNonsense/CorrugatedIron/issues/69) - Multiple values can now be stored in Secondary Indexes .

v1.0.0
------

### Features

* **Full Riak v1.2 support.**
* **Up-to-date PBC interface.** If Riak v1.2 can do it, we support it. And we send the correct message format, too.
* **Better secondary index support.** We've updated secondary indexes to be first class citizens of CorrugatedIron and not just a manually configured MapReduce input phase.
* **Riak Search support.** Our original support for Riak Search was a bit of a mess - users had to write their search queries as strings. We spent a lot of time focusing on getting our Riak Search interface right. Users can create searches using a fluent API. Because we know string mucking is neither fast nor fun we also added the option to compile your searches in CorrugatedIron. Compiling your searches will save a lot of CPU and memory during execution, so start compiling today!
* **Fluent interface all the things!** Wherever we could make a fluent interface, we did. Even when it hurt. You're welcome.
* **Even more documentation.** We've been on a documentation blitz both on the site and in the code base. We aren't done, but we're getting closer to having 100% API documentation up on the site and present in the code so you can read the docs while your IntelliSense goes crazy.
* **Messy ClientID behavior.** There are a lot of crazy things that users can do with a client ID. Originally we punted on the decision and relied on developers to supply a client ID. The default behavior for Riak is to use vnode vector clocks, which removes the need to set a client ID. This removes a lot of pain from a developer's standpoint and we decided to support removing your pain. No more client IDs!
* **Test Coverage.** CorrugatedIron v1.0.0 - now with more unit tests.
* **TPL Support.** Better support for the Task Parallel Library has been added to the async client interfaces. 
* **Custom serializers, deserializers, and sibling resolution.** You can supply your own logic to muck with your data and resolve siblings. 
* **Clean up.** There are too many minor fixes to mention, but you can rest assured that we've been going through the code base cleaning house and trying to make our code presentable and free of comments like `HAX GO HERE`.
* **Upgraded dependencies.** Our protobuf.net and JSON dependencies have been moved to the current versions.
* **Code removal.** We marked a number of methods in `RiakAsyncClient` as obsolete. In addition, while tidying up the interfaces, we updated the `Get` method to use the `RiakGetOptions` class. Make sure you check your code for these APIs - we'll be removing them from the code base in a few releases.

### Fixes

* [Issue 57](https://github.com/DistributedNonsense/CorrugatedIron/issues/57) - `Set<T>()` operations no longer require a content type.
* [Issue 53](https://github.com/DistributedNonsense/CorrugatedIron/issues/54) - Linux & OSX builds now correctly resolve their deps.

### Thanks

While OJ Reeves and Jeremiah Peschka are responsible for the majority of the code, we couldn't have done it without the support of the community. In no particular order, thanks to the following people for their code, ideas, and conversation:

* [Tiago Margalho](https://github.com/tiagomargalho)
* [Kevin Pullin](https://github.com/kppullin)
* Matthew Whitfield
* [Ethan J. Brown](https://github.com/Iristyle)
* [Taliesin Sisson](https://github.com/taliesins)
* Felix Terkhorn
* Marc Gravell (protobuf-net)
* James Newton-King (Json.NET)
* And, of course, the fine people of Basho and #riak

v0.2.1
------

### Features

#### TCP_NODELAY

We've set `TCP_NODELAY` on the sockets communicating with Riak. This forces CorrugatedIron into behavior where it won't buffer many small messages (very common when using Protocol Buffers). Instead, CorrugatedIron is going to send data as quickly as it can to Riak. 

You can read more about [Basho's recommendations](http://wiki.basho.com/Client-Implementation-Guide.html#Nagle's-Algorithm) and a deeper explanation of [Nagle's algorithm](http://en.wikipedia.org/wiki/Nagle%27s_algorithm)

#### GetIndex
To make life easier for developers we've added a GetIndex method. Developers who want to use this can create a `RiakIndexInput` object and pass that to the `GetIndex` method. Under the covers, `GetIndex` creates a MapReduce job to query the secondary index and then returns a `RiakBucketKeyInput` object, perfect for feeding to additional MapReduce phases.

    var rbki = new RiakIntIndexRangeInput("people", "age", 18, 35);
    var targetMarketKeys = client.GetIndex(rbki);
    
    var mr = new RiakMapReduceQuery { ContentType = RiakConstants.ContentTypes.ApplicationJson };
    
    mr.Inputs(targetMarketyKeys);
    
We think that this will make it easier for developers to work with secondary indexes in Riak.

#### Configuration
After a bit of work and time on OJ's part (everybody thank OJ), we've pushed out fluent configuration code. The relevant code is in [`RiakClusterConfiguration`](https://github.com/DistributedNonsense/CorrugatedIron/blob/master/CorrugatedIron/Config/Fluent/RiakClusterConfiguration.cs), [`RiakNodeConfiguration`](https://github.com/DistributedNonsense/CorrugatedIron/blob/master/CorrugatedIron/Config/Fluent/RiakNodeConfiguration.cs), and [`RiakExternalLoadBalancerConfiguration`](https://github.com/DistributedNonsense/CorrugatedIron/blob/master/CorrugatedIron/Config/Fluent/RiakExternalLoadBalancerConfiguration.cs).

### Fixes

Lots that we can't remember! Sorry. We've only just started to make sure we're on top of our release notes as of v1.0.0.

