using Sfs2X.Bitswarm;
using Sfs2X.FSM;
using Sfs2X.Logging;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Sfs2X.Core.Sockets
{
	public class TCPSocketLayer : ISocketLayer
	{
		public enum States
		{
			Disconnected,
			Connecting,
			Connected
		}
		public enum Transitions
		{
			StartConnect,
			ConnectionSuccess,
			ConnectionFailure,
			Disconnect
		}
		private static readonly int READ_BUFFER_SIZE = 4096; // 4k? 如果超過4k怎麼辦
		private static int connId = 0;
		private Logger log;
		private BitSwarmClient bitSwarm;
		private FiniteStateMachine fsm;
		private volatile bool isDisconnecting = false;
		private int socketPollSleep;
		private Thread thrConnect;
		private int socketNumber;
		private IPAddress ipAddress;
		private TcpClient connection;
		private NetworkStream networkStream;
		private Thread thrSocketReader;
		private byte[] byteBuffer = new byte[TCPSocketLayer.READ_BUFFER_SIZE];
		private OnDataDelegate onData = null;
		private OnErrorDelegate onError = null;
		private ConnectionDelegate onConnect;
		private ConnectionDelegate onDisconnect;
		public TCPSocketLayer.States State
		{
			get
			{
				return (TCPSocketLayer.States)this.fsm.GetCurrentState();
			}
		}
		public bool IsConnected
		{
			get
			{
				return this.State == TCPSocketLayer.States.Connected;
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
				return this.onConnect;
			}
			set
			{
				this.onConnect = value;
			}
		}
		public ConnectionDelegate OnDisconnect
		{
			get
			{
				return this.onDisconnect;
			}
			set
			{
				this.onDisconnect = value;
			}
		}
		public bool RequiresConnection
		{
			get
			{
				return true;
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
		public TCPSocketLayer(BitSwarmClient bs)
		{
			this.log = bs.Log;
			this.bitSwarm = bs;
			this.InitStates();
		}
		private void InitStates()
		{
			this.fsm = new FiniteStateMachine();
			this.fsm.AddAllStates(typeof(TCPSocketLayer.States));
			this.fsm.AddStateTransition(TCPSocketLayer.States.Disconnected, TCPSocketLayer.States.Connecting, TCPSocketLayer.Transitions.StartConnect);
			this.fsm.AddStateTransition(TCPSocketLayer.States.Connecting, TCPSocketLayer.States.Connected, TCPSocketLayer.Transitions.ConnectionSuccess);
			this.fsm.AddStateTransition(TCPSocketLayer.States.Connecting, TCPSocketLayer.States.Disconnected, TCPSocketLayer.Transitions.ConnectionFailure);
			this.fsm.AddStateTransition(TCPSocketLayer.States.Connected, TCPSocketLayer.States.Disconnected, TCPSocketLayer.Transitions.Disconnect);
			this.fsm.SetCurrentState(TCPSocketLayer.States.Disconnected);
		}
		private void LogWarn(string msg)
		{
			if (this.log != null)
			{
				this.log.Warn(new string[]
				{
					"TCPSocketLayer: " + msg
				});
			}
		}
		private void LogError(string msg)
		{
			if (this.log != null)
			{
				this.log.Error(new string[]
				{
					"TCPSocketLayer: " + msg
				});
			}
		}
		private void ConnectThread()
		{
			Thread.CurrentThread.Name = "ConnectionThread" + TCPSocketLayer.connId++;
			try
			{
				this.connection = new TcpClient();
				this.connection.Client.Connect(this.ipAddress, this.socketNumber);
				this.networkStream = this.connection.GetStream();
				this.fsm.ApplyTransition(TCPSocketLayer.Transitions.ConnectionSuccess);
				this.CallOnConnect();
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
		private void HandleError(string err)
		{
			this.HandleError(err, SocketError.NotSocket);
		}
		private void HandleError(string err, SocketError se)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["err"] = err;
			hashtable["se"] = se;
			this.bitSwarm.ThreadManager.EnqueueCustom(new ParameterizedThreadStart(this.HandleErrorCallback), hashtable);
		}
		private void HandleErrorCallback(object state)
		{
			Hashtable hashtable = state as Hashtable;
			string msg = (string)hashtable["err"];
			SocketError se = (SocketError)hashtable["se"];
			this.fsm.ApplyTransition(TCPSocketLayer.Transitions.ConnectionFailure);
			if (!this.isDisconnecting)
			{
				this.LogError(msg);
				this.CallOnError(msg, se);
			}
			this.HandleDisconnection();
		}
		private void HandleDisconnection()
		{
			this.HandleDisconnection(null);
		}
		private void HandleDisconnection(string reason)
		{
			if (this.State != TCPSocketLayer.States.Disconnected)
			{
				this.fsm.ApplyTransition(TCPSocketLayer.Transitions.Disconnect);
				if (reason == null)
				{
					this.CallOnDisconnect();
				}
			}
		}
		private void WriteSocket(byte[] buf)
		{
			if (this.State != TCPSocketLayer.States.Connected)
			{
				this.LogError("Trying to write to disconnected socket");
			}
			else
			{
				try
				{
					this.networkStream.Write(buf, 0, buf.Length);
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
		}
		private static void Sleep(int ms)
		{
			Thread.Sleep(10);
		}
		private void Read()
		{
			while (true)
			{
				try
				{
					if (this.State != TCPSocketLayer.States.Connected)
					{
						break;
					}
					if (this.socketPollSleep > 0)
					{
						TCPSocketLayer.Sleep(this.socketPollSleep);
					}
					int num = this.networkStream.Read(this.byteBuffer, 0, TCPSocketLayer.READ_BUFFER_SIZE);
					if (num < 1)
					{
						this.HandleError("Connection closed by the remote side");
						break;
					}
					this.HandleBinaryData(this.byteBuffer, num);
				}
				catch (Exception ex)
				{
					this.HandleError("General error reading data from socket: " + ex.Message + " " + ex.StackTrace);
					break;
				}
			}
		}
		private void HandleBinaryData(byte[] buf, int size)
		{
			byte[] array = new byte[size];
			Buffer.BlockCopy(buf, 0, array, 0, size);
			this.CallOnData(array);
		}
		public void Connect(IPAddress adr, int port)
		{
			if (this.State != TCPSocketLayer.States.Disconnected)
			{
				this.LogWarn("Calling connect when the socket is not disconnected");
			}
			else
			{
				this.socketNumber = port;
				this.ipAddress = adr;
				this.fsm.ApplyTransition(TCPSocketLayer.Transitions.StartConnect);
				this.thrConnect = new Thread(new ThreadStart(this.ConnectThread));
				this.thrConnect.Start();
			}
		}
		public void Disconnect()
		{
			this.Disconnect(null);
		}
		public void Disconnect(string reason)
		{
			if (this.State != TCPSocketLayer.States.Connected)
			{
				this.LogWarn("Calling disconnect when the socket is not connected");
			}
			else
			{
				this.isDisconnecting = true;
				try
				{
					this.connection.Client.Shutdown(SocketShutdown.Both);
					this.connection.Close();
					this.networkStream.Close();
				}
				catch (Exception)
				{
					this.LogWarn(">>> Trying to disconnect a non-connected tcp socket");
				}
				this.HandleDisconnection(reason);
				this.isDisconnecting = false;
			}
		}
		public void Kill()
		{
			this.fsm.ApplyTransition(TCPSocketLayer.Transitions.Disconnect);
			this.connection.Close();
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
		private void CallOnConnect()
		{
			if (this.onConnect != null)
			{
				this.onConnect();
			}
		}
		private void CallOnDisconnect()
		{
			if (this.onDisconnect != null)
			{
				this.onDisconnect();
			}
		}
		public void Write(byte[] data)
		{
			this.WriteSocket(data);
		}
	}
}
