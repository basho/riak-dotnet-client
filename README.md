Riak .NET Client - a .NET client for Riak
=========================================

**Riak .NET Client** is a .NET library which makes it easy to communicate with [Riak](http://basho.com/riak/), an open source, distributed database that focuses on high availability, horizontal scalability, and *predictable*
latency. Both Riak and this code is maintained by [Basho](http://www.basho.com/).

1. [Installation](#installation)
	* [Cloning](#cloning)
	* [Build from Source](#build-from-source)
2. [Documentation](#documentation)
3. [Contributing](#contributing)
	* [An honest disclaimer](#an-honest-disclaimer)
4. [Roadmap](#riak-net-client-roadmap)
	* [Cluster and node info](#cluster-and-node-info)
	* [SOLR Interface](#solr-interface)
	* [Improved Riak Search API](#improved-riak-search-api)
5. [License and Authors](#license-and-authors)


## Installation
The easiest way to get hold of it is to install the [Nuget package](http://www.nuget.org/Packages/RiakClient/).


### Cloning
*Note:* Please use the `--recursive` git option or run `git submodule update --init` after cloning as a couple submodules are used. Thanks!


### Build from Source

#### GUI

*Note*: `git` must be in your `PATH` when doing a `Release`-configuration build.

Open `RiakClient.sln` and build. This will generate the required `src/CommonAssemblyInfo.cs` file.

#### Command Line

*Note*: `git` must be in your `PATH` when doing a `Release`-configuration build.

* Windows
 * Execute `make.cmd` either via double-click in the file explorer, or
   by opening a shell in your cloned repository and running `.\make.cmd`.
   This will use `powershell.exe` to run `make.ps1` and create a Debug
   build. Running `make.ps1` has much more flexibility. Use the `Get-Help
   .\make.ps1 -Full` command in Powershell for more information.
* Mono
 * Execute `make` (GNU Make required)

## Documentation

* Master: [![Build Status](https://travis-ci.org/basho/riak-dotnet-client.svg?branch=master)](https://travis-ci.org/basho/riak-dotnet-client)
* Develop: [![Build Status](https://travis-ci.org/basho/riak-dotnet-client.svg?branch=develop)](https://travis-ci.org/basho/riak-dotnet-client)


Documentation for the project can be found on the [project wiki](https://github.com/basho/riak-dotnet-client/wiki). Please also see the [Release Notes](RELNOTES.md) for the latest updates.

## Contributing

This repo's maintainers are engineers at Basho and we welcome your contribution to the project! Review the details in [CONTRIBUTING.md](CONTRIBUTING.md) in order to give back to this project.

### An honest disclaimer

Due to our obsession with stability and our rich ecosystem of users, community updates on this repo take longer to review. 

The most helpful way to contribute is by reporting your experience through issues. Issues may not be updated while we review internally, but they're still incredibly appreciated.

Pull requests take multiple engineers for verification and testing. If you're passionate enough to want to learn more on how you can get hands on in this process, reach out to [Matt](mailto:mbrender@basho.com), your developer advocate. 

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

