// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Lobby.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace JinJvLi.Lobby {

  /// <summary>Holder for reflection information generated from Lobby.proto</summary>
  public static partial class LobbyReflection {

    #region Descriptor
    /// <summary>File descriptor for Lobby.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static LobbyReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgtMb2JieS5wcm90bxINSmluSnZMaS5Mb2JieSIoCghHYW1lUm9vbRIQCghH",
            "YW1lTmFtZRgBIAEoCRIKCgJJRBgCIAEoBWIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::JinJvLi.Lobby.GameRoom), global::JinJvLi.Lobby.GameRoom.Parser, new[]{ "GameName", "ID" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class GameRoom : pb::IMessage<GameRoom> {
    private static readonly pb::MessageParser<GameRoom> _parser = new pb::MessageParser<GameRoom>(() => new GameRoom());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<GameRoom> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::JinJvLi.Lobby.LobbyReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GameRoom() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GameRoom(GameRoom other) : this() {
      gameName_ = other.gameName_;
      iD_ = other.iD_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GameRoom Clone() {
      return new GameRoom(this);
    }

    /// <summary>Field number for the "GameName" field.</summary>
    public const int GameNameFieldNumber = 1;
    private string gameName_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string GameName {
      get { return gameName_; }
      set {
        gameName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "ID" field.</summary>
    public const int IDFieldNumber = 2;
    private int iD_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int ID {
      get { return iD_; }
      set {
        iD_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as GameRoom);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(GameRoom other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (GameName != other.GameName) return false;
      if (ID != other.ID) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (GameName.Length != 0) hash ^= GameName.GetHashCode();
      if (ID != 0) hash ^= ID.GetHashCode();
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
      if (GameName.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(GameName);
      }
      if (ID != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(ID);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (GameName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(GameName);
      }
      if (ID != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ID);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(GameRoom other) {
      if (other == null) {
        return;
      }
      if (other.GameName.Length != 0) {
        GameName = other.GameName;
      }
      if (other.ID != 0) {
        ID = other.ID;
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
          case 10: {
            GameName = input.ReadString();
            break;
          }
          case 16: {
            ID = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
