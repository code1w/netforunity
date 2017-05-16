using Sfs2X.Entities;
using Sfs2X.Exceptions;
using System;
using System.Collections.Generic;
namespace Sfs2X.Requests
{
	public class ChangeRoomNameRequest : BaseRequest
	{
		public static readonly string KEY_ROOM = "r";
		public static readonly string KEY_NAME = "n";
		private Room room;
		private string newName;
		public ChangeRoomNameRequest(Room room, string newName) : base(RequestType.ChangeRoomName)
		{
			this.room = room;
			this.newName = newName;
		}
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (this.room == null)
			{
				list.Add("Provided room is null");
			}
			if (this.newName == null || this.newName.Length == 0)
			{
				list.Add("Invalid new room name. It must be a non-null and non-empty string.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ChangeRoomName request error", list);
			}
		}
		public override void Execute(SmartFox sfs)
		{
			this.sfso.PutInt(ChangeRoomNameRequest.KEY_ROOM, this.room.Id);
			this.sfso.PutUtfString(ChangeRoomNameRequest.KEY_NAME, this.newName);
		}
	}
}
