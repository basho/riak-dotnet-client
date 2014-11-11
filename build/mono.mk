PROJDIR = $(SLNDIR)/src

INSTALL_MONO = $(SLNDIR)/build/install-mono
MONO_EXE = mono --runtime='v4.0.30319'

NUGET_PKGDIR = $(SLNDIR)/packages
NUGET_EXE = $(SLNDIR)/.nuget/NuGet.exe
NUGET_CFG = $(SLNDIR)/.nuget/NuGet.Config
NUGET_RESTORE = $(MONO_EXE) $(NUGET_EXE) restore -PackagesDirectory $(NUGET_PKGDIR) -ConfigFile $(NUGET_CFG)

VERBOSITY = normal
# NB: SolutionDir *must* end in slash here
XBUILD = xbuild /verbosity:$(VERBOSITY) /nologo /property:SolutionDir=$(SLNDIR)/ /property:Mono=True

.PHONY: all release debug
all: release debug

install-certs:
	mozroots --import --sync

# NB: run this target if package-restore fails on download
install-mono:
	$(INSTALL_MONO)

package-restore:
	$(NUGET_RESTORE) $(SLNDIR)/.nuget/packages.config

proto:
	cd CorrugatedIron/Messages
	$(MONO_EXE) $(PROTOGEN) -i:$< -o:$@
	rm $<

release: package-restore
	$(XBUILD) /target:Release $(SLNDIR)/build/build.proj

debug: package-restore
	$(XBUILD) /target:Debug $(SLNDIR)/build/build.proj

# NB: build.proj has debug as a dependency
unit-test: package-restore
	$(XBUILD) /target:UnitTest $(SLNDIR)/build/build.proj

# NB: build.proj has debug as a dependency
integration-test: package-restore
	$(XBUILD) /target:IntegrationTest $(SLNDIR)/build/build.proj

test-all: unit-test integration-test

.PHONY: clean-release clean-debug clean
clean-release: package-restore
	$(XBUILD) /target:Clean /property:Configuration=Release
clean-debug: package-restore
	$(XBUILD) /target:Clean /property:Configuration=Debug
clean: clean-release clean-debug

