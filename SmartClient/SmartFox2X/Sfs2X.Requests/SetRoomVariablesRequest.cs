using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;
using System;
using System.Collections.Generic;
namespace Sfs2X.Requests
{
	public class SetRoomVariablesRequest : BaseRequest
	{
		public static readonly string KEY_VAR_ROOM = "r";
		public static readonly string KEY_VAR_LIST = "vl";
		private ICollection<RoomVariable> roomVariables;
		private Room room;
		private void Init(ICollection<RoomVariable> roomVariables, Room room)
		{
			this.roomVariables = roomVariables;
			this.room = room;
		}
		public SetRoomVariablesRequest(ICollection<RoomVariable> roomVariables, Room room) : base(RequestType.SetRoomVariables)
		{
			this.Init(roomVariables, room);
		}
		public SetRoomVariablesRequest(ICollection<RoomVariable> roomVariables) : base(RequestType.SetRoomVariables)
		{
			this.Init(roomVariables, null);
		}
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (this.room != null)
			{
				if (!this.room.ContainsUser(sfs.MySelf))
				{
					list.Add("You are not joined in the target room");
				}
			}
			else
			{
				if (sfs.LastJoinedRoom == null)
				{
					list.Add("You are not joined in any rooms");
				}
			}
			if (this.roomVariables == null || this.roomVariables.Count == 0)
			{
				list.Add("No variables were specified");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("SetRoomVariables request error", list);
			}
		}
		public override void Execute(SmartFox sfs)
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			foreach (RoomVariable current in this.roomVariables)
			{
				iSFSArray.AddSFSArray(current.ToSFSArray());
			}
			if (this.room == null)
			{
				this.room = sfs.LastJoinedRoom;
			}
			this.sfso.PutSFSArray(SetRoomVariablesRequest.KEY_VAR_LIST, iSFSArray);
			this.sfso.PutInt(SetRoomVariablesRequest.KEY_VAR_ROOM, this.room.Id);
		}
	}
}
