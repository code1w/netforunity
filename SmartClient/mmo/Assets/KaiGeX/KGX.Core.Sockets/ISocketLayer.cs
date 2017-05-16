using System;
using System.Net;
namespace KaiGeX.Core.Sockets
{
	public interface ISocketLayer
	{
		bool IsConnected
		{
			get;
		}
		bool RequiresConnection
		{
			get;
		}
		ConnectionDelegate OnConnect
		{
			get;
			set;
		}
        DisConnectDelegate OnDisconnect
		{
			get;
			set;
		}
		OnDataDelegate OnData
		{
			get;
			set;
		}
		OnErrorDelegate OnError
		{
			get;
			set;
		}
        OnDataProtoBufDelegate OnProtoBufData
        {
            get;
            set;
        }
		void Connect(IPAddress adr, int port);
		void Disconnect();
		void Disconnect(string reason);
		void Write(byte[] data);
		void Kill();
	}
}
