using KaiGeX.Bitswarm;
using KaiGeX.Entities.Data;
using KaiGeX.Util;
using System;
namespace KaiGeX.Protocol
{
	public interface IProtocolCodec
	{
		IoHandler IOHandler
		{
			get;
			set;
		}
		void OnPacketRead(ISFSObject packet);
		void OnPacketRead(ByteArray packet);
        void OnPacketRead(ProtoBuf.IExtensible message);
		void OnPacketWrite(IMessage message);
        void OnProtoBufPacketWrite(ProtoBuf.IExtensible message);
	}
}
