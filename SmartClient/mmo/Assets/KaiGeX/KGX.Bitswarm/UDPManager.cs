using KaiGeX.Core;
using KaiGeX.Core.Sockets;
using KaiGeX.Entities.Data;
using KaiGeX.Logging;
using KaiGeX.Protocol.Serialization;
using KaiGeX.Util;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace KaiGeX.Bitswarm
{
	public class UDPManager : IUDPManager
	{
		private KaiGeNet sfs;
		private long packetId;
		private ISocketLayer udpSocket;
		private Logger log;
		private bool locked = false;
		private bool initSuccess = false;
		private readonly int MAX_RETRY = 3;
		private readonly int RESPONSE_TIMEOUT = 3000;
		private Timer initThread;
		private int currentAttempt;
		private object initThreadLocker = new object();
		public long NextUdpPacketId
		{
			get
			{
				long result;
				this.packetId = (result = this.packetId) + 1L;
				return result;
			}
		}
		public bool Inited
		{
			get
			{
				return this.initSuccess;
			}
		}
		public UDPManager(KaiGeNet sfs)
		{
			this.sfs = sfs;
			this.packetId = 0L;
			if (sfs != null)
			{
				this.log = sfs.Log;
			}
			else
			{
				this.log = new Logger(null);
			}
			this.currentAttempt = 1;
		}
		public void Initialize(string udpAddr, int udpPort)
		{
			if (this.initSuccess)
			{
				this.log.Warn(new string[]
				{
					"UDP Channel already initialized!"
				});
			}
			else
			{
				if (!this.locked)
				{
					this.locked = true;
					this.udpSocket = new UDPSocketLayer(this.sfs);
					this.udpSocket.OnData = new OnDataDelegate(this.OnUDPData);
					this.udpSocket.OnError = new OnErrorDelegate(this.OnUDPError);
					IPAddress adr = IPAddress.Parse(udpAddr);
					this.udpSocket.Connect(adr, udpPort);
					this.SendInitializationRequest();
				}
				else
				{
					this.log.Warn(new string[]
					{
						"UPD initialization is already in progress!"
					});
				}
			}
		}
		public void Send(ByteArray binaryData)
		{
			if (this.initSuccess)
			{
				try
				{
					this.udpSocket.Write(binaryData.Bytes);
					if (this.sfs.Debug)
					{
						this.log.Info(new string[]
						{
							"UDP Data written: " + DefaultObjectDumpFormatter.HexDump(binaryData)
						});
					}
				}
				catch (Exception ex)
				{
					this.log.Warn(new string[]
					{
						"WriteUDP operation failed due to Error: " + ex.Message + " " + ex.StackTrace
					});
				}
			}
			else
			{
				this.log.Warn(new string[]
				{
					"UDP protocol is not initialized yet. Pleas use the initUDP() method."
				});
			}
		}
		public bool isConnected()
		{
			return this.udpSocket.IsConnected;
		}
		public void Reset()
		{
			this.StopTimer();
			this.currentAttempt = 1;
			this.initSuccess = false;
			this.locked = false;
			this.packetId = 0L;
		}
		private void OnUDPData(byte[] bt)
		{
			ByteArray byteArray = new ByteArray(bt);
			if (byteArray.BytesAvailable < 4)
			{
				this.log.Warn(new string[]
				{
					"Too small UDP packet. Len: " + byteArray.Length
				});
			}
			else
			{
				if (this.sfs.Debug)
				{
					this.log.Info(new string[]
					{
						"UDP Data Read: " + DefaultObjectDumpFormatter.HexDump(byteArray)
					});
				}
				byte b = byteArray.ReadByte();
				bool flag = (b & 32) > 0;
				short num = byteArray.ReadShort();
				if ((int)num != byteArray.BytesAvailable)
				{
					this.log.Warn(new string[]
					{
						string.Concat(new object[]
						{
							"Insufficient UDP data. Expected: ",
							num,
							", got: ",
							byteArray.BytesAvailable
						})
					});
				}
				else
				{
					byte[] buf = byteArray.ReadBytes(byteArray.BytesAvailable);
					ByteArray byteArray2 = new ByteArray(buf);
					if (flag)
					{
						byteArray2.Uncompress();
					}
					ISFSObject iSFSObject = SFSObject.NewFromBinaryData(byteArray2);
					if (iSFSObject.ContainsKey("h"))
					{
						if (!this.initSuccess)
						{
							this.StopTimer();
							this.locked = false;
							this.initSuccess = true;
							Hashtable hashtable = new Hashtable();
							hashtable["success"] = true;
							this.sfs.DispatchEvent(new SFSEvent(SFSEvent.UDP_INIT, hashtable));
						}
					}
					else
					{
						this.sfs.GetSocketEngine().IoHandler.Codec.OnPacketRead(iSFSObject);
					}
				}
			}
		}
		private void OnUDPError(string error, SocketError se)
		{
			this.log.Warn(new string[]
			{
				string.Concat(new string[]
				{
					"Unexpected UDP I/O Error. ",
					error,
					" [",
					se.ToString(),
					"]"
				})
			});
		}
		private void SendInitializationRequest()
		{
			ISFSObject iSFSObject = new SFSObject();
			iSFSObject.PutByte("c", 1);
			iSFSObject.PutByte("h", 1);
			iSFSObject.PutLong("i", this.NextUdpPacketId);
			iSFSObject.PutInt("u", this.sfs.MySelf.Id);
			ByteArray byteArray = iSFSObject.ToBinary();
			ByteArray byteArray2 = new ByteArray();
			byteArray2.WriteByte(128);
			byteArray2.WriteShort(Convert.ToInt16(byteArray.Length));
			byteArray2.WriteBytes(byteArray.Bytes);
			this.udpSocket.Write(byteArray2.Bytes);
			this.StartTimer();
		}
		private void OnTimeout(object state)
		{
			if (!this.initSuccess)
			{
				object obj = this.initThreadLocker;
				Monitor.Enter(obj);
				try
				{
					if (this.initThread == null)
					{
						return;
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
				if (this.currentAttempt < this.MAX_RETRY)
				{
					this.currentAttempt++;
					this.log.Debug(new string[]
					{
						"UDP Init Attempt: " + this.currentAttempt
					});
					this.SendInitializationRequest();
					this.StartTimer();
				}
				else
				{
					this.currentAttempt = 0;
					this.locked = false;
					Hashtable hashtable = new Hashtable();
					hashtable["success"] = false;
					this.sfs.DispatchEvent(new SFSEvent(SFSEvent.UDP_INIT, hashtable));
				}
			}
		}
		private void StartTimer()
		{
			if (this.initThread != null)
			{
				this.initThread.Dispose();
			}
			this.initThread = new Timer(new TimerCallback(this.OnTimeout), null, this.RESPONSE_TIMEOUT, -1);
		}
		private void StopTimer()
		{
			object obj = this.initThreadLocker;
			Monitor.Enter(obj);
			try
			{
				if (this.initThread != null)
				{
					this.initThread.Dispose();
					this.initThread = null;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public void Disconnect()
		{
			this.udpSocket.Disconnect();
			this.Reset();
		}
	}
}
