CorrugatedIron - a .NET client for Riak
=======================================

Authors
-------

* [OJ Reeves](http://buffered.io)
* [Jeremiah Peschka](http://facility9.com/)

Current Features
----------------

<small>*&hearts;: denotes availability of both blocking and asynchronous APIs*<br/>
*&laquo;: denotes availability of both streaming and non-streaming APIs*</small>

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
* Graceful degrade to HTTP/REST API when the request isn't supported via Protocol Buffers.
* Riak cluster support:
    * One or more nodes in the cluster.
    * Load-balancing of connections across the nodes.
        * Currently only round-robin is supported, more strategies to come later.
    * Per-node configuration for:
        * Host Name (purely used for identification).
        * Host Address.
        * PBC Port.
        * HTTP/REST Port.
        * Pool Size.
        * Timeout parameters.
* Works with .NET 4.0 on Windows, and Mono on Linux and OSX.

v0.1 Features (in development)
----------------------

* Connection self-healing and node management.
* .NET 3.5 support.

Future Features
---------------

* LINQ expression parsing for map/reduce

License
-------

Please see the LICENSE file for license information.

"CorrugatedIron"? WTF is that
-----------------------------

Before [Riak](http://riak.basho.com/) was called Riak, it was called _Ripple_. [Basho](http://basho.com/) changed the name to Riak prior to it's first release (I think). _Riak_ is the Indonesian word for _Ripple_.

When we first kicked this project off we knew we were attempting to bring together Riak and .NET, so we wanted a name which was in some ways indicative of both of the technologies. _Ripple_ was an obvious component, and given that .NET was involved we thought we'd stick to the MS approach of using the word _iron_ for things related to .NET. [CorrugatedIron](http://en.wikipedia.org/wiki/Corrugated_galvanised_iron) is just a real-life representation of what happens when _iron_ and _ripples_ meet!

So that's where the name comes from.
