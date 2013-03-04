XBUILD=`which xbuild`

all: release

release:
	@$(XBUILD) ./CorrugatedIron.sln /property:configuration=Release

debug:
	@$(XBUILD) ./CorrugatedIron.sln /property:configuration=Debug

clean:
	rm -rf ./**/bin/
	rm -rf ./**/obj/
