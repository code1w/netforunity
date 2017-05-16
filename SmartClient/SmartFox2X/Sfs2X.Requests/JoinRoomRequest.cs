using Sfs2X.Entities;
using Sfs2X.Exceptions;
using System;
namespace Sfs2X.Requests
{
	public class JoinRoomRequest : BaseRequest
	{
		public static readonly string KEY_ROOM = "r";
		public static readonly string KEY_USER_LIST = "ul";
		public static readonly string KEY_ROOM_NAME = "n";
		public static readonly string KEY_ROOM_ID = "i";
		public static readonly string KEY_PASS = "p";
		public static readonly string KEY_ROOM_TO_LEAVE = "rl";
		public static readonly string KEY_AS_SPECTATOR = "sp";
		private int id = -1;
		private string name;
		private string pass;
		private int? roomIdToLeave;
		private bool asSpectator;
		private void Init(object id, string pass, int? roomIdToLeave, bool asSpectator)
		{
			if (id is string)
			{
				this.name = (id as string);
			}
			else
			{
				if (id is int)
				{
					this.id = (int)id;
				}
				else
				{
					if (id is Room)
					{
						this.id = (id as Room).Id;
					}
				}
			}
			this.pass = pass;
			this.roomIdToLeave = roomIdToLeave;
			this.asSpectator = asSpectator;
		}
		public JoinRoomRequest(object id, string pass, int? roomIdToLeave, bool asSpectator) : base(RequestType.JoinRoom)
		{
			this.Init(id, pass, roomIdToLeave, asSpectator);
		}
		public JoinRoomRequest(object id, string pass, int? roomIdToLeave) : base(RequestType.JoinRoom)
		{
			this.Init(id, pass, roomIdToLeave, false);
		}
		public JoinRoomRequest(object id, string pass) : base(RequestType.JoinRoom)
		{
			this.Init(id, pass, null, false);
		}
		public JoinRoomRequest(object id) : base(RequestType.JoinRoom)
		{
			this.Init(id, null, null, false);
		}
		public override void Validate(SmartFox sfs)
		{
			if (this.id < 0 && this.name == null)
			{
				throw new SFSValidationError("JoinRoomRequest Error", new string[]
				{
					"Missing Room id or name, you should provide at least one"
				});
			}
		}
		public override void Execute(SmartFox sfs)
		{
			if (this.id > -1)
			{
				this.sfso.PutInt(JoinRoomRequest.KEY_ROOM_ID, this.id);
			}
			else
			{
				if (this.name != null)
				{
					this.sfso.PutUtfString(JoinRoomRequest.KEY_ROOM_NAME, this.name);
				}
			}
			if (this.pass != null)
			{
				this.sfso.PutUtfString(JoinRoomRequest.KEY_PASS, this.pass);
			}
			int? num = this.roomIdToLeave;
			if (num.HasValue)
			{
				this.sfso.PutInt(JoinRoomRequest.KEY_ROOM_TO_LEAVE, this.roomIdToLeave.Value);
			}
			if (this.asSpectator)
			{
				this.sfso.PutBool(JoinRoomRequest.KEY_AS_SPECTATOR, this.asSpectator);
			}
		}
	}
}
