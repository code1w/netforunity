using System;
namespace KaiGeX.Bitswarm
{
	public interface IController
	{
		int Id
		{
			get;
			set;
		}
		void HandleMessage(IMessage message);
        void HandleMessage(ProtoBuf.IExtensible message);
	}
}
