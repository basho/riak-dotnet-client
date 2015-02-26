// <copyright file="riak_yokozuna.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

#pragma warning disable 1591
namespace RiakClient.Messages
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RpbYokozunaIndex")]
  public partial class RpbYokozunaIndex : global::ProtoBuf.IExtensible
  {
    public RpbYokozunaIndex() {}
    
    private byte[] _name;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] name
    {
      get { return _name; }
      set { _name = value; }
    }
    private byte[] _schema = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"schema", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] schema
    {
      get { return _schema; }
      set { _schema = value; }
    }
    private uint _n_val = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"n_val", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint n_val
    {
      get { return _n_val; }
      set { _n_val = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RpbYokozunaIndexGetReq")]
  public partial class RpbYokozunaIndexGetReq : global::ProtoBuf.IExtensible
  {
    public RpbYokozunaIndexGetReq() {}
    
    private byte[] _name = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] name
    {
      get { return _name; }
      set { _name = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RpbYokozunaIndexGetResp")]
  public partial class RpbYokozunaIndexGetResp : global::ProtoBuf.IExtensible
  {
    public RpbYokozunaIndexGetResp() {}
    
    private readonly global::System.Collections.Generic.List<RpbYokozunaIndex> _index = new global::System.Collections.Generic.List<RpbYokozunaIndex>();
    [global::ProtoBuf.ProtoMember(1, Name=@"index", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<RpbYokozunaIndex> index
    {
      get { return _index; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RpbYokozunaIndexPutReq")]
  public partial class RpbYokozunaIndexPutReq : global::ProtoBuf.IExtensible
  {
    public RpbYokozunaIndexPutReq() {}
    
    private RpbYokozunaIndex _index;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"index", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public RpbYokozunaIndex index
    {
      get { return _index; }
      set { _index = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RpbYokozunaIndexDeleteReq")]
  public partial class RpbYokozunaIndexDeleteReq : global::ProtoBuf.IExtensible
  {
    public RpbYokozunaIndexDeleteReq() {}
    
    private byte[] _name;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] name
    {
      get { return _name; }
      set { _name = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RpbYokozunaSchema")]
  public partial class RpbYokozunaSchema : global::ProtoBuf.IExtensible
  {
    public RpbYokozunaSchema() {}
    
    private byte[] _name;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] name
    {
      get { return _name; }
      set { _name = value; }
    }
    private byte[] _content = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"content", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] content
    {
      get { return _content; }
      set { _content = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RpbYokozunaSchemaPutReq")]
  public partial class RpbYokozunaSchemaPutReq : global::ProtoBuf.IExtensible
  {
    public RpbYokozunaSchemaPutReq() {}
    
    private RpbYokozunaSchema _schema;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"schema", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public RpbYokozunaSchema schema
    {
      get { return _schema; }
      set { _schema = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RpbYokozunaSchemaGetReq")]
  public partial class RpbYokozunaSchemaGetReq : global::ProtoBuf.IExtensible
  {
    public RpbYokozunaSchemaGetReq() {}
    
    private byte[] _name;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] name
    {
      get { return _name; }
      set { _name = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RpbYokozunaSchemaGetResp")]
  public partial class RpbYokozunaSchemaGetResp : global::ProtoBuf.IExtensible
  {
    public RpbYokozunaSchemaGetResp() {}
    
    private RpbYokozunaSchema _schema;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"schema", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public RpbYokozunaSchema schema
    {
      get { return _schema; }
      set { _schema = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}
