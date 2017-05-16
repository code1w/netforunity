using Sfs2X.Entities;
using Sfs2X.Exceptions;
using System;
using System.Collections.Generic;
namespace Sfs2X.Requests
{
	public class SpectatorToPlayerRequest : BaseRequest
	{
		public static readonly string KEY_ROOM_ID = "r";
		public static readonly string KEY_USER_ID = "u";
		public static readonly string KEY_PLAYER_ID = "p";
		private Room room;
		private void Init(Room targetRoom)
		{
			this.room = targetRoom;
		}
		public SpectatorToPlayerRequest(Room targetRoom) : base(RequestType.SpectatorToPlayer)
		{
			this.Init(targetRoom);
		}
		public SpectatorToPlayerRequest() : base(RequestType.SpectatorToPlayer)
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
			this.sfso.PutInt(SpectatorToPlayerRequest.KEY_ROOM_ID, this.room.Id);
		}
	}
}
