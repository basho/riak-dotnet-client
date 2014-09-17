RIAK_CONF = ${RIAK_DIR}/etc/riak.conf
ADVANCED_CONF = ${RIAK_DIR}/etc/advanced.config
RIAK_ADMIN = ${RIAK_DIR}/bin/riak-admin

preconfigure:
	echo "storage_backend = leveldb" >> ${RIAK_CONF}
	echo "search = on" >> ${RIAK_CONF}
	echo "listener.protobuf.internal = 127.0.0.1:8087" >> ${RIAK_CONF}
	cat ${ADVANCED_CONF}
	cp ${ADVANCED_CONF} "${ADVANCED_CONF}.old"
	sed -i '' -e '$$ d' ${ADVANCED_CONF}
	sed -i '' -e '$$ d' ${ADVANCED_CONF}
	cat advanced_config_append >> ${ADVANCED_CONF}
	cat ${ADVANCED_CONF}


configure:
	@${RIAK_ADMIN} bucket-type create plain '{"props":{}}'
	@${RIAK_ADMIN} bucket-type create search '{"props":{}}'
	@${RIAK_ADMIN} bucket-type create maps '{"props":{"datatype":"map"}}'
	@${RIAK_ADMIN} bucket-type create sets '{"props":{"datatype":"set"}}'
	@${RIAK_ADMIN} bucket-type create counters '{"props":{"datatype":"counter"}}'
	@${RIAK_ADMIN} bucket-type activate plain
	@${RIAK_ADMIN} bucket-type activate search
	@${RIAK_ADMIN} bucket-type activate maps
	@${RIAK_ADMIN} bucket-type activate sets
	@${RIAK_ADMIN} bucket-type activate counters

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

