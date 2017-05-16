using KaiGeX.Bitswarm;
using KaiGeX.FSM;
using KaiGeX.Logging;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using KaiGeX.Util;
using KaiGeX.Protocol.Serialization;
namespace KaiGeX.Core.Sockets
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
		private static readonly int READ_BUFFER_SIZE = 4096;
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
        private DisConnectDelegate onDisconnect;
        private Thread protobufMessageRecvThread;

        //-----------------------------------
        //2014-1-16 add zxb
        private Socket clientSocket;
        private OnDataProtoBufDelegate onDataProtoBuf = null;

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
                return this.State == TCPSocketLayer.States.Connected; //注意返回的是一个比较后的结果 不是返回的TCPSocketLayer.States.Connected
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
        public OnDataProtoBufDelegate OnProtoBufData
        {
            get
            {
                return this.onDataProtoBuf;
            }
            set
            {
                this.onDataProtoBuf = value;
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
        public DisConnectDelegate OnDisconnect
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
			int msglength = 0;
            int msgNameLen = 0;
       
            while (true)
            {
                try
                {

                    if (this.State != TCPSocketLayer.States.Connected)
                    {
                        UnityEngine.Debug.Log("Recive Thread DisConnect-->");
                        break;
                    }

                    if (this.socketPollSleep > 0)
                    {

                        TCPSocketLayer.Sleep(this.socketPollSleep);
                    }
                    if (this.connection.Client.Poll( -1 ,SelectMode.SelectRead) || this.connection.Client.Poll(-1, SelectMode.SelectError))
                    {
                        byte[] headerBytes = new byte[9];
                        int i = this.connection.Client.Receive(headerBytes);
                        if (i <= 0)
                        {
                            this.Disconnect();
                            break;
                        }
        
                        ProtoBufPackageHeader header = new ProtoBufPackageHeader();
                        int iHeaderLen = header.ReturnHeaderLen();
                        header.ReadFrom(headerBytes, 0);
                        msglength = header.MessageLength - iHeaderLen;
                        msgNameLen = header.MessageTypeLength;
                        if (0 == msglength)
                        {
                            ResponsePing();
                            continue;
                        }
                    }
                    if (this.connection.Client.Poll(-1, SelectMode.SelectRead) || this.connection.Client.Poll(-1, SelectMode.SelectError))
                    {
                        byte[] msgbytes = new byte[msglength];
                        int i = this.connection.Client.Receive(msgbytes);
                        if (i <= 0)
                        {
                            this.Disconnect();
                            break;
                        }
                        ByteArray byteArray = new ByteArray();
						byteArray.WriteBytes(msgbytes);
                        UnityEngine.Debug.Log("TCP Socket 收到的字节:  " + DefaultObjectDumpFormatter.HexDump(byteArray));
                        this.HandleBinaryProtoBufData(byteArray.Bytes, byteArray.Bytes.Length, msgNameLen);
                    }

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
        private void HandleBinaryProtoBufData(byte[] buf, int size, int msgTypeLen)
        {
			byte[] array = new byte[size];
			Buffer.BlockCopy(buf, 0, array, 0, size);
			this.CallOnProtoBufData(array, msgTypeLen);
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
               // this.ConnectServer(adr, port);
			}
		}
        //连接线程函数
        //连接的时候为何用线程呢? 是为了不想阻塞等待吗?
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
        private void CallOnProtoBufData(byte[] data, int msgTypeLen)
        {
            if (this.onDataProtoBuf != null)
            {
                this.bitSwarm.ThreadManager.EnqueueDataCall(this.onDataProtoBuf, data, msgTypeLen);
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
            //this.WriteSocket(data, 0, data.Length);
		}
        //----------------Begin-------------------------
        //为了与当前的游戏服务器协议匹配 重写sock
        //zxb 2014-1-17
        public void ConnectServer(IPAddress serverIPAddress, int serverPort)
        {
            if (this.clientSocket != null)
            {
                //----------------
                //关闭连接
            }
            this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEndPoint = new IPEndPoint(serverIPAddress, serverPort);
            IAsyncResult ansyncResult = this.clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(connectCallback), this.clientSocket);
            /*
            bool connectSuccess = ansyncResult.AsyncWaitHandle.WaitOne(10000, true);
            if (!connectSuccess)
            {
                //连接超时 关闭连接
                UnityEngine.Debug.Log("<------------connectSuccess-->flase-------------->");
                this.clientSocket.Close();
            }
             * */

            //-----------------------------
            //连接成功 开启接收数据线程
            if (protobufMessageRecvThread == null)
            {
                protobufMessageRecvThread = new Thread(new ThreadStart(ReceiveProtobufMessageThread));
                protobufMessageRecvThread.IsBackground = true;
                protobufMessageRecvThread.Start();
            }
            

        }
        private void ReceiveProtobufMessageThread()
        {
            while (true)
            {
                if (!this.clientSocket.Connected)
                {
                    //与服务器断开连接跳出循环
                    //Console.WriteLine("Failed to clientSocket server.");
                    UnityEngine.Debug.Log("<-----------this.clientSocket.Connected-->flase------------------>");
                    this.clientSocket.Close();
                    break;
                }
                try
                {
                    int msglength = 0;
                    int msgnamelength = 0;
                    if (this.clientSocket.Poll(-1, SelectMode.SelectRead) || this.clientSocket.Poll(-1, SelectMode.SelectError))
                    {
                        if (!this.clientSocket.Connected)
                        {
                            //与服务器断开连接跳出循环
                           // Console.WriteLine("Failed to clientSocket server.");
                            this.clientSocket.Close();
                    
                            break;
                        }
                        //接受数据保存至bytes当中
                        byte[] headerBytes = new byte[9];
                        //Receive方法中会一直等待服务端回发消息
                        //如果没有回发会一直在这里等着。
                        int i = this.clientSocket.Receive(headerBytes);
                        if (i <= 0)
                        {
                            clientSocket.Close();
                            break;
                        }
                        ProtoBufPackageHeader header = new ProtoBufPackageHeader();
                        int iHeaderLen = header.ReturnHeaderLen();
                        header.ReadFrom(headerBytes, 0);
                        msglength = header.MessageLength - iHeaderLen;
                        msgnamelength = header.MessageTypeLength;
                        if (0 == msglength)
                        {
                            ResponsePing();
                            continue;
                        }
                    }
                    if (this.clientSocket.Poll(-1, SelectMode.SelectRead) || this.clientSocket.Poll(-1, SelectMode.SelectError))
                    {
                        byte[] msgbytes = new byte[msglength];
                        int i = clientSocket.Receive(msgbytes);
                        if (i <= 0)
                        {
                            clientSocket.Close();
                            break;
                        }
                        PrintRecvHexData(msgbytes);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to clientSocket error" + e.ToString());
                    clientSocket.Close();
                    break;
                }
            }
        }

        public void ResponsePing()
        {
            byte[] pingmsg = { 9, 0, 0, 0, 5, 0, 0, 0, 0 };
            //WriteSocket(pingmsg, 0, pingmsg.Length);
            this.WriteSocket(pingmsg);
        }
        //向服务端发送一条字符串
        //一般不会发送字符串 应该是发送数据包
        public void WriteSocket(byte[] msg, int offset, int size)
        {
            try
            {
                lock (this)
                {
                    IAsyncResult asyncSend = this.clientSocket.BeginSend(msg, offset, size, SocketFlags.None, new AsyncCallback(sendCallback), this.clientSocket);
                    bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
                    if (!success)
                    {
                        clientSocket.Close();
                        //Console.WriteLine("Failed to SendMessage server.");
                    }
                }
            }
            catch
            {
                clientSocket.Close();
                Console.WriteLine("send message error");
            }
        }

        //发送成功回调
        private void sendCallback(IAsyncResult asyncSend)
        {
            Console.WriteLine("Send msg success" + asyncSend.ToString());
        }
        private void connectCallback(IAsyncResult asyncConnect)
        {
            Console.WriteLine("connectSuccess");
        }
        private void PrintRecvHexData(byte[] msg)
        {
            ByteArray byteArray = new ByteArray(msg);
            UnityEngine.Debug.Log("<----------------TcpSocketLayer--begin--------------->");
            UnityEngine.Debug.Log("Message Buffer-->" + DefaultObjectDumpFormatter.HexDump(byteArray));
            UnityEngine.Debug.Log("<----------------TcpSocketLayer--end----------------->");

        }

	}
}
