MONO = mono
PROTOGEN = ~/bin/ProtoGen/protogen.exe
PROTOC = protoc

.PHONY : all clean

compiled := $(patsubst %.proto,%.cs,$(wildcard *.proto))

all: $(compiled)

%.cs : %.fds
        $(MONO) $(PROTOGEN) -i:$< -o:$@
        rm $<

%.fds : %.proto
        $(PROTOC) $< -o $@

clean: 
        rm -f *.cs *.fds
