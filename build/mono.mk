INSTALL_MONO = $(SLNDIR)/build/install-mono

NUGET_PKGDIR = $(SLNDIR)/packages
NUGET_EXE = $(SLNDIR)/.nuget/NuGet.exe
NUGET_CFG = $(SLNDIR)/.nuget/NuGet.Config
NUGET_RESTORE = $(MONO) $(NUGET_EXE) restore -PackagesDirectory $(NUGET_PKGDIR) -ConfigFile $(NUGET_CFG)

XBUILD = xbuild
MONO = mono --runtime='v4.0.30319'
NUNIT = $(SLNDIR)/packages/NUnit.Runners.2.6.3/tools/nunit-console.exe -config Debug-Mono -nologo -nodots -labels
PROTOGEN = ~/bin/ProtoGen/protogen.exe
PROTOC = protoc

all: release debug

install-certs:
	mozroots --import --sync

# NB: run this target if package-restore fails on download
install-mono:
	$(INSTALL_MONO)

package-restore:
	$(NUGET_RESTORE) $(SLNDIR)/.nuget/packages.config
	$(NUGET_RESTORE) $(SLNDIR)/CorrugatedIron/packages.config
	$(NUGET_RESTORE) $(SLNDIR)/CorrugatedIron.Tests/packages.config
	$(NUGET_RESTORE) $(SLNDIR)/CorrugatedIron.Tests.Live/packages.config

proto:
	cd CorrugatedIron/Messages
	$(MONO) $(PROTOGEN) -i:$< -o:$@
	rm $<

release: package-restore
	$(XBUILD) $(SLNDIR)/CorrugatedIron.sln /property:Configuration=Release /property:Mono=True

debug: package-restore
	$(XBUILD) $(SLNDIR)/CorrugatedIron.sln /property:Configuration=Debug /property:Mono=True

test-all: unit-test integration-test

unit-test: debug
	$(MONO) $(NUNIT) -work=$(SLNDIR)/CorrugatedIron.Tests $(SLNDIR)/CorrugatedIron.Tests/CorrugatedIron.Tests.nunit

integration-test: debug unit-test
	$(MONO) $(NUNIT) -work=$(SLNDIR)/CorrugatedIron.Tests.Live $(SLNDIR)/CorrugatedIron.Tests.Live/CorrugatedIron.Tests.Live.nunit

clean:
	$(XBUILD) $(SLNDIR)/CorrugatedIron.sln /target:clean /property:Configuration=Debug /property:Mono=True
	$(XBUILD) $(SLNDIR)/CorrugatedIron.sln /target:clean /property:Configuration=Release /property:Mono=True
	find $(SLNDIR) -type f \( -name '*.err' -o -name '*.out' \) -delete

