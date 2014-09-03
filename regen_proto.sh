cd CorrugatedIron/Messages;

protogen() {
    local old_path=$PATH;
    PATH=$PATH:~/bin/ProtoGen;

    local cipath=pwd;
    cd ~/bin/ProtoGen;

    mono protogen.exe -ns:CorrugatedIron.messages -i:$cipath/riak.proto -o:$cipath/riak.cs
    mono protogen.exe -ns:CorrugatedIron.messages -i:$cipath/riak_dt.proto -o:$cipath/riak_dt.cs
    mono protogen.exe -ns:CorrugatedIron.messages -i:$cipath/riak_kv.proto -o:$cipath/riak_kv.cs
    mono protogen.exe -ns:CorrugatedIron.messages -i:$cipath/riak_search.proto -o:$cipath/riak_search.cs
    mono protogen.exe -ns:CorrugatedIron.messages -i:$cipath/riak_yokozuna.proto -o:$cipath/riak_yokozuna.cs
    PATH=$old_path;
}

protogen;

cd ../..
