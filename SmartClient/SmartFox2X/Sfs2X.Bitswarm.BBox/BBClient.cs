using Sfs2X.Core;
using Sfs2X.Http;
using Sfs2X.Logging;
using Sfs2X.Util;
using System;
using System.Collections;
using System.Threading;
namespace Sfs2X.Bitswarm.BBox
{
	public class BBClient : IDispatchable
	{
		public const string BB_SERVLET = "BlueBox/BlueBox.do";
		private const string BB_DEFAULT_HOST = "localhost";
		private const int BB_DEFAULT_PORT = 8080;
		private const string BB_NULL = "null";
		private const string CMD_CONNECT = "connect";
		private const string CMD_POLL = "poll";
		private const string CMD_DATA = "data";
		private const string CMD_DISCONNECT = "disconnect";
		private const string ERR_INVALID_SESSION = "err01";
		private const string SFS_HTTP = "sfsHttp";
		private const char SEP = '|';
		private const int MIN_POLL_SPEED = 50;
		private const int MAX_POLL_SPEED = 5000;
		private const int DEFAULT_POLL_SPEED = 300;
		private bool isConnected = false;
		private string host = "localhost";
		private int port = 8080;
		private string bbUrl;
		private bool debug;
		private string sessId;
		private int pollSpeed = 300;
		private EventDispatcher dispatcher;
		private Logger log;
		private Timer pollTimer = null;
		public bool IsConnected
		{
			get
			{
				return this.sessId != null;
			}
		}
		public bool IsDebug
		{
			get
			{
				return this.debug;
			}
			set
			{
				this.debug = value;
			}
		}
		public string Host
		{
			get
			{
				return this.host;
			}
		}
		public int Port
		{
			get
			{
				return this.port;
			}
		}
		public string SessionId
		{
			get
			{
				return this.sessId;
			}
		}
		public int PollSpeed
		{
			get
			{
				return this.pollSpeed;
			}
			set
			{
				this.pollSpeed = ((value < 50 || value > 5000) ? 300 : value);
			}
		}
		public EventDispatcher Dispatcher
		{
			get
			{
				return this.dispatcher;
			}
		}
		public BBClient(BitSwarmClient bs)
		{
			this.debug = bs.Debug;
			this.log = bs.Log;
			if (this.dispatcher == null)
			{
				this.dispatcher = new EventDispatcher(this);
			}
		}
		public void Connect(string host, int port)
		{
			if (this.isConnected)
			{
				throw new Exception("BlueBox session is already connected");
			}
			this.host = host;
			this.port = port;
			this.bbUrl = string.Concat(new object[]
			{
				"http://",
				host,
				":",
				port,
				"/BlueBox/BlueBox.do"
			});
			if (this.debug)
			{
				this.log.Debug(new string[]
				{
					"[ BB-Connect ]: " + this.bbUrl
				});
			}
			this.SendRequest("connect");
		}
		public void Send(ByteArray binData)
		{
			if (!this.isConnected)
			{
				throw new Exception("Can't send data, BlueBox connection is not active");
			}
			this.SendRequest("data", binData);
		}
		public void Close()
		{
			this.HandleConnectionLost(true);
		}
		private void OnHttpResponse(bool error, string response)
		{
			if (error)
			{
				Hashtable hashtable = new Hashtable();
				hashtable["message"] = response;
				this.HandleConnectionLost(true);
				this.DispatchEvent(new BBEvent(BBEvent.IO_ERROR, hashtable));
			}
			else
			{
				try
				{
					if (this.debug)
					{
						this.log.Debug(new string[]
						{
							"[ BB-Receive ]: " + response.ToString()
						});
					}
					string[] array = response.Split(new char[]
					{
						'|'
					});
					if (array.Length >= 2)
					{
						string a = array[0];
						string text = array[1];
						if (a == "connect")
						{
							this.sessId = text;
							this.isConnected = true;
							this.DispatchEvent(new BBEvent(BBEvent.CONNECT));
							this.Poll(null);
						}
						else
						{
							if (a == "poll")
							{
								ByteArray value = null;
								if (text != "null")
								{
									value = this.DecodeResponse(text);
								}
								if (this.isConnected)
								{
									this.pollTimer = new Timer(new TimerCallback(this.Poll), null, this.pollSpeed, -1);
								}
								if (text != "null")
								{
									Hashtable hashtable2 = new Hashtable();
									hashtable2["data"] = value;
									this.DispatchEvent(new BBEvent(BBEvent.DATA, hashtable2));
								}
							}
							else
							{
								if (a == "err01")
								{
									Hashtable hashtable3 = new Hashtable();
									hashtable3["message"] = "Invalid http session !";
									this.HandleConnectionLost(false);
									this.DispatchEvent(new BBEvent(BBEvent.IO_ERROR, hashtable3));
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					Hashtable hashtable4 = new Hashtable();
					hashtable4["message"] = ex.Message + " " + ex.StackTrace;
					this.HandleConnectionLost(false);
					this.DispatchEvent(new BBEvent(BBEvent.IO_ERROR, hashtable4));
				}
			}
		}
		private void Poll(object state)
		{
			if (this.isConnected)
			{
				this.SendRequest("poll");
			}
		}
		private void SendRequest(string cmd)
		{
			this.SendRequest(cmd, null);
		}
		private void SendRequest(string cmd, object data)
		{
			string text = this.EncodeRequest(cmd, data);
			if (this.debug)
			{
				this.log.Debug(new string[]
				{
					"[ BB-Send ]: " + text
				});
			}
			SFSWebClient webClient = this.GetWebClient();
			Uri uri = new Uri(this.bbUrl);
			webClient.UploadValuesAsync(uri, "sfsHttp", Uri.EscapeDataString(text));
		}
		private SFSWebClient GetWebClient()
		{
			SFSWebClient sFSWebClient = new SFSWebClient();
			SFSWebClient expr_08 = sFSWebClient;
			expr_08.OnHttpResponse = (HttpResponseDelegate)Delegate.Combine(expr_08.OnHttpResponse, new HttpResponseDelegate(this.OnHttpResponse));
			return sFSWebClient;
		}
		private void HandleConnectionLost(bool fireEvent)
		{
			if (this.isConnected)
			{
				this.isConnected = false;
				this.sessId = null;
				this.pollTimer.Dispose();
				if (fireEvent)
				{
					this.DispatchEvent(new BBEvent(BBEvent.DISCONNECT));
				}
			}
		}
		private string EncodeRequest(string cmd)
		{
			return this.EncodeRequest(cmd, null);
		}
		private string EncodeRequest(string cmd, object data)
		{
			string text = "";
			if (cmd == null)
			{
				cmd = "null";
			}
			if (data == null)
			{
				text = "null";
			}
			else
			{
				if (data is ByteArray)
				{
					text = Convert.ToBase64String(((ByteArray)data).Bytes);
				}
			}
			return string.Concat(new string[]
			{
				(this.sessId != null) ? this.sessId : "null",
				Convert.ToString('|'),
				cmd,
				Convert.ToString('|'),
				text
			});
		}
		private ByteArray DecodeResponse(string rawData)
		{
			return new ByteArray(Convert.FromBase64String(rawData));
		}
		public void AddEventListener(string eventType, EventListenerDelegate listener)
		{
			this.dispatcher.AddEventListener(eventType, listener);
		}
		private void DispatchEvent(BaseEvent evt)
		{
			this.dispatcher.DispatchEvent(evt);
		}
	}
}
