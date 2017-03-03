PROJDIR = $(SLNDIR)/src

MONO_EXE = mono

VERBOSITY = normal

# NB: SolutionDir *must* end in slash here
XBUILD = xbuild /verbosity:$(VERBOSITY) /nologo /property:SolutionDir=$(SLNDIR)/

.PHONY: all release debug
all: release debug

install-certs:
	mozroots --import --sync

install-deps:
	$(SLNDIR)/build/install-deps

release:
	$(XBUILD) /target:Release $(SLNDIR)/build/build.targets

debug:
	$(XBUILD) /target:Debug $(SLNDIR)/build/build.targets

protogen:
	@echo 'protogen is Windows-only'

# NB: build.targets has debug as a dependency for tests
test-all:
	$(XBUILD) /target:TestAll $(SLNDIR)/build/build.targets

unit-test:
	$(XBUILD) /target:UnitTest $(SLNDIR)/build/build.targets

integration-test:
	$(XBUILD) /target:IntegrationTest $(SLNDIR)/build/build.targets

timeseries-test:
	$(XBUILD) /target:TimeseriesTest $(SLNDIR)/build/build.targets

deprecated-test:
	$(XBUILD) /target:DeprecatedTest $(SLNDIR)/build/build.targets

.PHONY: clean-release clean-debug clean
clean-release:
	$(XBUILD) /target:Clean /property:Configuration=Release
clean-debug:
	$(XBUILD) /target:Clean /property:Configuration=Debug
clean: clean-release clean-debug

.PHONY: help

help:
	@echo ''
	@echo ' Targets:'
	@echo ' --------------------------------------------------------'
	@echo ' debug                - Debug build                      '
	@echo ' release              - Release build with versioning    '
	@echo ' all                  - Debug, then Release build        '
	@echo ' clean                - Clean everything                 '
	@echo ' test                 - Run all tests (except deprecated)'
	@echo ' unit-test            - Run unit tests                   '
	@echo ' integration-test     - Run integration tests            '
	@echo ' integration-hll-test - Run integration Hll tests        '
	@echo ' timeseries-test      - Run timeseries tests             '
	@echo ' deprecated-test      - Run deprecated tests             '
	@echo ' --------------------------------------------------------'
	@echo ''
