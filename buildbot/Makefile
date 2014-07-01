RIAK_CONF = ${RIAK_DIR}/etc/riak.conf
RIAK_ADMIN = ${RIAK_DIR}/bin/riak-admin

preconfigure:
	echo "storage_backend = leveldb" >> ${RIAK_CONF}
	echo "search = on" >> ${RIAK_CONF}
	echo "listener.protobuf.internal = 127.0.0.1:8087" >> ${RIAK_CONF}


configure:
	@${RIAK_ADMIN} bucket-type create yokozuna '{"props":{}}'
	@${RIAK_ADMIN} bucket-type activate yokozuna

compile:
	../.travis/PackageRestore.sh
	../.travis/RunBuild.sh

lint:
	echo "Nope" >> /dev/null

test: unit-tests integration-tests

unit-tests:
	../.travis/RunTests.sh

integration-tests:
	@mv ../CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll.config ../CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll.config.original
	@cp App.config ../CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll.config
	../.travis/RunIntegrationTests.sh

