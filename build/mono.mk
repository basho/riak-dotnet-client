PROJDIR = $(SLNDIR)/src

MONO_EXE = mono --runtime='v4.0.30319'

NUGET_PKGDIR = $(SLNDIR)/packages
NUGET_EXE = $(SLNDIR)/.nuget/NuGet.exe
NUGET_CFG = $(SLNDIR)/.nuget/NuGet.Config
NUGET_RESTORE = $(MONO_EXE) $(NUGET_EXE) restore -PackagesDirectory $(NUGET_PKGDIR) -ConfigFile $(NUGET_CFG)

VERBOSITY = normal
# NB: SolutionDir *must* end in slash here
XBUILD = xbuild /verbosity:$(VERBOSITY) /nologo /property:SolutionDir=$(SLNDIR)/

.PHONY: all release debug
all: release debug

install-certs:
	mozroots --import --sync

# NB: run this target if package-restore fails on download
install-deps:
	$(SLNDIR)/build/install-deps

package-restore:
	$(NUGET_RESTORE) $(SLNDIR)/.nuget/packages.config

release: package-restore
	$(XBUILD) /target:Release $(SLNDIR)/build/build.targets

debug: package-restore
	$(XBUILD) /target:Debug $(SLNDIR)/build/build.targets

protogen:
	@echo 'protogen is Windows-only'

# NB: build.targets has debug as a dependency
unit-test: package-restore
	$(XBUILD) /target:UnitTest $(SLNDIR)/build/build.targets

# NB: build.targets has debug as a dependency
integration-test: package-restore
	$(XBUILD) /target:IntegrationTest $(SLNDIR)/build/build.targets

test-all: package-restore
	$(XBUILD) /target:TestAll $(SLNDIR)/build/build.targets

deprecated-test: package-restore
	$(XBUILD) /target:DeprecatedTest $(SLNDIR)/build/build.targets

.PHONY: clean-release clean-debug clean
clean-release: package-restore
	$(XBUILD) /target:Clean /property:Configuration=Release
clean-debug: package-restore
	$(XBUILD) /target:Clean /property:Configuration=Debug
clean: clean-release clean-debug

.PHONY: help

help:
	@echo ''
	@echo ' Targets:'
	@echo ' ----------------------------------------------------'
	@echo ' debug            - Debug build                      '
	@echo ' release          - Release build with versioning    '
	@echo ' all              - Debug, then Release build        '
	@echo ' clean            - Clean everything                 '
	@echo ' test             - Run all tests (except deprecated)'
	@echo ' unit-test        - Run unit tests                   '
	@echo ' integration-test - Run integration tests            '
	@echo ' deprecated-test  - Run deprecated tests             '
	@echo ' ----------------------------------------------------'
	@echo ''
