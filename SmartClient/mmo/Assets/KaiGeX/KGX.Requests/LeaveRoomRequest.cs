using KaiGeX.Entities;
using KaiGeX.Exceptions;
using System;
namespace KaiGeX.Requests
{
	public class LeaveRoomRequest : BaseRequest
	{
		public static readonly string KEY_ROOM_ID = "r";
		private Room room;
		private void Init(Room room)
		{
			this.room = room;
		}
		public LeaveRoomRequest(Room room) : base(RequestType.LeaveRoom)
		{
			this.Init(room);
		}
		public LeaveRoomRequest() : base(RequestType.LeaveRoom)
		{
			this.Init(null);
		}
		public override void Validate(KaiGeNet sfs)
		{
			if (sfs.JoinedRooms.Count < 1)
			{
				throw new SFSValidationError("LeaveRoom request error", new string[]
				{
					"You are not joined in any rooms"
				});
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			if (this.room != null)
			{
				this.sfso.PutInt(LeaveRoomRequest.KEY_ROOM_ID, this.room.Id);
			}
		}
	}
}
