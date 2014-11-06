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
	@$(XBUILD) ./CorrugatedIron.sln /property:Configuration=Release /property:Mono=True

debug:
	@$(XBUILD) ./CorrugatedIron.sln /property:Configuration=Debug /property:Mono=True

clean:
	rm -rf ./**/bin/
	rm -rf ./**/obj/
