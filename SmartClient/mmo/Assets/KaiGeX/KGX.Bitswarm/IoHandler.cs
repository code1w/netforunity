using KaiGeX.Protocol;
using KaiGeX.Util;
using System;
namespace KaiGeX.Bitswarm
{
	public interface IoHandler
	{
		IProtocolCodec Codec
		{
			get;
		}
		void OnDataRead(ByteArray buffer);
        void OnDataRead(ByteArray buffer, int msgTypeLen);
		void OnDataWrite(IMessage message);
        void OnProtoBufDataWrite(ProtoBuf.IExtensible message);
        void OnProtoBufDataWriteOnlyHeaderTest(ProtoBuf.IExtensible message);
	}
}
