using System;
namespace KaiGeX.Entities.Variables
{
	public interface BuddyVariable : UserVariable
	{
		bool IsOffline
		{
			get;
		}
	}
}
