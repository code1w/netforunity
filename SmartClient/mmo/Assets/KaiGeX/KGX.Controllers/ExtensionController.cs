using KaiGeX.Bitswarm;
using KaiGeX.Core;
using KaiGeX.Entities.Data;
using System;
using System.Collections;
namespace KaiGeX.Controllers
{
	public class ExtensionController : BaseController
	{
		public static readonly string KEY_CMD = "c";
		public static readonly string KEY_PARAMS = "p";
		public static readonly string KEY_ROOM = "r";
		public ExtensionController(BitSwarmClient bitSwarm) : base(bitSwarm)
		{
		}
		public override void HandleMessage(IMessage message)
		{
			if (this.sfs.Debug)
			{
				this.log.Info(new string[]
				{
					message.ToString()
				});
			}
			ISFSObject content = message.Content;
			Hashtable hashtable = new Hashtable();
			hashtable["cmd"] = content.GetUtfString(ExtensionController.KEY_CMD);
			hashtable["params"] = content.GetSFSObject(ExtensionController.KEY_PARAMS);
			if (content.ContainsKey(ExtensionController.KEY_ROOM))
			{
				hashtable["sourceRoom"] = content.GetInt(ExtensionController.KEY_ROOM);
			}
			if (message.IsUDP)
			{
				hashtable["packetId"] = message.PacketId;
			}
			this.sfs.DispatchEvent(new SFSEvent(SFSEvent.EXTENSION_RESPONSE, hashtable));
		}
        
	}
}
