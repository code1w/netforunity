using Sfs2X.Protocol;
using Sfs2X.Util;
using System;
namespace Sfs2X.Bitswarm
{
	public interface IoHandler
	{
		IProtocolCodec Codec
		{
			get;
		}
		void OnDataRead(ByteArray buffer);
		void OnDataWrite(IMessage message);
	}
}
