using System;
using System.Collections.Generic;
using System.Text;
//using System.Runtime.InteropServices;
using System.Diagnostics;

namespace KaiGeX.Core
{
	/// <summary>
	/// 网络封包包头
	/// </summary>
	//[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ProtoBufPackageHeader
	{
		/// <summary>
		/// 有效负载的类型
		/// </summary>
		// public PackageHeadFlags Flags;
		/// <summary>
		/// 随后附带的有效负载字节长度
		/// </summary>
        public int MessageLength;
        public byte MessageType;
        public int MessageTypeLength;  

		private readonly static int s_size;
		/// <summary>
		/// 包头的字节大小
		/// </summary>
		public static int SizeOf { get { return s_size; } }

        static ProtoBufPackageHeader()
		{
			//unsafe { s_size = sizeof(PackageHead); }
            //s_size = Marshal.SizeOf(typeof(ProtoBufPackageHeader));
           // s_size = typeof(ProtoBufPackageHeader)
		}

        public int ReturnHeaderLen()
        {
			/*
            int HeaderLen = 0;
            HeaderLen += BitConverter.GetBytes(this.MessageLength).Length;
            HeaderLen += BitConverter.GetBytes(this.MessageType).Length;
            HeaderLen += BitConverter.GetBytes(this.MessageTypeLength).Length;
            return HeaderLen;
            */
			return 9;

        }
		public void WriteTo(System.IO.Stream stream)
		{
			byte[] buf = new byte[SizeOf];
			WriteTo(buf, 0);
			stream.Write(buf, 0, buf.Length);
		}

		public void WriteTo(byte[] buf, int index)
		{
			//CheckBuf(buf, index);
            //-------------------------------
            //2014-1-17 zxb
            Buffer.BlockCopy(BitConverter.GetBytes(this.MessageLength), 0, buf, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(this.MessageType), 0, buf, 4, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(this.MessageTypeLength), 0, buf, 5, 4);

		}
		public void ReadFrom(byte[] buf, int offset)
		{
           
			//CheckBuf(buf, offset);
            UnityEngine.Debug.Log("in-->ReadFrom()----Fun--->");
            byte[] byteMsgLen = new byte[4];
            byte[] byteMsgType = new byte[1];
            byte[] byteMsgTypeLen = new byte[4];
            Buffer.BlockCopy(buf, 0, byteMsgLen, 0, 4);
            Buffer.BlockCopy(buf, 4, byteMsgType, 0, 1);
            Buffer.BlockCopy(buf, 5, byteMsgTypeLen, 0, 4);
            this.MessageLength = BitConverter.ToInt32(byteMsgLen,0);
            this.MessageType = byteMsgType[0];
            this.MessageTypeLength = BitConverter.ToInt32(byteMsgTypeLen, 0);
		}

		private static void CheckBuf(byte[] buf, int index)
		{
			if (buf == null)
				throw new ArgumentNullException("buf");
			if (index < 0 || index + SizeOf > buf.Length)
				throw new ArgumentOutOfRangeException("index");
		}
	}
}
