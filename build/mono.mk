PROJDIR = $(SLNDIR)/src

INSTALL_MONO = $(SLNDIR)/build/install-mono

NUGET_PKGDIR = $(SLNDIR)/packages
NUGET_EXE = $(SLNDIR)/.nuget/NuGet.exe
NUGET_CFG = $(SLNDIR)/.nuget/NuGet.Config
NUGET_RESTORE = $(MONO) $(NUGET_EXE) restore -PackagesDirectory $(NUGET_PKGDIR) -ConfigFile $(NUGET_CFG)

XBUILD = xbuild $(SLNDIR)/CorrugatedIron.sln /property:Mono=True
MONO = mono --runtime='v4.0.30319'
NUNIT = $(SLNDIR)/packages/NUnit.Runners.2.6.3/tools/nunit-console.exe -config Debug-Mono -nologo -nodots -labels
PROTOGEN = ~/bin/ProtoGen/protogen.exe
PROTOC = protoc

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
	$(MONO) $(PROTOGEN) -i:$< -o:$@
	rm $<

release: package-restore
	$(XBUILD) /property:Configuration=Release

debug: package-restore
	$(XBUILD) /property:Configuration=Debug

test-all: unit-test integration-test

unit-test: debug
	$(MONO) $(NUNIT) -work=$(PROJDIR)/CorrugatedIron.Tests $(PROJDIR)/CorrugatedIron.Tests/CorrugatedIron.Tests.nunit

integration-test: debug unit-test
	$(MONO) $(NUNIT) -work=$(PROJDIR)/CorrugatedIron.Tests.Live $(PROJDIR)/CorrugatedIron.Tests.Live/CorrugatedIron.Tests.Live.nunit

.PHONY: clean-release clean-debug clean
clean-release:
	$(XBUILD) /target:Clean /property:Configuration=Release
clean-debug:
	$(XBUILD) /target:Clean /property:Configuration=Debug
clean: clean-release clean-debug

