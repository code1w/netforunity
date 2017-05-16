using KaiGeX.Bitswarm;
using System;
namespace KaiGeX.Controllers
{
	public delegate void RequestDelegate(IMessage msg);
    public delegate void RequestProtoBufDelegate(ProtoBuf.IExtensible msg);

}
