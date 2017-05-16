using System;
namespace KaiGeX.Entities.Variables
{
	public interface RoomVariable : UserVariable
	{
		bool IsPrivate
		{
			get;
			set;
		}
		bool IsPersistent
		{
			get;
			set;
		}
	}
}
