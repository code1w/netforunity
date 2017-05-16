using System;
namespace KaiGeX.Bitswarm
{
	public enum PacketReadTransition
	{
		HeaderReceived,
		SizeReceived,
		IncompleteSize,
		WholeSizeReceived,
		PacketFinished,
		InvalidData,
		InvalidDataFinished
	}
}
