using Sfs2X.Entities.Data;
using System;
namespace Sfs2X.Entities.Variables
{
	public class SFSRoomVariable : SFSUserVariable, RoomVariable, UserVariable
	{
		private bool isPersistent;
		private bool isPrivate;
		public bool IsPrivate
		{
			get
			{
				return this.isPrivate;
			}
			set
			{
				this.isPrivate = value;
			}
		}
		public bool IsPersistent
		{
			get
			{
				return this.isPersistent;
			}
			set
			{
				this.isPersistent = value;
			}
		}
		public new static RoomVariable FromSFSArray(ISFSArray sfsa)
		{
			return new SFSRoomVariable(sfsa.GetUtfString(0), sfsa.GetElementAt(2), (int)sfsa.GetByte(1))
			{
				IsPrivate = sfsa.GetBool(3),
				IsPersistent = sfsa.GetBool(4)
			};
		}
		public SFSRoomVariable(string name, object val, int type) : base(name, val, type)
		{
		}
		public SFSRoomVariable(string name, object val) : base(name, val, -1)
		{
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[RVar: ",
				this.name,
				", type: ",
				this.type,
				", value: ",
				this.val,
				", isPriv: ",
				this.isPrivate,
				"]"
			});
		}
		public override ISFSArray ToSFSArray()
		{
			ISFSArray iSFSArray = base.ToSFSArray();
			iSFSArray.AddBool(this.isPrivate);
			iSFSArray.AddBool(this.isPersistent);
			return iSFSArray;
		}
	}
}
