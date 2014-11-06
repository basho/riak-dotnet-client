XBUILD=`which xbuild`
MONO = mono
PROTOGEN = ~/bin/ProtoGen/protogen.exe
PROTOC = protoc

all: release

fixcerts:
	mozroots --import --sync

restorepkg:
	$(MONO) .nuget/NuGet.exe restore -PackagesDirectory ./packages -ConfigFile .nuget/NuGet.Config .nuget/packages.config
	$(MONO) .nuget/NuGet.exe restore -PackagesDirectory ./packages -ConfigFile .nuget/NuGet.Config CorrugatedIron/packages.config
	$(MONO) .nuget/NuGet.exe restore -PackagesDirectory ./packages -ConfigFile .nuget/NuGet.Config CorrugatedIron.Tests/packages.config
	$(MONO) .nuget/NuGet.exe restore -PackagesDirectory ./packages -ConfigFile .nuget/NuGet.Config CorrugatedIron.Tests.Live/packages.config

proto:
	cd CorrugatedIron/Messages
	$(MONO) $(PROTOGEN) -i:$< -o:$@
	rm $<

release:
	@$(XBUILD) ./CorrugatedIron.sln /property:Configuration=Release /property:Mono=True

debug:
	@$(XBUILD) ./CorrugatedIron.sln /property:Configuration=Debug /property:Mono=True

clean:
	rm -rf ./**/bin/
	rm -rf ./**/obj/
