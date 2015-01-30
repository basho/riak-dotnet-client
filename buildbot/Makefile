SLNDIR = $(realpath $(CURDIR)/..)
include $(SLNDIR)/build/mono.mk

.PHONY: setup-integration

TOOLS_DIR = $(SLNDIR)/tools
RIAK_CONF = $(RIAK_DIR)/etc/riak.conf
ADV_CONF = $(RIAK_DIR)/etc/advanced.config
RIAK_ADMIN = $(RIAK_DIR)/bin/riak-admin

preconfigure:
	$(TOOLS_DIR)/gen-riak-conf $(RIAK_CONF) 8098 8087
	$(TOOLS_DIR)/gen-adv-conf $(ADV_CONF)

configure:
	$(TOOLS_DIR)/riak-cluster-config $(RIAK_ADMIN) 8098

compile: install-deps debug

lint:
	echo Nope > /dev/null

setup-integration:
	cp -f $(CURDIR)/App.config $(PROJDIR)/RiakClientTests.Live/bin/Debug/RiakClientTests.Live.dll.config

test: setup-integration test-all

