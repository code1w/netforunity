using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace KaiGeX.Entities.Managers
{
	public class SFSGlobalUserManager : SFSUserManager, IUserManager
	{
		private Dictionary<User, int> roomRefCount;
		public SFSGlobalUserManager(KaiGeNet sfs) : base(sfs)
		{
			this.roomRefCount = new Dictionary<User, int>();
		}
		public SFSGlobalUserManager(Room room) : base(room)
		{
			this.roomRefCount = new Dictionary<User, int>();
		}
		public override void AddUser(User user)
		{
			if (!this.roomRefCount.ContainsKey(user))
			{
				base.AddUser(user);
				this.roomRefCount[user] = 1;
			}
			else
			{
				//Dictionary<User, int> dictionary;
				//(dictionary = this.roomRefCount)[user] = dictionary[user] + 1;
			}
		}
		public override void RemoveUser(User user)
		{
			if (this.roomRefCount.ContainsKey(user))
			{
				if (this.roomRefCount[user] < 1)
				{
					base.LogWarn("GlobalUserManager RefCount is already at zero. User: " + user);
				}
				else
				{
					/*
					Dictionary<User, int> dictionary;
					(dictionary = this.roomRefCount)[user] = dictionary[user] - 1;
					if (this.roomRefCount[user] == 0)
					{
						base.RemoveUser(user);
						this.roomRefCount.Remove(user);
					}
					*/
				}
			}
			else
			{
				base.LogWarn("Can't remove User from GlobalUserManager. RefCount missing. User: " + user);
			}
		}
	}
}
