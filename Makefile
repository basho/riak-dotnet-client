SLNDIR = $(realpath $(CURDIR))
include $(SLNDIR)/build/mono.mk

test: test-all

