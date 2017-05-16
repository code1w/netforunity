﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: objectsys.proto
// Note: requires additional types generated from: common.proto
using common;
namespace clientmsg
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CompanyCheck")]
  public partial class CompanyCheck : global::ProtoBuf.IExtensible
  {
    public CompanyCheck() {}
    

    private uint _personid = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"personid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint personid
    {
      get { return _personid; }
      set { _personid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PersonPost")]
  public partial class PersonPost : global::ProtoBuf.IExtensible
  {
    public PersonPost() {}
    

    private uint _companyid = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"companyid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint companyid
    {
      get { return _companyid; }
      set { _companyid = value; }
    }

    private uint _stationid = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"stationid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint stationid
    {
      get { return _stationid; }
      set { _stationid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CompanyPass")]
  public partial class CompanyPass : global::ProtoBuf.IExtensible
  {
    public CompanyPass() {}
    

    private uint _personid = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"personid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint personid
    {
      get { return _personid; }
      set { _personid = value; }
    }

    private uint _stationid = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"stationid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint stationid
    {
      get { return _stationid; }
      set { _stationid = value; }
    }

    private enumCompanyTalentType _talentype = enumCompanyTalentType.enumCompanyTalent_Recycle;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"talentype", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(enumCompanyTalentType.enumCompanyTalent_Recycle)]
    public enumCompanyTalentType talentype
    {
      get { return _talentype; }
      set { _talentype = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CompanyNoPass")]
  public partial class CompanyNoPass : global::ProtoBuf.IExtensible
  {
    public CompanyNoPass() {}
    

    private uint _personid = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"personid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint personid
    {
      get { return _personid; }
      set { _personid = value; }
    }

    private uint _stationid = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"stationid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint stationid
    {
      get { return _stationid; }
      set { _stationid = value; }
    }

    private enumCompanyTalentType _talentype = enumCompanyTalentType.enumCompanyTalent_Recycle;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"talentype", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(enumCompanyTalentType.enumCompanyTalent_Recycle)]
    public enumCompanyTalentType talentype
    {
      get { return _talentype; }
      set { _talentype = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"InformPersonPass")]
  public partial class InformPersonPass : global::ProtoBuf.IExtensible
  {
    public InformPersonPass() {}
    

    private uint _companyid = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"companyid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint companyid
    {
      get { return _companyid; }
      set { _companyid = value; }
    }

    private uint _statinid = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"statinid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint statinid
    {
      get { return _statinid; }
      set { _statinid = value; }
    }

    private uint _passtime = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"passtime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint passtime
    {
      get { return _passtime; }
      set { _passtime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"InformPersonWhoCheck")]
  public partial class InformPersonWhoCheck : global::ProtoBuf.IExtensible
  {
    public InformPersonWhoCheck() {}
    

    private uint _companyid = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"companyid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint companyid
    {
      get { return _companyid; }
      set { _companyid = value; }
    }

    private uint _checktime = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"checktime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint checktime
    {
      get { return _checktime; }
      set { _checktime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"InformPersNoPass")]
  public partial class InformPersNoPass : global::ProtoBuf.IExtensible
  {
    public InformPersNoPass() {}
    

    private uint _companyid = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"companyid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint companyid
    {
      get { return _companyid; }
      set { _companyid = value; }
    }

    private uint _nopasstime = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"nopasstime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint nopasstime
    {
      get { return _nopasstime; }
      set { _nopasstime = value; }
    }

    private uint _stationid = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"stationid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint stationid
    {
      get { return _stationid; }
      set { _stationid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"InformUsedPost")]
  public partial class InformUsedPost : global::ProtoBuf.IExtensible
  {
    public InformUsedPost() {}
    

    private uint _companyid = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"companyid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint companyid
    {
      get { return _companyid; }
      set { _companyid = value; }
    }

    private uint _stationid = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"stationid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint stationid
    {
      get { return _stationid; }
      set { _stationid = value; }
    }

    private uint _posttime = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"posttime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint posttime
    {
      get { return _posttime; }
      set { _posttime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"InformCompanyWhoPost")]
  public partial class InformCompanyWhoPost : global::ProtoBuf.IExtensible
  {
    public InformCompanyWhoPost() {}
    

    private uint _personid = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"personid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint personid
    {
      get { return _personid; }
      set { _personid = value; }
    }

    private uint _posttime = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"posttime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint posttime
    {
      get { return _posttime; }
      set { _posttime = value; }
    }

    private uint _stationid = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"stationid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint stationid
    {
      get { return _stationid; }
      set { _stationid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"InformAutoFriend")]
  public partial class InformAutoFriend : global::ProtoBuf.IExtensible
  {
    public InformAutoFriend() {}
    

    private uint _id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint id
    {
      get { return _id; }
      set { _id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"IsOnlineOrFriend")]
  public partial class IsOnlineOrFriend : global::ProtoBuf.IExtensible
  {
    public IsOnlineOrFriend() {}
    

    private uint _isonline = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"isonline", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint isonline
    {
      get { return _isonline; }
      set { _isonline = value; }
    }

    private uint _isfriend = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"isfriend", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint isfriend
    {
      get { return _isfriend; }
      set { _isfriend = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}