using Sfs2X.Exceptions;
using Sfs2X.Logging;
using System;
namespace Sfs2X.Bitswarm
{
	public abstract class BaseController : IController
	{
		protected int id = -1;
		protected SmartFox sfs;
		protected BitSwarmClient bitSwarm;
		protected Logger log;
		public int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				if (this.id == -1)
				{
					this.id = value;
					return;
				}
				throw new SFSError("Controller ID is already set: " + this.id + ". Can't be changed at runtime!");
			}
		}
		public BaseController(BitSwarmClient bitSwarm)
		{
			this.bitSwarm = bitSwarm;
			if (bitSwarm != null)
			{
				this.log = bitSwarm.Log;
				this.sfs = bitSwarm.Sfs;
			}
		}
		public abstract void HandleMessage(IMessage message);
	}
}
