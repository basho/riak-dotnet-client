Riak .NET Client - a .NET client for Riak
=========================================

**Riak .NET Client** is a .NET library which makes it easy to communicate with [Riak](http://riak.basho.com/), [Basho](http://www.basho.com/)'s fault-tolerante, distributed KV store.

The easiest way to get hold of it is to install the [Nuget package](http://www.nuget.org/Packages/RiakClient/).

Building From Source
----------------------

### GUI

*Note*: `git` must be in your `PATH` when doing a `Release`-configuration build.

Open `RiakClient.sln` and build. This will generate the required `src/CommonAssemblyInfo.cs` file.

### Command Line

*Note*: `git` must be in your `PATH` when doing a `Release`-configuration build.

* Windows
 * Execute `make.cmd` either via double-click in the file explorer, or
   by opening a shell in your cloned repository and running `.\make.cmd`.
   This will use `powershell.exe` to run `make.ps1` and create a Debug
   build. Running `make.ps1` has much more flexibility. Use the `Get-Help
   .\make.ps1 -Full` command in Powershell for more information.
* Mono
 * Execute `make` (GNU Make required)

Travis-CI Build Status
----------------------

* Master: [![Build Status](https://travis-ci.org/basho-labs/riak-dotnet-client.svg?branch=master)](https://travis-ci.org/basho-labs/riak-dotnet-client)
* Develop: [![Build Status](https://travis-ci.org/basho-labs/riak-dotnet-client.svg?branch=develop)](https://travis-ci.org/basho-labs/riak-dotnet-client)

Authors
-------

* [OJ Reeves](http://buffered.io)
* [Jeremiah Peschka](http://facility9.com/)
* [Alex Moore](http://alexmoore.io/)
* [Luke Bakken](http://bakken.io/)

Documentation
-------------

Documentation for the project can be found on the [project wiki](https://github.com/basho-labs/riak-dotnet-client/wiki)

Release Notes
-------------

Please see the [Release Notes](RELNOTES.md) document.

License
-------

**Riak .NET Client** is Open Source software released under the Apache 2.0 License. Please see the [LICENSE](LICENSE) file for full license details.

Riak .NET Client Roadmap
======================

### Cluster and node info
[Github Issue 10](https://github.com/basho-labs/riak-dotnet-client/issues/10)
This would be looking for ways to get the results of `riak_admin status` and other commands through the Riak Client API

### SOLR Interface
[Github Issue 20](https://github.com/basho-labs/riak-dotnet-client/issues/20)
Create an HTTP interface for [Faceted queries vi the Solr Interface](http://wiki.basho.com/Riak-Search---Querying.html#Faceted-Queries-via-the-Solr-Interface)

### Improved Riak Search API
[Github Issue 23](url:https://github.com/basho-labs/riak-dotnet-client/issues/23) Additional development should be undertaken to make it easier for developers to use this.

Thanks
======

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

