using KaiGeX.Entities.Data;
using System;
using System.Text;
namespace KaiGeX.Bitswarm
{
	public class Message : IMessage
	{
		private int id;
		private ISFSObject content;
		private int targetController;
		private bool isEncrypted;
		private bool isUDP;
		private long packetId;
		public int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}
		public ISFSObject Content
		{
			get
			{
				return this.content;
			}
			set
			{
				this.content = value;
			}
		}
		public int TargetController
		{
			get
			{
				return this.targetController;
			}
			set
			{
				this.targetController = value;
			}
		}
		public bool IsEncrypted
		{
			get
			{
				return this.isEncrypted;
			}
			set
			{
				this.isEncrypted = value;
			}
		}
		public bool IsUDP
		{
			get
			{
				return this.isUDP;
			}
			set
			{
				this.isUDP = value;
			}
		}
		public long PacketId
		{
			get
			{
				return this.packetId;
			}
			set
			{
				this.packetId = value;
			}
		}
		public Message()
		{
			this.isEncrypted = false;
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("{ Message id: " + this.id + " }\n");
			stringBuilder.Append("{ Dump: }\n");
			stringBuilder.Append(this.content.GetDump());
			return stringBuilder.ToString();
		}
	}
}
