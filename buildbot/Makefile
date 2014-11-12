SLNDIR = $(realpath $(CURDIR)/..)
include $(SLNDIR)/build/mono.mk

.PHONY: setup-integration

RIAK_CONF = $(RIAK_DIR)/etc/riak.conf
RIAK_ADMIN = $(RIAK_DIR)/bin/riak-admin

preconfigure:
	echo 'ring_size = 16' >> $(RIAK_CONF)
	echo 'storage_backend = multi' >> $(RIAK_CONF)
	echo 'multi_backend.default = bitcask_backend' >> $(RIAK_CONF)
	echo 'multi_backend.bitcask_backend.storage_backend = bitcask' >> $(RIAK_CONF)
	echo 'multi_backend.bitcask_backend.bitcask.data_root = $$(platform_data_dir)/bitcask' >> $(RIAK_CONF)
	echo 'multi_backend.bitcask_backend.bitcask.io_mode = erlang' >> $(RIAK_CONF)
	echo 'multi_backend.leveldb_backend.storage_backend = leveldb' >> $(RIAK_CONF)
	echo 'multi_backend.leveldb_backend.leveldb.data_root = $$(platform_data_dir)/leveldb' >> $(RIAK_CONF)
	echo 'multi_backend.leveldb_backend.leveldb.maximum_memory.percent = 30' >> $(RIAK_CONF)
	echo 'multi_backend.mem_backend.storage_backend = memory' >> $(RIAK_CONF)
	echo 'multi_backend.mem_backend.memory_backend.ttl = 10s' >> $(RIAK_CONF)
	echo 'multi_backend.mem_backend.memory_backend.max_memory_per_vnode = 4MB' >> $(RIAK_CONF)
	echo 'listener.http.internal = 127.0.0.1:8098' >> $(RIAK_CONF)
	echo 'listener.protobuf.internal = 127.0.0.1:8087' >> $(RIAK_CONF)
	curl -XPUT -H 'Content-type: application/json' http://localhost:10018/buckets/riak_index_tests/props -d '{"props":{"backend":"leveldb_backend"}}'

configure:
	$(RIAK_ADMIN) bucket-type create yokozuna '{"props":{}}'
	$(RIAK_ADMIN) bucket-type activate yokozuna
	$(RIAK_ADMIN) bucket-type create leveldb_type '{"props":{"backend":"leveldb_backend"}}'
	$(RIAK_ADMIN) bucket-type create memory_type '{"props":{"backend":"mem_backend"}}'
	$(RIAK_ADMIN) bucket-type activate leveldb_type
	$(RIAK_ADMIN) bucket-type activate memory_type

compile: install-deps debug

lint:
	echo Nope > /dev/null

setup-integration:
	cp -f $(CURDIR)/App.config $(PROJDIR)/CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll.config

test: setup-integration test-all

