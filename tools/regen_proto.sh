#!/usr/bin/env bash

pushd ../CorrugatedIron/Messages

PATH=$PATH:$HOME/bin/ProtoGen

cipath=$(pwd)
cd $HOME/bin/ProtoGen

mono protogen.exe -ns:CorrugatedIron.messages -i:$cipath/riak.proto -o:$cipath/riak.cs
#    mono ~/bin/ProtoGen/protogen.exe -ns:CorrugatedIron.messages -i:riak_dt.proto -o:riak_dt.cs
#    mono ~/bin/ProtoGen/protogen.exe -ns:CorrugatedIron.messages -i:riak_kv.proto -o:riak_kv.cs
#    mono ~/bin/ProtoGen/protogen.exe -ns:CorrugatedIron.messages -i:riak_search.proto -o:riak_search.cs
#    mono ~/bin/ProtoGen/protogen.exe -ns:CorrugatedIron.messages -i:riak_yokozuna.proto -o:riak_yokozuna.cs

popd
