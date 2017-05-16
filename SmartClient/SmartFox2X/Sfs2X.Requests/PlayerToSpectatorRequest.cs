using Sfs2X.Entities;
using Sfs2X.Exceptions;
using System;
using System.Collections.Generic;
namespace Sfs2X.Requests
{
	public class PlayerToSpectatorRequest : BaseRequest
	{
		public static readonly string KEY_ROOM_ID = "r";
		public static readonly string KEY_USER_ID = "u";
		private Room room;
		private void Init(Room targetRoom)
		{
			this.room = targetRoom;
		}
		public PlayerToSpectatorRequest(Room targetRoom) : base(RequestType.PlayerToSpectator)
		{
			this.Init(targetRoom);
		}
		public PlayerToSpectatorRequest() : base(RequestType.PlayerToSpectator)
		{
			this.Init(null);
		}
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (sfs.JoinedRooms.Count < 1)
			{
				list.Add("You are not joined in any rooms");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("LeaveRoom request error", list);
			}
		}
		public override void Execute(SmartFox sfs)
		{
			if (this.room == null)
			{
				this.room = sfs.LastJoinedRoom;
			}
			this.sfso.PutInt(PlayerToSpectatorRequest.KEY_ROOM_ID, this.room.Id);
		}
	}
}
