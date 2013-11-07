cd CorrugatedIron\Messages

"C:\Program Files (x86)\protobuf-net\protobuf-net-VS9\protogen.exe" -ns:CorrugatedIron.Messages -i:riak.proto -o:riak.cs
"C:\Program Files (x86)\protobuf-net\protobuf-net-VS9\protogen.exe" -ns:CorrugatedIron.Messages -i:riak_dt.proto -o:riak_dt.cs
"C:\Program Files (x86)\protobuf-net\protobuf-net-VS9\protogen.exe" -ns:CorrugatedIron.Messages -i:riak_kv.proto -o:riak_kv.cs
"C:\Program Files (x86)\protobuf-net\protobuf-net-VS9\protogen.exe" -ns:CorrugatedIron.Messages -i:riak_search.proto -o:riak_search.cs
"C:\Program Files (x86)\protobuf-net\protobuf-net-VS9\protogen.exe" -ns:CorrugatedIron.Messages -i:riak_yokozuna.proto -o:riak_yokozuna.cs

cd ..\..