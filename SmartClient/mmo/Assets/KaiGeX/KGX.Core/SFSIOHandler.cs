using KaiGeX.Bitswarm;
using KaiGeX.Exceptions;
using KaiGeX.FSM;
using KaiGeX.Logging;
using KaiGeX.Protocol;
using KaiGeX.Protocol.Serialization;
using KaiGeX.Util;
using System;
using System.IO;
using KaiGeX.Protobuf.Serialization;


using System.Text;

namespace KaiGeX.Core
{
	public class SFSIOHandler : IoHandler
	{
		public static readonly int SHORT_BYTE_SIZE = 2;
		public static readonly int INT_BYTE_SIZE = 4;
		private readonly ByteArray EMPTY_BUFFER = new ByteArray();
		private BitSwarmClient bitSwarm;
		private Logger log;
		private PendingPacket pendingPacket;
		private IProtocolCodec protocolCodec;
		private int skipBytes = 0;
		private FiniteStateMachine fsm;
        private ProtoBufMessageSerializer protoBufserializer;
		public IProtocolCodec Codec
		{
			get
			{
				return this.protocolCodec;
			}
		}
		private PacketReadState ReadState
		{
			get
			{
				return (PacketReadState)this.fsm.GetCurrentState();
			}
		}
		public SFSIOHandler(BitSwarmClient bitSwarm)
		{
			this.bitSwarm = bitSwarm;
			this.log = bitSwarm.Log;
			this.protocolCodec = new SFSProtocolCodec(this, bitSwarm);
			this.InitStates();
		}
		private void InitStates()
		{
			this.fsm = new FiniteStateMachine();
			this.fsm.AddAllStates(typeof(PacketReadState));
			this.fsm.AddStateTransition(PacketReadState.WAIT_NEW_PACKET, PacketReadState.WAIT_DATA_SIZE, PacketReadTransition.HeaderReceived);
			this.fsm.AddStateTransition(PacketReadState.WAIT_DATA_SIZE, PacketReadState.WAIT_DATA, PacketReadTransition.SizeReceived);
			this.fsm.AddStateTransition(PacketReadState.WAIT_DATA_SIZE, PacketReadState.WAIT_DATA_SIZE_FRAGMENT, PacketReadTransition.IncompleteSize);
			this.fsm.AddStateTransition(PacketReadState.WAIT_DATA_SIZE_FRAGMENT, PacketReadState.WAIT_DATA, PacketReadTransition.WholeSizeReceived);
			this.fsm.AddStateTransition(PacketReadState.WAIT_DATA, PacketReadState.WAIT_NEW_PACKET, PacketReadTransition.PacketFinished);
			this.fsm.AddStateTransition(PacketReadState.WAIT_DATA, PacketReadState.INVALID_DATA, PacketReadTransition.InvalidData);
			this.fsm.AddStateTransition(PacketReadState.INVALID_DATA, PacketReadState.WAIT_NEW_PACKET, PacketReadTransition.InvalidDataFinished);
			this.fsm.SetCurrentState(PacketReadState.WAIT_NEW_PACKET);

			//---------------------------------------------
			//Init protoBufserializer
			this.protoBufserializer = new ProtoBufMessageSerializer();
		}
		public void OnDataRead(ByteArray data)
		{
			if (data.Length == 0)
			{
				throw new SFSError("Unexpected empty packet data: no readable bytes available!");
			}
			if (this.bitSwarm != null && this.bitSwarm.Sfs.Debug)
			{
				if (data.Length > 1024)
				{
					this.log.Info(new string[]
					{
						"Data Read: Size > 1024, dump omitted"
					});
				}
				else
				{
					this.log.Info(new string[]
					{
						"Data Read: " + DefaultObjectDumpFormatter.HexDump(data)
					});
				}
			}
			data.Position = 0;
			while (data.Length > 0)
			{
				if (this.ReadState == PacketReadState.WAIT_NEW_PACKET)
				{
					data = this.HandleNewPacket(data);
				}
				else
				{
					if (this.ReadState == PacketReadState.WAIT_DATA_SIZE)
					{
						data = this.HandleDataSize(data);
					}
					else
					{
						if (this.ReadState == PacketReadState.WAIT_DATA_SIZE_FRAGMENT)
						{
							data = this.HandleDataSizeFragment(data);
						}
						else
						{
							if (this.ReadState == PacketReadState.WAIT_DATA)
							{
								data = this.HandlePacketData(data);
							}
							else
							{
								if (this.ReadState == PacketReadState.INVALID_DATA)
								{
									data = this.HandleInvalidData(data);
								}
							}
						}
					}
				}
			}
		}
        public void OnDataRead(ByteArray data, int msgNameLen)
        {
            if (data.Length == 0)
            {
                throw new SFSError("Unexpected empty packet data: no readable bytes available!");
            }
           // UnityEngine.Debug.Log("SFSIOHandler--call OnDataProtoBufRead --" + DefaultObjectDumpFormatter.HexDump(data));
            UnityEngine.Debug.Log("反序列化消息名字长度:  " + msgNameLen);
            UnityEngine.Debug.Log("反序列化消息参数据度:  " + data.Bytes.Length);
            UnityEngine.Debug.Log("反序列化消息内容:   " + DefaultObjectDumpFormatter.HexDump(data));
            ProtoBuf.IExtensible message = this.protoBufserializer.Deserialize(msgNameLen, data.Bytes, 0, data.Bytes.Length);
            UnityEngine.Debug.Log("反序列化消息:  " + message.GetType().FullName.ToString(null) +"   OK!");
            HandleProtoBufPacketData(message);
        }
		private ByteArray HandleNewPacket(ByteArray data)
		{
			this.log.Debug(new string[]
			{
				"Handling New Packet of size " + data.Length
			});
			byte b = data.ReadByte();
			if (~(b & 128) > 0)
			{
				throw new SFSError(string.Concat(new object[]
				{
					"Unexpected header byte: ",
					b,
					"\n",
					DefaultObjectDumpFormatter.HexDump(data)
				}));
			}
			PacketHeader header = PacketHeader.FromBinary((int)b);
			this.pendingPacket = new PendingPacket(header);
			this.fsm.ApplyTransition(PacketReadTransition.HeaderReceived);
			return this.ResizeByteArray(data, 1, data.Length - 1);
		}
		private ByteArray HandleDataSize(ByteArray data)
		{
			this.log.Debug(new string[]
			{
				string.Concat(new object[]
				{
					"Handling Header Size. Length: ",
					data.Length,
					" (",
					(!this.pendingPacket.Header.BigSized) ? "small" : "big",
					")"
				})
			});
			int num = -1;
			int num2 = SFSIOHandler.SHORT_BYTE_SIZE;
			if (this.pendingPacket.Header.BigSized)
			{
				if (data.Length >= SFSIOHandler.INT_BYTE_SIZE)
				{
					num = data.ReadInt();
				}
				num2 = 4;
			}
			else
			{
				if (data.Length >= SFSIOHandler.SHORT_BYTE_SIZE)
				{
					num = (int)data.ReadUShort();
				}
			}
			this.log.Debug(new string[]
			{
				"Data size is " + num
			});
			if (num != -1)
			{
				this.pendingPacket.Header.ExpectedLength = num;
				data = this.ResizeByteArray(data, num2, data.Length - num2);
				this.fsm.ApplyTransition(PacketReadTransition.SizeReceived);
			}
			else
			{
				this.fsm.ApplyTransition(PacketReadTransition.IncompleteSize);
				this.pendingPacket.Buffer.WriteBytes(data.Bytes);
				data = this.EMPTY_BUFFER;
			}
			return data;
		}
		private ByteArray HandleDataSizeFragment(ByteArray data)
		{
			this.log.Debug(new string[]
			{
				"Handling Size fragment. Data: " + data.Length
			});
			int num = (!this.pendingPacket.Header.BigSized) ? (SFSIOHandler.SHORT_BYTE_SIZE - this.pendingPacket.Buffer.Length) : (SFSIOHandler.INT_BYTE_SIZE - this.pendingPacket.Buffer.Length);
			if (data.Length >= num)
			{
				this.pendingPacket.Buffer.WriteBytes(data.Bytes, 0, num);
				int count = (!this.pendingPacket.Header.BigSized) ? 2 : 4;
				ByteArray byteArray = new ByteArray();
				byteArray.WriteBytes(this.pendingPacket.Buffer.Bytes, 0, count);
				byteArray.Position = 0;
				int num2 = (!this.pendingPacket.Header.BigSized) ? ((int)byteArray.ReadShort()) : byteArray.ReadInt();
				this.log.Debug(new string[]
				{
					"DataSize is ready: " + num2 + " bytes"
				});
				this.pendingPacket.Header.ExpectedLength = num2;
				this.pendingPacket.Buffer = new ByteArray();
				this.fsm.ApplyTransition(PacketReadTransition.WholeSizeReceived);
				if (data.Length > num)
				{
					data = this.ResizeByteArray(data, num, data.Length - num);
				}
				else
				{
					data = this.EMPTY_BUFFER;
				}
			}
			else
			{
				this.pendingPacket.Buffer.WriteBytes(data.Bytes);
				data = this.EMPTY_BUFFER;
			}
			return data;
		}
        private void HandleProtoBufPacketData(ProtoBuf.IExtensible message)
        {
            
            this.protocolCodec.OnPacketRead(message);

        }
		private ByteArray HandlePacketData(ByteArray data)
		{
			int num = this.pendingPacket.Header.ExpectedLength - this.pendingPacket.Buffer.Length;
			bool flag = data.Length > num;
			ByteArray byteArray = new ByteArray(data.Bytes);
			ByteArray result;
			try
			{
				this.log.Debug(new string[]
				{
					string.Concat(new object[]
					{
						"Handling Data: ",
						data.Length,
						", previous state: ",
						this.pendingPacket.Buffer.Length,
						"/",
						this.pendingPacket.Header.ExpectedLength
					})
				});
				if (data.Length >= num)
				{
					this.pendingPacket.Buffer.WriteBytes(data.Bytes, 0, num);
					this.log.Debug(new string[]
					{
						"<<< Packet Complete >>>"
					});
					if (this.pendingPacket.Header.Compressed)
					{
						this.pendingPacket.Buffer.Uncompress();
					}
					this.protocolCodec.OnPacketRead(this.pendingPacket.Buffer);
					this.fsm.ApplyTransition(PacketReadTransition.PacketFinished);
				}
				else
				{
					this.pendingPacket.Buffer.WriteBytes(data.Bytes);
				}
				if (flag)
				{
					data = this.ResizeByteArray(data, num, data.Length - num);
				}
				else
				{
					data = this.EMPTY_BUFFER;
				}
			}
			catch (Exception ex)
			{
				this.log.Error(new string[]
				{
					"Error handling data: " + ex.Message + " " + ex.StackTrace
				});
				this.skipBytes = num;
				this.fsm.ApplyTransition(PacketReadTransition.InvalidData);
				result = byteArray;
				return result;
			}
			result = data;
			return result;
		}
		private ByteArray HandleInvalidData(ByteArray data)
		{
			ByteArray result;
			if (this.skipBytes == 0)
			{
				this.fsm.ApplyTransition(PacketReadTransition.InvalidDataFinished);
				result = data;
			}
			else
			{
				int num = Math.Min(data.Length, this.skipBytes);
				data = this.ResizeByteArray(data, num, data.Length - num);
				this.skipBytes -= num;
				result = data;
			}
			return result;
		}
		private ByteArray ResizeByteArray(ByteArray array, int pos, int len)
		{
			byte[] array2 = new byte[len];
			Buffer.BlockCopy(array.Bytes, pos, array2, 0, len);
			return new ByteArray(array2);
		}
        //-----------------------------------------
        // the real send data callback function
		private void WriteBinaryData(PacketHeader header, ByteArray binData, bool udp)
		{
			ByteArray byteArray = new ByteArray();
			if (header.Compressed)
			{
				binData.Compress();
			}
			byteArray.WriteByte(header.Encode());
			if (header.BigSized)
			{
				byteArray.WriteInt(binData.Length);
			}
			else
			{
				byteArray.WriteUShort(Convert.ToUInt16(binData.Length));
			}
			byteArray.WriteBytes(binData.Bytes);
			if (this.bitSwarm.UseBlueBox)
			{
				this.bitSwarm.HttpClient.Send(byteArray);
			}
			else
			{
				if (this.bitSwarm.Socket.IsConnected)
				{
					if (udp)
					{
						this.WriteUDP(byteArray);
					}
					else
					{
						this.WriteTCP(byteArray);
					}
				}
			}
		}
        //---------------------------------------------
        //Write ProtoBuf to socket 2014-1-14 zxb
        private void WriteProtoBufBinaryData(ProtoBufPackageHeader header, ByteArray binData)
        {
            ByteArray byteArray = new ByteArray();
            byteArray.WriteBytes(binData.Bytes);
            header.WriteTo(byteArray.Bytes, 0);
            this.WriteTCP(byteArray);

        }
		public void OnDataWrite(IMessage message)
		{
            ByteArray byteArray = message.Content.ToBinary();
           	bool compressed = byteArray.Length > this.bitSwarm.CompressionThreshold;
			if (byteArray.Length > this.bitSwarm.MaxMessageSize)
			{
				throw new SFSCodecError(string.Concat(new object[]
				{
					"Message size is too big: ",
					byteArray.Length,
					", the server limit is: ",
					this.bitSwarm.MaxMessageSize
				}));
			}
			int num = SFSIOHandler.SHORT_BYTE_SIZE;
			if (byteArray.Length > 65535)
			{
				num = SFSIOHandler.INT_BYTE_SIZE;
			}
			bool useBlueBox = this.bitSwarm.UseBlueBox;
			PacketHeader header = new PacketHeader(message.IsEncrypted, compressed, useBlueBox, num == SFSIOHandler.INT_BYTE_SIZE);
			if (this.bitSwarm.Debug)
			{
				this.log.Info(new string[]
				{
					"Data written: " + message.Content.GetHexDump()
				});
			}
			this.bitSwarm.ThreadManager.EnqueueSend(new WriteBinaryDataDelegate(this.WriteBinaryData), header, byteArray, message.IsUDP);
		}
        //--------------------------------------------------
        //Serializer ProtoBufMsg and send 2014-1-14 zxbx
        public void OnProtoBufDataWrite(ProtoBuf.IExtensible message)
        {
            int iHeaderLen = 0;
		    MemoryStream stream = new MemoryStream();
            ProtoBufPackageHeader header = new ProtoBufPackageHeader();
            iHeaderLen = header.ReturnHeaderLen();
			byte[] headbuffer = new byte[iHeaderLen];
			stream.Write(headbuffer, 0, iHeaderLen);
			int messagename_length = this.protoBufserializer.Serialize(message, stream);

            header.MessageLength = (int)stream.Length;  //effective data len
		    header.MessageType = (byte)6;
		    header.MessageTypeLength = messagename_length;

			//ByteArray byteArray = new ByteArray(stream.GetBuffer());

			byte[] byteT = new byte[header.MessageLength];
			ByteArray byteArray = new ByteArray(byteT);
			ByteArray byteTcp = new ByteArray(stream.GetBuffer());

			Buffer.BlockCopy(byteTcp.Bytes, 0, byteArray.Bytes, 0, header.MessageLength);
   			//UnityEngine.Debug.Log("Message Buffer with empty header-->" + DefaultObjectDumpFormatter.HexDump(byteArray));
            
            this.bitSwarm.ThreadManager.EnqueueSend(new WriteProtoBufBinaryDataDelegate(this.WriteProtoBufBinaryData),header,byteArray);
        }
        //----------------------------------------------
        //Test data packet only header zxb 2014-1-20
        public void OnProtoBufDataWriteOnlyHeaderTest(ProtoBuf.IExtensible message)
        {
            int iHeaderLen = 0;
            MemoryStream stream = new MemoryStream();
            ProtoBufPackageHeader header = new ProtoBufPackageHeader();
            iHeaderLen = header.ReturnHeaderLen();
            byte[] headbuffer = new byte[iHeaderLen];
            stream.Write(headbuffer, 0, iHeaderLen);
           
            header.MessageLength = 47;
            header.MessageType = (byte)6;
            header.MessageTypeLength = 24;
            ByteArray byteArray = new ByteArray(stream.GetBuffer());
            this.bitSwarm.ThreadManager.EnqueueSend(new WriteProtoBufBinaryDataDelegate(this.WriteProtoBufBinaryData), header, byteArray);
        }
		private void WriteTCP(ByteArray writeBuffer)
		{
			this.bitSwarm.Socket.Write(writeBuffer.Bytes);
            
		}
		private void WriteUDP(ByteArray writeBuffer)
		{
			this.bitSwarm.UdpManager.Send(writeBuffer);
		}
	}
}
