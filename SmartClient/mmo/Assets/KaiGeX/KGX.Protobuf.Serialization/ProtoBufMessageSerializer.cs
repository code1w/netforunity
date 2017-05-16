using System;
using System.Collections.Generic;
// System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace KaiGeX.Protobuf.Serialization
{
	/// <summary>
	/// <see cref="ProtoBuf.IExtensible"/>对象和字节流间的编码器
	/// </summary>
    public class ProtoBufMessageSerializer
	{
		#region Serialize
		public byte[] Serialize<T>(T message)
			where T : ProtoBuf.IExtensible
		{
			var stream = new MemoryStream();
			Serialize(message, stream);
			return stream.ToArray();
		}

		public int Serialize<T>(T message, Stream dest)
			where T : ProtoBuf.IExtensible
		{
			//Debug.Assert(ProtoBuf.Serializer.NonGeneric.CanSerialize(message.GetType()));
            //写名称
            string messagename = message.GetType().FullName;
            byte[] byteArray = Encoding.Default.GetBytes(messagename);
            dest.Write(byteArray, 0, byteArray.Length);
            dest.WriteByte(0);
            //serialize Type
            SerializeImpl(dest, message);
            return messagename.Length + 1;
		}

		private static void SerializeImpl<T>(Stream stream, T message)
			where T : ProtoBuf.IExtensible
		{
			ProtoBuf.Serializer.NonGeneric.Serialize(stream, message);
		}
		#endregion

		#region Deserialize
		public ProtoBuf.IExtensible Deserialize(int msgnamelength, byte[] messagePackageData, int offset, int count)
		{
			return DeserializeImpl(msgnamelength, new MemoryStream(messagePackageData, offset, count));
		}

        private ProtoBuf.IExtensible DeserializeImpl(int msgnamelength, Stream stream)
		{
            StreamReader reader = new StreamReader(stream);
            string messagename = reader.ReadLine();

            System.Type type = MessageDispatcher.getTypeByStr(messagename.Substring(0, msgnamelength - 1));

            stream.Position = msgnamelength;
            return (ProtoBuf.IExtensible)ProtoBuf.Serializer.NonGeneric.Deserialize(type, stream);
		}
		#endregion

	}
}
