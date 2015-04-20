Riak .NET Client
================

**Riak .NET Client** is a .NET library which facilitates communication with
[Riak](http://basho.com/riak/), an open source, distributed database that
focuses on high availability, horizontal scalability, and *predictable* latency.
Both Riak and this code is maintained by [Basho](http://www.basho.com/).

1. [Installation](#installation)
2. [Documentation](#documentation)
3. [Contributing](#contributing)
	* [An honest disclaimer](#an-honest-disclaimer)
4. [Roadmap](#riak-net-client-roadmap)
	* [Cluster and node info](#cluster-and-node-info)
	* [SOLR Interface](#solr-interface)
	* [Improved Riak Search API](#improved-riak-search-api)
5. [License and Authors](#license-and-authors)

## Build Status

* Master: [![Build Status](https://travis-ci.org/basho/riak-dotnet-client.svg?branch=master)](https://travis-ci.org/basho/riak-dotnet-client)
* Develop: [![Build Status](https://travis-ci.org/basho/riak-dotnet-client.svg?branch=develop)](https://travis-ci.org/basho/riak-dotnet-client)

## Installation
The easiest way to get hold of it is to install the [Nuget package](http://www.nuget.org/Packages/RiakClient/).

For more details on installation, including building from source, see the [installation section of the wiki](https://github.com/basho/riak-dotnet-client/wiki/Installation).

## Documentation

Documentation for the project can be found on the [project wiki](https://github.com/basho/riak-dotnet-client/wiki). Please also see the [Release Notes](RELNOTES.md) for the latest updates.

## Contributing

This repo's maintainers are engineers at Basho and we welcome your contribution to the project! Review the details in [CONTRIBUTING.md](CONTRIBUTING.md) in order to give back to this project.

### An honest disclaimer

Due to our obsession with stability and our rich ecosystem of users, community updates on this repo may take a little longer to review. 

The most helpful way to contribute is by reporting your experience through issues. Issues may not be updated while we review internally, but they're still incredibly appreciated.

Thank you for being part of the community! We love you for it. 


## Riak .NET Client Roadmap

### Cluster and node info
[Github Issue 10](https://github.com/basho/riak-dotnet-client/issues/10)
This would be looking for ways to get the results of `riak_admin status` and other commands through the Riak Client API

### SOLR Interface
[Github Issue 20](https://github.com/basho/riak-dotnet-client/issues/20)
Create an HTTP interface for [Faceted queries vi the Solr Interface](http://wiki.basho.com/Riak-Search---Querying.html#Faceted-Queries-via-the-Solr-Interface)

### Improved Riak Search API
[Github Issue 23](url:https://github.com/basho/riak-dotnet-client/issues/23) Additional development should be undertaken to make it easier for developers to use this.

## License and Authors
**Riak .NET Client** is Open Source software released under the Apache 2.0 License. Please see the [LICENSE](LICENSE) file for full license details.

* Author: [OJ Reeves](http://buffered.io)
* Author: [Jeremiah Peschka](http://facility9.com/)
* Author: [Alex Moore](http://alexmoore.io/)
* Author: [Luke Bakken](http://bakken.io/)

### Special Thanks

The following people have contributed to Riak .NET Client, it's predecessor CorrugatedIron, or one of the related libraries or applications that make it work:

* Jeremiah Peschka
* OJ Reeves
* Matthew Whitfield
* Kevin Pullin
* Tiago Margalho
* Marc Gravell (protobuf-net)
* James Newton-King (Json.NET)
* Alex Moore
* Luke Bakken
* Myles McDonnell

