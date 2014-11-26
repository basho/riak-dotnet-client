CorrugatedIron - a .NET client for Riak
=======================================

**CorrugatedIron** is a .NET library which makes it easy to communicate with [Riak](http://riak.basho.com/), [Basho](http://www.basho.com/)'s fault-tolerante, distributed KV store.

The easiest way to get hold of it is to install the [Nuget package](http://www.nuget.org/Packages/CorrugatedIron/).

Building From Source
----------------------

### GUI
Open `CorrugatedIron.sln` and build. All necessary files will be downloaded and generated for you.

### Command Line
* Windows
 * Execute `make.cmd` either via double-click in the file explorer, or by opening a shell in your cloned repository and running `.\make.cmd`
* Mono
 * Execute `make` (GNU Make required)

Travis-CI Build Status
----------------------

* Master: [![Build Status](https://travis-ci.org/basho-labs/CorrugatedIron.svg?branch=master)](https://travis-ci.org/basho-labs/CorrugatedIron)
* Develop: [![Build Status](https://travis-ci.org/basho-labs/CorrugatedIron.svg?branch=develop)](https://travis-ci.org/basho-labs/CorrugatedIron)

Authors
-------

* [OJ Reeves](http://buffered.io)
* [Jeremiah Peschka](http://facility9.com/)
* [Alex Moore](http://basho.com/)
* [Luke Bakken](http://bakken.io/)

Documentation
-------------

Documentation for the project can be found on the [project website](http://corrugatediron.org/).

Release Notes
-------------

Please see the [Release Notes](RELNOTES.md) document.

License
-------

**CorrugatedIron** is Open Source software released under the Apache 2.0 License. Please see the [LICENSE](LICENSE) file for full license details.

Regenerating .proto files
=========================

* Ensure submodules are checked out
* `.\make.cmd ProtoGen`

CorrugatedIron Deployment Process
=================================

* Merge all required feature branches into develop.
* Verify that all tests succeed.
* Make sure that the TravisCI build succeeds.
* run `git flow release start vA.B.C`
* Edit CorrugatedIron.nuspec and VersionInfo.cs so that the version numbers are up to date.
* To produce a new `release` build open a command prompt, change to the CI folder and run: `make`
* Verify that two nupkg files were created, one for the library and one for the symbols. The version number should match that which you are releasing.
* Finish the release by running: `git flow release finish vA.B.C`
* Push all the branches/tags up: `git push origin master:master && git push origin develop:develop && git push --tags`
* Push to Nuget by running (from the same command prompt): `.nuget\Nuget.exe push CorrugatedIron.VERSION.nupkg` (it should also push up the symbols).
* Make any changes to samples that might be required.
* Make any changes to documentation that might be required.

Give yourself a pat on the back and have some tea.

CorrugatedIron Roadmap
======================

### Cluster and node info
[Github Issue 10](https://github.com/basho-labs/CorrugatedIron/issues/10)
This would be looking for ways to get the results of `riak_admin status` and other commands through the Riak Client API

### SOLR Interface
[Github Issue 20](https://github.com/basho-labs/CorrugatedIron/issues/20)
Create an HTTP interface for [Faceted queries vi the Solr Interface](http://wiki.basho.com/Riak-Search---Querying.html#Faceted-Queries-via-the-Solr-Interface)

### Improved Riak Search API
[Github Issue 23](url:https://github.com/basho-labs/CorrugatedIron/issues/23) Additional development should be undertaken to make it easier for developers to use this.

Thanks
======

The following people have contributed to CorrugatedIron and/or one of the related libraries or applications that make it work:

* Jeremiah Peschka
* OJ Reeves
* Matthew Whitfield
* Kevin Pullin
* Tiago Margalho
* Marc Gravell (protobuf-net)
* James Newton-King (Json.NET)
* Alex Moore
* Luke Bakken
