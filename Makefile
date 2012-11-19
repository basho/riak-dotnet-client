XBUILD=`which xbuild`

all: compile

compile:
	@$(XBUILD) ./CorrugatedIron.sln

