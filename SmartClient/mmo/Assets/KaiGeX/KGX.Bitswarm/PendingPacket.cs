using KaiGeX.Core;
using KaiGeX.Util;
using System;
namespace KaiGeX.Bitswarm
{
	public class PendingPacket
	{
		private PacketHeader header;
		private ByteArray buffer;
		public PacketHeader Header
		{
			get
			{
				return this.header;
			}
		}
		public ByteArray Buffer
		{
			get
			{
				return this.buffer;
			}
			set
			{
				this.buffer = value;
			}
		}
		public PendingPacket(PacketHeader header)
		{
			this.header = header;
			this.buffer = new ByteArray();
			this.buffer.Compressed = header.Compressed;
		}
	}
}
