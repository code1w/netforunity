using KaiGeX.Util;
using System;
namespace KaiGeX.Core
{
	public delegate void WriteBinaryDataDelegate(PacketHeader header, ByteArray binData, bool udp);
    public delegate void WriteProtoBufBinaryDataDelegate(ProtoBufPackageHeader header, ByteArray binData);
}
