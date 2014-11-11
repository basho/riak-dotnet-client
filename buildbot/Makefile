SLNDIR = $(realpath $(CURDIR)/..)
include $(SLNDIR)/build/mono.mk

.PHONY: setup-integration

RIAK_CONF = $(RIAK_DIR)/etc/riak.conf
RIAK_ADMIN = $(RIAK_DIR)/bin/riak-admin

preconfigure:
	echo "storage_backend = leveldb" >> $(RIAK_CONF)
	echo "search = on" >> $(RIAK_CONF)
	echo "listener.protobuf.internal = 127.0.0.1:8087" >> $(RIAK_CONF)

configure:
	$(RIAK_ADMIN) bucket-type create yokozuna '{"props":{}}'
	$(RIAK_ADMIN) bucket-type activate yokozuna

compile: install-mono debug

lint:
	echo Nope > /dev/null

setup-integration:
	cp -f $(CURDIR)/App.config $(PROJDIR)/CorrugatedIron.Tests.Live/bin/Debug/CorrugatedIron.Tests.Live.dll.config

test: setup-integration test-all

