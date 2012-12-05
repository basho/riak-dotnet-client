CorrugatedIron - a .NET client for Riak
=======================================

Authors
-------

* [OJ Reeves](http://buffered.io)
* [Jeremiah Peschka](http://facility9.com/)

"CorrugatedIron"? WTF is that?!
-------------------------------

Before [Riak](http://riak.basho.com/) was called Riak, it was called _Ripple_. [Basho](http://basho.com/) changed the name to Riak prior to it's first release (I think). _Riak_ is the Indonesian word for _Ripple_.

When we first kicked this project off we knew we were attempting to bring together Riak and .NET, so we wanted a name which was in some ways indicative of both of the technologies. _Ripple_ was an obvious component, and given that .NET was involved we thought we'd stick to the MS approach of using the word _iron_ for things related to .NET. [CorrugatedIron](http://en.wikipedia.org/wiki/Corrugated_galvanised_iron) is just a real-life representation of what happens when _iron_ and _ripples_ meet!

So that's where the name comes from.

Current Features
----------------

<small>*&hearts;: denotes availability of both blocking and asynchronous APIs*<br/>
*&laquo;: denotes availability of both streaming and non-streaming APIs*</small>

* Riak cluster support:
    * One or more nodes in the cluster.
    * Load-balancing and **pooling** of connections across the nodes.
        * Currently only round-robin is supported, more strategies to come later.
    * Per-node configuration for:
        * Host Name (purely used for identification).
        * Host Address.
        * PBC Port.
        * HTTP/REST Port.
        * Pool Size.
        * Timeout parameters.
* Server ping. &nbsp;&nbsp;&nbsp;&hearts;
* Get server information/version. &nbsp;&nbsp;&nbsp;&hearts;
* Simple Get/Put/Delete operations. &nbsp;&nbsp;&nbsp;&hearts;
* Bulk Get/Put/Delete operations. &nbsp;&nbsp;&nbsp;&hearts;
* List buckets. &nbsp;&nbsp;&nbsp;&hearts;
* List keys. &nbsp;&nbsp;&nbsp;&hearts;&nbsp;&nbsp;&laquo;
* Semi-fluent Map/Reduce. &nbsp;&nbsp;&nbsp;&hearts;&nbsp;&nbsp;&laquo;
* Link walking. &nbsp;&nbsp;&nbsp;&hearts;
* Delete buckets. &nbsp;&nbsp;&nbsp;&hearts;
* Set/Get bucket properties. &nbsp;&nbsp;&nbsp;&hearts;
* Batch operations on a single connection.
    * Each time a Client function is called resulting in communication with the Riak
      cluster, a connection is pulled from a pool on a given node. In most use-cases this
      functionality is fine as it is often single-shot calls that are made. There are,
      however, cases where many operations will happen at once. Rather than forcing the
      user to make multiple calls to the client, resulting in multiple connection
      acquisitions behind the scenes, the user can use the Batch interface to make many
      calls on a single connection. This also reduces the overhead of setting the client
      ID on each call.
    * Because a batch operation reuses a single connection only a subset of the client
      API is available for batch actions. The functions that are excluded are the
      asynchronous functions.
* Graceful degrades to HTTP/REST API when the request isn't supported via Protocol Buffers.
* Configurable via `web.config`, `app.config` or a custom configuration file.

**CorrugatedIron works with .NET 4.0 on Windows and Mono on Linux and OSX.**

Planned Features (in development)
---------------------------------

* Connection idling when connections haven't been used for a while.
* Improvements to connection self-healing and node management.

Unplanned Features
------------------

* **.NET 3.5 support** - after careful consideration we decided to not worry about
  adding support for .NET 3.5 applications. The main reason for this is that Riak is new
  to the .NET world and any applications that aim to talk to Raik will most likely be
  new applications themselves and hence would use the latest version of .NET (v4.0 at the
  time of writing). If the community feels that .NET 3.5 support is required then we can
  look into the options around adding support. At this stage, we don't think it's worth
  the effort when we could instead be adding new features to the client.

Future Features
---------------

* Suggestions from users.
* Support for Riak Search.
* LINQ expression parsing for map/reduce.

License
-------

CorrugatedIron is Open Source software released under the Apache 2.0 License.
Please see the `LICENSE` file for full license details.

