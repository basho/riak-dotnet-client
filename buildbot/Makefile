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
	../.mono-build-scripts/InstallMono.sh
	../.mono-build-scripts/PackageRestore.sh
	../.mono-build-scripts/RunBuild.sh

lint:
	echo "Nope" >> /dev/null

test: unit-tests integration-tests

unit-tests:
	../.mono-build-scripts/RunTests.sh

integration-tests:
	@mv ../CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll.config ../CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll.config.original
	@cp App.config ../CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll.config
	../.mono-build-scripts/RunIntegrationTests.sh

