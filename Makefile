XBUILD=`which xbuild`
MONO = mono
PROTOGEN = ~/bin/ProtoGen/protogen.exe
PROTOC = protoc

all: release

proto:
	cd CorrugatedIron/Messages
	$(MONO) $(PROTOGEN) -i:$< -o:$@
	rm $<

release:
	@$(XBUILD) ./CorrugatedIron.sln /property:configuration=Release

debug:
	@$(XBUILD) ./CorrugatedIron.sln /property:configuration=Debug

clean:
	rm -rf ./**/bin/
	rm -rf ./**/obj/
