using Sfs2X.Bitswarm;
using Sfs2X.Logging;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Sfs2X.Core.Sockets
{
	public class UDPSocketLayer : ISocketLayer
	{
		private Logger log;
		private BitSwarmClient bitSwarm;
		private int socketPollSleep;
		private int socketNumber;
		private IPAddress ipAddress;
		private UdpClient connection;
		private IPEndPoint sender;
		private volatile bool isDisconnecting = false;
		private Thread thrSocketReader;
		private byte[] byteBuffer;
		private bool connected = false;
		private OnDataDelegate onData = null;
		private OnErrorDelegate onError = null;
		public bool IsConnected
		{
			get
			{
				return this.connected;
			}
		}
		public OnDataDelegate OnData
		{
			get
			{
				return this.onData;
			}
			set
			{
				this.onData = value;
			}
		}
		public OnErrorDelegate OnError
		{
			get
			{
				return this.onError;
			}
			set
			{
				this.onError = value;
			}
		}
		public ConnectionDelegate OnConnect
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
		public ConnectionDelegate OnDisconnect
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
		public bool RequiresConnection
		{
			get
			{
				return false;
			}
		}
		public int SocketPollSleep
		{
			get
			{
				return this.socketPollSleep;
			}
			set
			{
				this.socketPollSleep = value;
			}
		}
		public UDPSocketLayer(SmartFox sfs)
		{
			if (sfs != null)
			{
				this.log = sfs.Log;
				this.bitSwarm = sfs.BitSwarm;
			}
		}
		private void LogWarn(string msg)
		{
			if (this.log != null)
			{
				this.log.Warn(new string[]
				{
					"UDPSocketLayer: " + msg
				});
			}
		}
		private void LogError(string msg)
		{
			if (this.log != null)
			{
				this.log.Error(new string[]
				{
					"UDPSocketLayer: " + msg
				});
			}
		}
		private void HandleError(string err, SocketError se)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["err"] = err;
			hashtable["se"] = se;
			this.bitSwarm.ThreadManager.EnqueueCustom(new ParameterizedThreadStart(this.HandleErrorCallback), hashtable);
		}
		private void HandleError(string err)
		{
			this.HandleError(err, SocketError.NotSocket);
		}
		private void HandleErrorCallback(object state)
		{
			Hashtable hashtable = state as Hashtable;
			string msg = (string)hashtable["err"];
			SocketError se = (SocketError)hashtable["se"];
			if (!this.isDisconnecting)
			{
				this.CloseConnection();
				this.LogError(msg);
				this.CallOnError(msg, se);
			}
		}
		private void WriteSocket(byte[] buf)
		{
			try
			{
				this.connection.Send(buf, buf.Length);
			}
			catch (SocketException ex)
			{
				string err = "Error writing to socket: " + ex.Message;
				this.HandleError(err, ex.SocketErrorCode);
			}
			catch (Exception ex2)
			{
				string err2 = "General error writing to socket: " + ex2.Message + " " + ex2.StackTrace;
				this.HandleError(err2);
			}
		}
		private static void Sleep(int ms)
		{
			Thread.Sleep(10);
		}
		private void Read()
		{
			this.connected = true;
			while (this.connected)
			{
				try
				{
					if (this.socketPollSleep > 0)
					{
						UDPSocketLayer.Sleep(this.socketPollSleep);
					}
					this.byteBuffer = this.connection.Receive(ref this.sender);
					if (this.byteBuffer != null && this.byteBuffer.Length != 0)
					{
						this.HandleBinaryData(this.byteBuffer);
					}
				}
				catch (SocketException ex)
				{
					this.HandleError("Error reading data from socket: " + ex.Message, ex.SocketErrorCode);
				}
				catch (ThreadAbortException)
				{
					break;
				}
				catch (Exception ex2)
				{
					this.HandleError("General error reading data from socket: " + ex2.Message + " " + ex2.StackTrace);
				}
			}
		}
		private void HandleBinaryData(byte[] buf)
		{
			this.CallOnData(buf);
		}
		public void Connect(IPAddress adr, int port)
		{
			this.socketNumber = port;
			this.ipAddress = adr;
			try
			{
				this.connection = new UdpClient(this.ipAddress.ToString(), this.socketNumber);
				this.sender = new IPEndPoint(IPAddress.Any, 0);
				this.thrSocketReader = new Thread(new ThreadStart(this.Read));
				this.thrSocketReader.Start();
			}
			catch (SocketException ex)
			{
				string err = "Connection error: " + ex.Message + " " + ex.StackTrace;
				this.HandleError(err, ex.SocketErrorCode);
			}
			catch (Exception ex2)
			{
				string err2 = "General exception on connection: " + ex2.Message + " " + ex2.StackTrace;
				this.HandleError(err2);
			}
		}
		public void Disconnect()
		{
			this.isDisconnecting = true;
			this.CloseConnection();
			this.isDisconnecting = false;
		}
		public void Disconnect(string reason)
		{
		}
		private void CloseConnection()
		{
			try
			{
				this.connection.Client.Shutdown(SocketShutdown.Both);
				this.connection.Close();
			}
			catch (Exception)
			{
			}
			this.connected = false;
		}
		public void Kill()
		{
			throw new NotSupportedException();
		}
		private void CallOnData(byte[] data)
		{
			if (this.onData != null)
			{
				this.bitSwarm.ThreadManager.EnqueueDataCall(this.onData, data);
			}
		}
		private void CallOnError(string msg, SocketError se)
		{
			if (this.onError != null)
			{
				this.onError(msg, se);
			}
		}
		public void Write(byte[] data)
		{
			this.WriteSocket(data);
		}
	}
}
