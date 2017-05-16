using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;
using System;
using System.Collections.Generic;
namespace Sfs2X.Requests.MMO
{
	public class SetUserPositionRequest : BaseRequest
	{
		public static readonly string KEY_ROOM = "r";
		public static readonly string KEY_VEC3D = "v";
		public static readonly string KEY_PLUS_USER_LIST = "p";
		public static readonly string KEY_MINUS_USER_LIST = "m";
		public static readonly string KEY_PLUS_ITEM_LIST = "q";
		public static readonly string KEY_MINUS_ITEM_LIST = "n";
		private Vec3D pos;
		private Room room;
		public SetUserPositionRequest(Vec3D position, Room room) : base(RequestType.SetUserPosition)
		{
			this.room = room;
			this.pos = position;
		}
		public SetUserPositionRequest(Vec3D position) : this(position, null)
		{
		}
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (this.pos == null)
			{
				list.Add("Position must be a valid Vec3D ");
			}
			if (this.room == null)
			{
				this.room = sfs.LastJoinedRoom;
			}
			if (this.room == null)
			{
				list.Add("You are not joined in any room");
			}
			if (!(this.room is MMORoom))
			{
				list.Add("Selected Room is not an MMORoom");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("SetUserVariables request error", list);
			}
		}
		public override void Execute(SmartFox sfs)
		{
			this.sfso.PutInt(SetUserPositionRequest.KEY_ROOM, this.room.Id);
			if (this.pos.IsFloat())
			{
				this.sfso.PutFloatArray(SetUserPositionRequest.KEY_VEC3D, this.pos.ToFloatArray());
			}
			else
			{
				this.sfso.PutIntArray(SetUserPositionRequest.KEY_VEC3D, this.pos.ToIntArray());
			}
		}
	}
}
