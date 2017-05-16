using KaiGeX.Entities;
using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests
{
	public class ChangeRoomPasswordStateRequest : BaseRequest
	{
		public static readonly string KEY_ROOM = "r";
		public static readonly string KEY_PASS = "p";
		private Room room;
		private string newPass;
		public ChangeRoomPasswordStateRequest(Room room, string newPass) : base(RequestType.ChangeRoomPassword)
		{
			this.room = room;
			this.newPass = newPass;
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (this.room == null)
			{
				list.Add("Provided room is null");
			}
			if (this.newPass == null)
			{
				list.Add("Invalid new room password. It must be a non-null string.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ChangePassState request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			this.sfso.PutInt(ChangeRoomPasswordStateRequest.KEY_ROOM, this.room.Id);
			this.sfso.PutUtfString(ChangeRoomPasswordStateRequest.KEY_PASS, this.newPass);
		}
	}
}
