// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: PB_MsgData.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace FGUFW.Core {

  /// <summary>Holder for reflection information generated from PB_MsgData.proto</summary>
  public static partial class PBMsgDataReflection {

    #region Descriptor
    /// <summary>File descriptor for PB_MsgData.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static PBMsgDataReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChBQQl9Nc2dEYXRhLnByb3RvEgpGR1VGVy5Db3JlIjwKClBCX01zZ0RhdGES",
            "EAoIUGxheWVySUQYASABKAUSCwoDQ21kGAIgASgNEg8KB01zZ0RhdGEYAyAB",
            "KAwiSwoIUEJfRnJhbWUSDQoFSW5kZXgYASABKAUSEAoIUGxheWVySUQYAiAB",
            "KAUSDAoEQ21kcxgDIAEoDRIQCghNc2dEYXRhcxgEIAEoDGIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::FGUFW.Core.PB_MsgData), global::FGUFW.Core.PB_MsgData.Parser, new[]{ "PlayerID", "Cmd", "MsgData" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::FGUFW.Core.PB_Frame), global::FGUFW.Core.PB_Frame.Parser, new[]{ "Index", "PlayerID", "Cmds", "MsgDatas" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class PB_MsgData : pb::IMessage<PB_MsgData> {
    private static readonly pb::MessageParser<PB_MsgData> _parser = new pb::MessageParser<PB_MsgData>(() => new PB_MsgData());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<PB_MsgData> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::FGUFW.Core.PBMsgDataReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PB_MsgData() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PB_MsgData(PB_MsgData other) : this() {
      playerID_ = other.playerID_;
      cmd_ = other.cmd_;
      msgData_ = other.msgData_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PB_MsgData Clone() {
      return new PB_MsgData(this);
    }

    /// <summary>Field number for the "PlayerID" field.</summary>
    public const int PlayerIDFieldNumber = 1;
    private int playerID_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PlayerID {
      get { return playerID_; }
      set {
        playerID_ = value;
      }
    }

    /// <summary>Field number for the "Cmd" field.</summary>
    public const int CmdFieldNumber = 2;
    private uint cmd_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Cmd {
      get { return cmd_; }
      set {
        cmd_ = value;
      }
    }

    /// <summary>Field number for the "MsgData" field.</summary>
    public const int MsgDataFieldNumber = 3;
    private pb::ByteString msgData_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString MsgData {
      get { return msgData_; }
      set {
        msgData_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as PB_MsgData);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(PB_MsgData other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (PlayerID != other.PlayerID) return false;
      if (Cmd != other.Cmd) return false;
      if (MsgData != other.MsgData) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (PlayerID != 0) hash ^= PlayerID.GetHashCode();
      if (Cmd != 0) hash ^= Cmd.GetHashCode();
      if (MsgData.Length != 0) hash ^= MsgData.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (PlayerID != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(PlayerID);
      }
      if (Cmd != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Cmd);
      }
      if (MsgData.Length != 0) {
        output.WriteRawTag(26);
        output.WriteBytes(MsgData);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (PlayerID != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PlayerID);
      }
      if (Cmd != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Cmd);
      }
      if (MsgData.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(MsgData);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(PB_MsgData other) {
      if (other == null) {
        return;
      }
      if (other.PlayerID != 0) {
        PlayerID = other.PlayerID;
      }
      if (other.Cmd != 0) {
        Cmd = other.Cmd;
      }
      if (other.MsgData.Length != 0) {
        MsgData = other.MsgData;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            PlayerID = input.ReadInt32();
            break;
          }
          case 16: {
            Cmd = input.ReadUInt32();
            break;
          }
          case 26: {
            MsgData = input.ReadBytes();
            break;
          }
        }
      }
    }

  }

  public sealed partial class PB_Frame : pb::IMessage<PB_Frame> {
    private static readonly pb::MessageParser<PB_Frame> _parser = new pb::MessageParser<PB_Frame>(() => new PB_Frame());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<PB_Frame> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::FGUFW.Core.PBMsgDataReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PB_Frame() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PB_Frame(PB_Frame other) : this() {
      index_ = other.index_;
      playerID_ = other.playerID_;
      cmds_ = other.cmds_;
      msgDatas_ = other.msgDatas_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PB_Frame Clone() {
      return new PB_Frame(this);
    }

    /// <summary>Field number for the "Index" field.</summary>
    public const int IndexFieldNumber = 1;
    private int index_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Index {
      get { return index_; }
      set {
        index_ = value;
      }
    }

    /// <summary>Field number for the "PlayerID" field.</summary>
    public const int PlayerIDFieldNumber = 2;
    private int playerID_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int PlayerID {
      get { return playerID_; }
      set {
        playerID_ = value;
      }
    }

    /// <summary>Field number for the "Cmds" field.</summary>
    public const int CmdsFieldNumber = 3;
    private uint cmds_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Cmds {
      get { return cmds_; }
      set {
        cmds_ = value;
      }
    }

    /// <summary>Field number for the "MsgDatas" field.</summary>
    public const int MsgDatasFieldNumber = 4;
    private pb::ByteString msgDatas_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString MsgDatas {
      get { return msgDatas_; }
      set {
        msgDatas_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as PB_Frame);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(PB_Frame other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Index != other.Index) return false;
      if (PlayerID != other.PlayerID) return false;
      if (Cmds != other.Cmds) return false;
      if (MsgDatas != other.MsgDatas) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Index != 0) hash ^= Index.GetHashCode();
      if (PlayerID != 0) hash ^= PlayerID.GetHashCode();
      if (Cmds != 0) hash ^= Cmds.GetHashCode();
      if (MsgDatas.Length != 0) hash ^= MsgDatas.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Index != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Index);
      }
      if (PlayerID != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(PlayerID);
      }
      if (Cmds != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(Cmds);
      }
      if (MsgDatas.Length != 0) {
        output.WriteRawTag(34);
        output.WriteBytes(MsgDatas);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Index != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Index);
      }
      if (PlayerID != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PlayerID);
      }
      if (Cmds != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Cmds);
      }
      if (MsgDatas.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(MsgDatas);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(PB_Frame other) {
      if (other == null) {
        return;
      }
      if (other.Index != 0) {
        Index = other.Index;
      }
      if (other.PlayerID != 0) {
        PlayerID = other.PlayerID;
      }
      if (other.Cmds != 0) {
        Cmds = other.Cmds;
      }
      if (other.MsgDatas.Length != 0) {
        MsgDatas = other.MsgDatas;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Index = input.ReadInt32();
            break;
          }
          case 16: {
            PlayerID = input.ReadInt32();
            break;
          }
          case 24: {
            Cmds = input.ReadUInt32();
            break;
          }
          case 34: {
            MsgDatas = input.ReadBytes();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
