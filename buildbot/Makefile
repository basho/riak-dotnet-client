RIAK_CONF = ${RIAK_DIR}/etc/riak.conf
ADVANCED_CONF = ${RIAK_DIR}/etc/advanced.config
# RIAK = ${RIAK_DIR}/bin/riak
RIAK_ADMIN = ${RIAK_DIR}/bin/riak-admin
CERTS_DIR = $(shell pwd)/../spec/support/certs

preconfigure:
	echo "storage_backend = leveldb" >> ${RIAK_CONF}
	echo "search = on" >> ${RIAK_CONF}
	echo "listener.protobuf.internal = 127.0.0.1:8087" >> ${RIAK_CONF}


configure:
	#@${RIAK_ADMIN} bucket-type create counters '{"props":{"datatype":"counter", "allow_mult":true}}'
	#@${RIAK_ADMIN} bucket-type create maps '{"props":{"datatype":"map", "allow_mult":true}}'
	#@${RIAK_ADMIN} bucket-type create sets '{"props":{"datatype":"set", "allow_mult":true}}'
	@${RIAK_ADMIN} bucket-type create yokozuna '{"props":{}}'
	#@${RIAK_ADMIN} bucket-type activate counters
	#@${RIAK_ADMIN} bucket-type activate maps
	#@${RIAK_ADMIN} bucket-type activate sets
	@${RIAK_ADMIN} bucket-type activate yokozuna

compile:
	./.travis/PackageRestore.sh
  	./.travis/RunBuild.sh

lint:
	echo "Nope" >> /dev/null

test: 
	@mv ../CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll.config ../CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll.config.original
	@cp App.config ../CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll.config
	../.travis/RunTests.sh
	../.travis/RunIntegrationTests.sh

