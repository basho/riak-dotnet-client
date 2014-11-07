XBUILD = xbuild
MONO = mono
NUNIT = ./packages/NUnit.Runners.2.6.3/tools/nunit-console.exe
PROTOGEN = ~/bin/ProtoGen/protogen.exe
PROTOC = protoc

all: release

fixcerts:
	mozroots --import --sync

install-mono:
	./build/mono/install-mono

restorepkg:
	$(MONO) .nuget/NuGet.exe restore -PackagesDirectory ./packages -ConfigFile .nuget/NuGet.Config .nuget/packages.config
	$(MONO) .nuget/NuGet.exe restore -PackagesDirectory ./packages -ConfigFile .nuget/NuGet.Config CorrugatedIron/packages.config
	$(MONO) .nuget/NuGet.exe restore -PackagesDirectory ./packages -ConfigFile .nuget/NuGet.Config CorrugatedIron.Tests/packages.config
	$(MONO) .nuget/NuGet.exe restore -PackagesDirectory ./packages -ConfigFile .nuget/NuGet.Config CorrugatedIron.Tests.Live/packages.config

proto:
	cd CorrugatedIron/Messages
	$(MONO) $(PROTOGEN) -i:$< -o:$@
	rm $<

release: restorepkg
	$(XBUILD) ./CorrugatedIron.sln /property:Configuration=Release /property:Mono=True

debug: restorepkg
	$(XBUILD) ./CorrugatedIron.sln /property:Configuration=Debug /property:Mono=True

test: debug
	$(MONO) $(NUNIT) -config Debug-Mono -nologo -nodots -work=CorrugatedIron.Tests CorrugatedIron.Tests/CorrugatedIron.Tests.nunit

integration: debug test
	$(MONO) $(NUNIT) -config Debug-Mono -nologo -nodots -work=CorrugatedIron.Tests.Live CorrugatedIron.Tests.Live/CorrugatedIron.Tests.Live.nunit

clean:
	$(XBUILD) ./CorrugatedIron.sln /target:clean /property:Configuration=Debug /property:Mono=True
	$(XBUILD) ./CorrugatedIron.sln /target:clean /property:Configuration=Release /property:Mono=True

