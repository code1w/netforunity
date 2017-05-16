using KaiGeX.Entities;
using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests
{
	public class ChangeRoomCapacityRequest : BaseRequest
	{
		public static readonly string KEY_ROOM = "r";
		public static readonly string KEY_USER_SIZE = "u";
		public static readonly string KEY_SPEC_SIZE = "s";
		private Room room;
		private int newMaxUsers;
		private int newMaxSpect;
		public ChangeRoomCapacityRequest(Room room, int newMaxUsers, int newMaxSpect) : base(RequestType.ChangeRoomCapacity)
		{
			this.room = room;
			this.newMaxUsers = newMaxUsers;
			this.newMaxSpect = newMaxSpect;
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (this.room == null)
			{
				list.Add("Provided room is null");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ChangeRoomCapacity request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			this.sfso.PutInt(ChangeRoomCapacityRequest.KEY_ROOM, this.room.Id);
			this.sfso.PutInt(ChangeRoomCapacityRequest.KEY_USER_SIZE, this.newMaxUsers);
			this.sfso.PutInt(ChangeRoomCapacityRequest.KEY_SPEC_SIZE, this.newMaxSpect);
		}
	}
}
