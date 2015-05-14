SLNDIR = $(realpath $(CURDIR)/..)
include $(SLNDIR)/build/mono.mk

.PHONY: setup-integration

TOOLS_DIR = $(SLNDIR)/tools/devrel
RIAK_CONF = $(RIAK_DIR)/etc/riak.conf
ADV_CONF = $(RIAK_DIR)/etc/advanced.config
RIAK_ADMIN = $(RIAK_DIR)/bin/riak-admin

preconfigure:
	$(TOOLS_DIR)/gen-riak-conf $(RIAK_CONF) 8098 8087
	$(TOOLS_DIR)/strong-consistency-conf $(RIAK_CONF)
	$(TOOLS_DIR)/gen-adv-conf $(ADV_CONF)

configure:
	$(TOOLS_DIR)/riak-cluster-config $(RIAK_ADMIN) 8098 true false

compile: install-deps debug

lint:
	echo Nope > /dev/null

setup-integration:
	sed -e 's/riak-test/127.0.0.1/g' -e 's/\<10017\>/8087/g' $(PROJDIR)/RiakClientTests.Live/App.config > $(PROJDIR)/RiakClientTests.Live/bin/Debug/RiakClientTests.Live.dll.config
	sed -e 's/riak-test/127.0.0.1/g' -e 's/\<10017\>/8087/g' $(PROJDIR)/Test/App.config > $(PROJDIR)/Test/bin/Debug/Test.dll.config

# test: setup-integration test-all
test: unit-test

