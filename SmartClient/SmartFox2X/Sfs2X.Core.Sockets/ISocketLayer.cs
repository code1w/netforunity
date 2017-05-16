using System;
using System.Net;
namespace Sfs2X.Core.Sockets
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
		ConnectionDelegate OnDisconnect
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
		void Connect(IPAddress adr, int port);
		void Disconnect();
		void Disconnect(string reason);
		void Write(byte[] data);
		void Kill();
	}
}
