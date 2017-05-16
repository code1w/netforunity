using Sfs2X.Bitswarm.BBox;
using Sfs2X.Controllers;
using Sfs2X.Core;
using Sfs2X.Core.Sockets;
using Sfs2X.Exceptions;
using Sfs2X.Logging;
using Sfs2X.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Timers;
namespace Sfs2X.Bitswarm
{
	public class BitSwarmClient : IDispatchable
	{
		private readonly double reconnectionDelayMillis = 1000.0;
		private ISocketLayer socket = null;
		private IoHandler ioHandler;
		private Dictionary<int, IController> controllers = new Dictionary<int, IController>();
		private int compressionThreshold = 2000000;
		private int maxMessageSize = 10000;
		private SmartFox sfs;
		private string lastIpAddress;
		private int lastTcpPort;
		private Logger log;
		private int reconnectionSeconds = 0;
		private bool attemptingReconnection = false;
		private DateTime firstReconnAttempt = DateTime.MinValue;
		private int reconnCounter = 1;
		private SystemController sysController;
		private ExtensionController extController;
		private IUDPManager udpManager;
		private bool controllersInited = false;
		private EventDispatcher dispatcher;
		private BBClient bbClient;
		private volatile bool useBlueBox = false;
		private bool bbConnected = false;
		private string connectionMode;
		private ThreadManager threadManager = new ThreadManager();
		private bool manualDisconnection = false;
		private Timer retryTimer = null;
		public ThreadManager ThreadManager
		{
			get
			{
				return this.threadManager;
			}
		}
		public string ConnectionMode
		{
			get
			{
				return this.connectionMode;
			}
		}
		public bool UseBlueBox
		{
			get
			{
				return this.useBlueBox;
			}
		}
		public bool Debug
		{
			get
			{
				return this.sfs == null || this.sfs.Debug;
			}
		}
		public SmartFox Sfs
		{
			get
			{
				return this.sfs;
			}
		}
		public bool Connected
		{
			get
			{
				bool result;
				if (this.useBlueBox)
				{
					result = this.bbConnected;
				}
				else
				{
					result = (this.socket != null && this.socket.IsConnected);
				}
				return result;
			}
		}
		public IoHandler IoHandler
		{
			get
			{
				return this.ioHandler;
			}
			set
			{
				if (this.ioHandler != null)
				{
					throw new SFSError("IOHandler is already set!");
				}
				this.ioHandler = value;
			}
		}
		public int CompressionThreshold
		{
			get
			{
				return this.compressionThreshold;
			}
			set
			{
				if (value > 100)
				{
					this.compressionThreshold = value;
					return;
				}
				throw new ArgumentException("Compression threshold cannot be < 100 bytes.");
			}
		}
		public int MaxMessageSize
		{
			get
			{
				return this.maxMessageSize;
			}
			set
			{
				this.maxMessageSize = value;
			}
		}
		public SystemController SysController
		{
			get
			{
				return this.sysController;
			}
		}
		public ExtensionController ExtController
		{
			get
			{
				return this.extController;
			}
		}
		public ISocketLayer Socket
		{
			get
			{
				return this.socket;
			}
		}
		public BBClient HttpClient
		{
			get
			{
				return this.bbClient;
			}
		}
		public bool IsReconnecting
		{
			get
			{
				return this.attemptingReconnection;
			}
			set
			{
				this.attemptingReconnection = value;
			}
		}
		public int ReconnectionSeconds
		{
			get
			{
				int result;
				if (this.reconnectionSeconds < 0)
				{
					result = 0;
				}
				else
				{
					result = this.reconnectionSeconds;
				}
				return result;
			}
			set
			{
				this.reconnectionSeconds = value;
			}
		}
		public EventDispatcher Dispatcher
		{
			get
			{
				return this.dispatcher;
			}
			set
			{
				this.dispatcher = value;
			}
		}
		public Logger Log
		{
			get
			{
				Logger result;
				if (this.sfs == null)
				{
					result = new Logger(null);
				}
				else
				{
					result = this.sfs.Log;
				}
				return result;
			}
		}
		public string ConnectionIp
		{
			get
			{
				string result;
				if (!this.Connected)
				{
					result = "Not Connected";
				}
				else
				{
					result = this.lastIpAddress;
				}
				return result;
			}
		}
		public int ConnectionPort
		{
			get
			{
				int result;
				if (!this.Connected)
				{
					result = -1;
				}
				else
				{
					result = this.lastTcpPort;
				}
				return result;
			}
		}
		public IUDPManager UdpManager
		{
			get
			{
				return this.udpManager;
			}
			set
			{
				this.udpManager = value;
			}
		}
		public BitSwarmClient()
		{
			this.sfs = null;
			this.log = null;
		}
		public BitSwarmClient(SmartFox sfs)
		{
			this.sfs = sfs;
			this.log = sfs.Log;
		}
		public void ForceBlueBox(bool val)
		{
			if (!this.bbConnected)
			{
				this.useBlueBox = val;
				return;
			}
			throw new Exception("You can't change the BlueBox mode while the connection is running");
		}
		public void EnableBlueBoxDebug(bool val)
		{
			this.bbClient.IsDebug = val;
		}
		public void Init()
		{
			if (this.dispatcher == null)
			{
				this.dispatcher = new EventDispatcher(this);
			}
			if (!this.controllersInited)
			{
				this.InitControllers();
				this.controllersInited = true;
			}
			if (this.socket == null)
			{
				this.socket = new TCPSocketLayer(this);
				ISocketLayer expr_56 = this.socket;
				expr_56.OnConnect = (ConnectionDelegate)Delegate.Combine(expr_56.OnConnect, new ConnectionDelegate(this.OnSocketConnect));
				ISocketLayer expr_7D = this.socket;
				expr_7D.OnDisconnect = (ConnectionDelegate)Delegate.Combine(expr_7D.OnDisconnect, new ConnectionDelegate(this.OnSocketClose));
				ISocketLayer expr_A4 = this.socket;
				expr_A4.OnData = (OnDataDelegate)Delegate.Combine(expr_A4.OnData, new OnDataDelegate(this.OnSocketData));
				ISocketLayer expr_CB = this.socket;
				expr_CB.OnError = (OnErrorDelegate)Delegate.Combine(expr_CB.OnError, new OnErrorDelegate(this.OnSocketError));
				this.bbClient = new BBClient(this);
				this.bbClient.AddEventListener(BBEvent.CONNECT, new EventListenerDelegate(this.OnBBConnect));
				this.bbClient.AddEventListener(BBEvent.DATA, new EventListenerDelegate(this.OnBBData));
				this.bbClient.AddEventListener(BBEvent.DISCONNECT, new EventListenerDelegate(this.OnBBDisconnect));
				this.bbClient.AddEventListener(BBEvent.IO_ERROR, new EventListenerDelegate(this.OnBBError));
				this.bbClient.AddEventListener(BBEvent.SECURITY_ERROR, new EventListenerDelegate(this.OnBBError));
			}
		}
		public void Destroy()
		{
			ISocketLayer expr_07 = this.socket;
			expr_07.OnConnect = (ConnectionDelegate)Delegate.Remove(expr_07.OnConnect, new ConnectionDelegate(this.OnSocketConnect));
			ISocketLayer expr_2E = this.socket;
			expr_2E.OnDisconnect = (ConnectionDelegate)Delegate.Remove(expr_2E.OnDisconnect, new ConnectionDelegate(this.OnSocketClose));
			ISocketLayer expr_55 = this.socket;
			expr_55.OnData = (OnDataDelegate)Delegate.Remove(expr_55.OnData, new OnDataDelegate(this.OnSocketData));
			ISocketLayer expr_7C = this.socket;
			expr_7C.OnError = (OnErrorDelegate)Delegate.Remove(expr_7C.OnError, new OnErrorDelegate(this.OnSocketError));
			if (this.socket.IsConnected)
			{
				this.socket.Disconnect();
			}
			this.socket = null;
			this.threadManager.Stop();
		}
		public IController GetController(int id)
		{
			return this.controllers[id];
		}
		private void AddController(int id, IController controller)
		{
			if (controller == null)
			{
				throw new ArgumentException("Controller is null, it can't be added.");
			}
			if (this.controllers.ContainsKey(id))
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"A controller with id: ",
					id,
					" already exists! Controller can't be added: ",
					controller
				}));
			}
			this.controllers[id] = controller;
		}
		private void AddCustomController(int id, Type controllerType)
		{
			IController controller = Activator.CreateInstance(controllerType) as IController;
			this.AddController(id, controller);
		}
		public void Connect()
		{
			this.Connect("127.0.0.1", 9339);
		}
		public void Connect(string ip, int port)
		{
			this.lastIpAddress = ip;
			this.lastTcpPort = port;
			this.threadManager.Start();
			if (this.useBlueBox)
			{
				this.connectionMode = ConnectionModes.HTTP;
				this.bbClient.PollSpeed = ((this.sfs.Config == null) ? 750 : this.sfs.Config.BlueBoxPollingRate);
				this.bbClient.Connect(ip, port);
			}
			else
			{
				this.socket.Connect(IPAddress.Parse(this.lastIpAddress), this.lastTcpPort);
				this.connectionMode = ConnectionModes.SOCKET;
			}
		}
		public void Send(IMessage message)
		{
			this.ioHandler.Codec.OnPacketWrite(message);
		}
		public void Disconnect()
		{
			this.Disconnect(null);
		}
		public void Disconnect(string reason)
		{
			if (this.useBlueBox)
			{
				this.bbClient.Close();
			}
			else
			{
				this.socket.Disconnect(reason);
				if (this.udpManager != null)
				{
					this.udpManager.Disconnect();
				}
			}
			this.ReleaseResources();
		}
		private void InitControllers()
		{
			this.sysController = new SystemController(this);
			this.extController = new ExtensionController(this);
			this.AddController(0, this.sysController);
			this.AddController(1, this.extController);
		}
		private void OnSocketConnect()
		{
			BitSwarmEvent bitSwarmEvent = new BitSwarmEvent(BitSwarmEvent.CONNECT);
			Hashtable hashtable = new Hashtable();
			hashtable["success"] = true;
			hashtable["isReconnection"] = this.attemptingReconnection;
			bitSwarmEvent.Params = hashtable;
			this.DispatchEvent(bitSwarmEvent);
		}
		public void StopReconnection()
		{
			this.attemptingReconnection = false;
			this.firstReconnAttempt = DateTime.MinValue;
			if (this.socket.IsConnected)
			{
				this.socket.Disconnect();
			}
			this.ExecuteDisconnection();
		}
		private void OnSocketClose()
		{
			bool flag = this.sfs.GetReconnectionSeconds() == 0;
			if (flag)
			{
				this.firstReconnAttempt = DateTime.MinValue;
				this.ExecuteDisconnection();
			}
			else
			{
				if (this.attemptingReconnection)
				{
					this.Reconnect();
				}
				else
				{
					this.attemptingReconnection = true;
					this.firstReconnAttempt = DateTime.Now;
					this.reconnCounter = 1;
					this.DispatchEvent(new BitSwarmEvent(BitSwarmEvent.RECONNECTION_TRY));
					this.Reconnect();
				}
			}
		}
		private void SetTimeout(ElapsedEventHandler handler, double timeout)
		{
			if (this.retryTimer == null)
			{
				this.retryTimer = new Timer(timeout);
				this.retryTimer.Elapsed += handler;
			}
			this.retryTimer.AutoReset = false;
			this.retryTimer.Enabled = true;
			this.retryTimer.Start();
		}
		private void OnRetryConnectionEvent(object source, ElapsedEventArgs e)
		{
			this.retryTimer.Enabled = false;
			this.retryTimer.Stop();
			this.socket.Connect(IPAddress.Parse(this.lastIpAddress), this.lastTcpPort);
		}
		private void Reconnect()
		{
			if (this.attemptingReconnection)
			{
				int seconds = this.sfs.GetReconnectionSeconds();
				DateTime now = DateTime.Now;
				TimeSpan t = this.firstReconnAttempt + new TimeSpan(0, 0, seconds) - now;
				if (t > TimeSpan.Zero)
				{
					this.log.Info(new string[]
					{
						string.Concat(new object[]
						{
							"Reconnection attempt: ",
							this.reconnCounter,
							" - time left:",
							t.TotalSeconds,
							" sec."
						})
					});
					this.SetTimeout(new ElapsedEventHandler(this.OnRetryConnectionEvent), this.reconnectionDelayMillis);
					this.reconnCounter++;
				}
				else
				{
					this.ExecuteDisconnection();
				}
			}
		}
		private void ExecuteDisconnection()
		{
			Hashtable hashtable = new Hashtable();
			hashtable["reason"] = ClientDisconnectionReason.UNKNOWN;
			this.DispatchEvent(new BitSwarmEvent(BitSwarmEvent.DISCONNECT, hashtable));
			this.ReleaseResources();
		}
		private void ReleaseResources()
		{
			this.threadManager.Stop();
			if (this.udpManager != null && this.udpManager.Inited)
			{
				this.udpManager.Disconnect();
			}
		}
		private void OnSocketData(byte[] data)
		{
			try
			{
				ByteArray buffer = new ByteArray(data);
				this.ioHandler.OnDataRead(buffer);
			}
			catch (Exception ex)
			{
				this.log.Error(new string[]
				{
					"## SocketDataError: " + ex.Message
				});
				BitSwarmEvent bitSwarmEvent = new BitSwarmEvent(BitSwarmEvent.DATA_ERROR);
				Hashtable hashtable = new Hashtable();
				hashtable["message"] = ex.ToString();
				bitSwarmEvent.Params = hashtable;
				this.DispatchEvent(bitSwarmEvent);
			}
		}
		private void OnSocketError(string message, SocketError se)
		{
			this.manualDisconnection = false;
			if (this.attemptingReconnection)
			{
				this.Reconnect();
			}
			else
			{
				BitSwarmEvent bitSwarmEvent = new BitSwarmEvent(BitSwarmEvent.IO_ERROR);
				bitSwarmEvent.Params = new Hashtable();
				bitSwarmEvent.Params["message"] = message + " ==> " + se.ToString();
				this.DispatchEvent(bitSwarmEvent);
			}
		}
		public void KillConnection()
		{
			if (!this.useBlueBox)
			{
				this.socket.Kill();
				this.OnSocketClose();
			}
		}
		public long NextUdpPacketId()
		{
			return this.udpManager.NextUdpPacketId;
		}
		public void AddEventListener(string eventType, EventListenerDelegate listener)
		{
			this.dispatcher.AddEventListener(eventType, listener);
		}
		private void DispatchEvent(BitSwarmEvent evt)
		{
			this.dispatcher.DispatchEvent(evt);
		}
		private void OnBBConnect(BaseEvent e)
		{
			this.bbConnected = true;
			BitSwarmEvent bitSwarmEvent = new BitSwarmEvent(BitSwarmEvent.CONNECT);
			bitSwarmEvent.Params = new Hashtable();
			bitSwarmEvent.Params["success"] = true;
			bitSwarmEvent.Params["isReconnection"] = false;
			this.DispatchEvent(bitSwarmEvent);
		}
		private void OnBBData(BaseEvent e)
		{
			BBEvent bBEvent = e as BBEvent;
			ByteArray buffer = (ByteArray)bBEvent.Params["data"];
			this.ioHandler.OnDataRead(buffer);
		}
		private void OnBBDisconnect(BaseEvent e)
		{
			this.bbConnected = false;
			this.useBlueBox = false;
			if (this.manualDisconnection)
			{
				this.manualDisconnection = false;
				this.ExecuteDisconnection();
			}
		}
		private void OnBBError(BaseEvent e)
		{
			BBEvent bBEvent = e as BBEvent;
			this.log.Error(new string[]
			{
				"## BlueBox Error: " + (string)bBEvent.Params["message"]
			});
			BitSwarmEvent bitSwarmEvent = new BitSwarmEvent(BitSwarmEvent.IO_ERROR);
			bitSwarmEvent.Params = new Hashtable();
			bitSwarmEvent.Params["message"] = bBEvent.Params["message"];
			this.DispatchEvent(bitSwarmEvent);
		}
	}
}
