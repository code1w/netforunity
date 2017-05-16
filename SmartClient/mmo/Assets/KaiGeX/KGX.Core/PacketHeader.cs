using System;
using System.Text;
namespace KaiGeX.Core
{
	public class PacketHeader
	{
		private int expectedLength = -1;
		private bool binary = true;
		private bool compressed;
		private bool encrypted;
		private bool blueBoxed;
		private bool bigSized;
		public int ExpectedLength
		{
			get
			{
				return this.expectedLength;
			}
			set
			{
				this.expectedLength = value;
			}
		}
		public bool Encrypted
		{
			get
			{
				return this.encrypted;
			}
			set
			{
				this.encrypted = value;
			}
		}
		public bool Compressed
		{
			get
			{
				return this.compressed;
			}
			set
			{
				this.compressed = value;
			}
		}
		public bool BlueBoxed
		{
			get
			{
				return this.blueBoxed;
			}
			set
			{
				this.blueBoxed = value;
			}
		}
		public bool Binary
		{
			get
			{
				return this.binary;
			}
			set
			{
				this.binary = value;
			}
		}
		public bool BigSized
		{
			get
			{
				return this.bigSized;
			}
			set
			{
				this.bigSized = value;
			}
		}
		public PacketHeader(bool encrypted, bool compressed, bool blueBoxed, bool bigSized)
		{
			this.compressed = compressed;
			this.encrypted = encrypted;
			this.blueBoxed = blueBoxed;
			this.bigSized = bigSized;
		}
		public static PacketHeader FromBinary(int headerByte)
		{
			return new PacketHeader((headerByte & 64) > 0, (headerByte & 32) > 0, (headerByte & 16) > 0, (headerByte & 8) > 0);
		}
		public byte Encode()
		{
			byte b = 0;
			if (this.binary)
			{
				b |= 128;
			}
			if (this.Encrypted)
			{
				b |= 64;
			}
			if (this.Compressed)
			{
				b |= 32;
			}
			if (this.blueBoxed)
			{
				b |= 16;
			}
			if (this.bigSized)
			{
				b |= 8;
			}
			return b;
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("---------------------------------------------\n");
			stringBuilder.Append("Binary:  \t" + this.binary + "\n");
			stringBuilder.Append("Compressed:\t" + this.compressed + "\n");
			stringBuilder.Append("Encrypted:\t" + this.encrypted + "\n");
			stringBuilder.Append("BlueBoxed:\t" + this.blueBoxed + "\n");
			stringBuilder.Append("BigSized:\t" + this.bigSized + "\n");
			stringBuilder.Append("---------------------------------------------\n");
			return stringBuilder.ToString();
		}
	}
}
