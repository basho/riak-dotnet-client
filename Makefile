XBUILD=`which xbuild`

all: compile

compile:
	@$(XBUILD) ./CorrugatedIron.sln /property:configuration=Release

clean:
	rm -rf ./**/bin/
	rm -rf ./**/obj/
