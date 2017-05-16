using KaiGeX.Util;
using System;
namespace KaiGeX.Bitswarm
{
	public interface IUDPManager
	{
		bool Inited
		{
			get;
		}
		long NextUdpPacketId
		{
			get;
		}
		void Initialize(string udpAddr, int udpPort);
		void Send(ByteArray binaryData);
		void Reset();
		void Disconnect();
		bool isConnected();
	}
}
