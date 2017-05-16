using Sfs2X.Core;
using Sfs2X.Util;
using System;
namespace Sfs2X.Bitswarm
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
