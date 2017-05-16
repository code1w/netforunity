using System;
namespace Sfs2X.Util
{
	public class ConfigData
	{
		private string host = "127.0.0.1";
		private int port = 9933;
		private string udpHost = "127.0.0.1";
		private int udpPort = 9933;
		private string zone;
		private bool debug = false;
		private int httpPort = 8080;
		private bool useBlueBox = true;
		private int blueBoxPollingRate = 750;
		public string Host
		{
			get
			{
				return this.host;
			}
			set
			{
				this.host = value;
			}
		}
		public int Port
		{
			get
			{
				return this.port;
			}
			set
			{
				this.port = value;
			}
		}
		public string UdpHost
		{
			get
			{
				return this.udpHost;
			}
			set
			{
				this.udpHost = value;
			}
		}
		public int UdpPort
		{
			get
			{
				return this.udpPort;
			}
			set
			{
				this.udpPort = value;
			}
		}
		public string Zone
		{
			get
			{
				return this.zone;
			}
			set
			{
				this.zone = value;
			}
		}
		public bool Debug
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
		public int HttpPort
		{
			get
			{
				return this.httpPort;
			}
			set
			{
				this.httpPort = value;
			}
		}
		public bool UseBlueBox
		{
			get
			{
				return this.useBlueBox;
			}
			set
			{
				this.useBlueBox = value;
			}
		}
		public int BlueBoxPollingRate
		{
			get
			{
				return this.blueBoxPollingRate;
			}
			set
			{
				this.blueBoxPollingRate = value;
			}
		}
	}
}
