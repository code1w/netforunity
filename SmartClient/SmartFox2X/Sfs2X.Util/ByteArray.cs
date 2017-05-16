using ComponentAce.Compression.Libs.zlib;
using Sfs2X.Entities.Data;
using System;
using System.IO;
using System.Text;
namespace Sfs2X.Util
{
	public class ByteArray
	{
		private byte[] buffer;
		private int position = 0;
		private bool compressed = false;
		public byte[] Bytes
		{
			get
			{
				return this.buffer;
			}
			set
			{
				this.buffer = value;
				this.compressed = false;
			}
		}
		public int Length
		{
			get
			{
				return this.buffer.Length;
			}
		}
		public int Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
			}
		}
		public int BytesAvailable
		{
			get
			{
				int num = this.buffer.Length - this.position;
				if (num > this.buffer.Length || num < 0)
				{
					num = 0;
				}
				return num;
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
		public ByteArray()
		{
			this.buffer = new byte[0];
		}
		public ByteArray(byte[] buf)
		{
			this.buffer = buf;
		}
		public void Compress()
		{
			if (this.compressed)
			{
				throw new Exception("Buffer is already compressed");
			}
			MemoryStream memoryStream = new MemoryStream();
			using (ZOutputStream zOutputStream = new ZOutputStream(memoryStream, 9))
			{
				zOutputStream.Write(this.buffer, 0, this.buffer.Length);
				zOutputStream.Flush();
			}
			this.buffer = memoryStream.ToArray();
			this.position = 0;
			this.compressed = true;
		}
		public void Uncompress()
		{
			MemoryStream memoryStream = new MemoryStream();
			using (ZOutputStream zOutputStream = new ZOutputStream(memoryStream))
			{
				zOutputStream.Write(this.buffer, 0, this.buffer.Length);
				zOutputStream.Flush();
			}
			this.buffer = memoryStream.ToArray();
			this.position = 0;
			this.compressed = false;
		}
		private void CheckCompressedWrite()
		{
			if (this.compressed)
			{
				throw new Exception("Only raw bytes can be written a compressed array. Call Uncompress first.");
			}
		}
		private void CheckCompressedRead()
		{
			if (this.compressed)
			{
				throw new Exception("Only raw bytes can be read from a compressed array.");
			}
		}
		public byte[] ReverseOrder(byte[] dt)
		{
			byte[] result;
			if (!BitConverter.IsLittleEndian)
			{
				result = dt;
			}
			else
			{
				byte[] array = new byte[dt.Length];
				int num = 0;
				for (int i = dt.Length - 1; i >= 0; i--)
				{
					array[num++] = dt[i];
				}
				result = array;
			}
			return result;
		}
		public void WriteByte(SFSDataType tp)
		{
			this.WriteByte(Convert.ToByte((int)tp));
		}
		public void WriteByte(byte b)
		{
			this.WriteBytes(new byte[]
			{
				b
			});
		}
		public void WriteBytes(byte[] data)
		{
			this.WriteBytes(data, 0, data.Length);
		}
		public void WriteBytes(byte[] data, int ofs, int count)
		{
			byte[] dst = new byte[count + this.buffer.Length];
			Buffer.BlockCopy(this.buffer, 0, dst, 0, this.buffer.Length);
			Buffer.BlockCopy(data, ofs, dst, this.buffer.Length, count);
			this.buffer = dst;
		}
		public void WriteBool(bool b)
		{
			this.CheckCompressedWrite();
			this.WriteBytes(new byte[]
			{
				//(!b) ? 0 : 1
			});
		}
		public void WriteInt(int i)
		{
			this.CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(i);
			this.WriteBytes(this.ReverseOrder(bytes));
		}
		public void WriteUShort(ushort us)
		{
			this.CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(us);
			this.WriteBytes(this.ReverseOrder(bytes));
		}
		public void WriteShort(short s)
		{
			this.CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(s);
			this.WriteBytes(this.ReverseOrder(bytes));
		}
		public void WriteLong(long l)
		{
			this.CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(l);
			this.WriteBytes(this.ReverseOrder(bytes));
		}
		public void WriteFloat(float f)
		{
			this.CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(f);
			this.WriteBytes(this.ReverseOrder(bytes));
		}
		public void WriteDouble(double d)
		{
			this.CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(d);
			this.WriteBytes(this.ReverseOrder(bytes));
		}
		public void WriteUTF(string str)
		{
			this.CheckCompressedWrite();
			int num = 0;
			for (int i = 0; i < str.Length; i++)
			{
				int num2 = (int)str[i];
				if (num2 >= 1 && num2 <= 127)
				{
					num++;
				}
				else
				{
					if (num2 > 2047)
					{
						num += 3;
					}
					else
					{
						num += 2;
					}
				}
			}
			if (num > 32768)
			{
				throw new FormatException("String length cannot be greater then 32768 !");
			}
			this.WriteUShort(Convert.ToUInt16(num));
			this.WriteBytes(Encoding.UTF8.GetBytes(str));
		}
		public byte ReadByte()
		{
			this.CheckCompressedRead();
			return this.buffer[this.position++];
		}
		public byte[] ReadBytes(int count)
		{
			byte[] array = new byte[count];
			Buffer.BlockCopy(this.buffer, this.position, array, 0, count);
			this.position += count;
			return array;
		}
		public bool ReadBool()
		{
			this.CheckCompressedRead();
			return this.buffer[this.position++] == 1;
		}
		public int ReadInt()
		{
			this.CheckCompressedRead();
			byte[] value = this.ReverseOrder(this.ReadBytes(4));
			return BitConverter.ToInt32(value, 0);
		}
		public ushort ReadUShort()
		{
			this.CheckCompressedRead();
			byte[] value = this.ReverseOrder(this.ReadBytes(2));
			return BitConverter.ToUInt16(value, 0);
		}
		public short ReadShort()
		{
			this.CheckCompressedRead();
			byte[] value = this.ReverseOrder(this.ReadBytes(2));
			return BitConverter.ToInt16(value, 0);
		}
		public long ReadLong()
		{
			this.CheckCompressedRead();
			byte[] value = this.ReverseOrder(this.ReadBytes(8));
			return BitConverter.ToInt64(value, 0);
		}
		public float ReadFloat()
		{
			this.CheckCompressedRead();
			byte[] value = this.ReverseOrder(this.ReadBytes(4));
			return BitConverter.ToSingle(value, 0);
		}
		public double ReadDouble()
		{
			this.CheckCompressedRead();
			byte[] value = this.ReverseOrder(this.ReadBytes(8));
			return BitConverter.ToDouble(value, 0);
		}
		public string ReadUTF()
		{
			this.CheckCompressedRead();
			ushort num = this.ReadUShort();
			string @string = Encoding.UTF8.GetString(this.buffer, this.position, (int)num);
			this.position += (int)num;
			return @string;
		}
	}
}
