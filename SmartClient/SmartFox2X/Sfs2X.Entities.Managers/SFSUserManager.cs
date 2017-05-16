using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Sfs2X.Entities.Managers
{
	public class SFSUserManager : IUserManager
	{
		private Dictionary<string, User> usersByName;
		private Dictionary<int, User> usersById;
		protected Room room;
		protected SmartFox sfs;
		public int UserCount
		{
			get
			{
				return this.usersById.Count;
			}
		}
		public SmartFox SmartFoxClient
		{
			get
			{
				return this.sfs;
			}
		}
		protected void LogWarn(string msg)
		{
			if (this.sfs != null)
			{
				this.sfs.Log.Warn(new string[]
				{
					msg
				});
			}
			else
			{
				if (this.room != null && this.room.RoomManager != null)
				{
					this.room.RoomManager.SmartFoxClient.Log.Warn(new string[]
					{
						msg
					});
				}
			}
		}
		public SFSUserManager(SmartFox sfs)
		{
			this.sfs = sfs;
			this.usersByName = new Dictionary<string, User>();
			this.usersById = new Dictionary<int, User>();
		}
		public SFSUserManager(Room room)
		{
			this.room = room;
			this.usersByName = new Dictionary<string, User>();
			this.usersById = new Dictionary<int, User>();
		}
		public bool ContainsUserName(string userName)
		{
			return this.usersByName.ContainsKey(userName);
		}
		public bool ContainsUserId(int userId)
		{
			return this.usersById.ContainsKey(userId);
		}
		public bool ContainsUser(User user)
		{
			return this.usersByName.ContainsValue(user);
		}
		public User GetUserByName(string userName)
		{
			User result;
			if (!this.usersByName.ContainsKey(userName))
			{
				result = null;
			}
			else
			{
				result = this.usersByName[userName];
			}
			return result;
		}
		public User GetUserById(int userId)
		{
			User result;
			if (!this.usersById.ContainsKey(userId))
			{
				result = null;
			}
			else
			{
				result = this.usersById[userId];
			}
			return result;
		}
		public virtual void AddUser(User user)
		{
			if (this.usersById.ContainsKey(user.Id))
			{
				this.LogWarn("Unexpected: duplicate user in UserManager: " + user);
			}
			this.AddUserInternal(user);
		}
		protected void AddUserInternal(User user)
		{
			this.usersByName[user.Name] = user;
			this.usersById[user.Id] = user;
		}
		public virtual void RemoveUser(User user)
		{
			this.LogWarn("---------------------- USER REMOVED: " + user + " ------------------------------------");
			StackTrace stackTrace = new StackTrace();
			this.LogWarn(stackTrace.ToString());
			this.usersByName.Remove(user.Name);
			this.usersById.Remove(user.Id);
		}
		public void RemoveUserById(int id)
		{
			if (this.usersById.ContainsKey(id))
			{
				User user = this.usersById[id];
				this.RemoveUser(user);
			}
		}
		public List<User> GetUserList()
		{
			return new List<User>(this.usersById.Values);
		}
		public void ClearAll()
		{
			this.usersByName = new Dictionary<string, User>();
			this.usersById = new Dictionary<int, User>();
		}
	}
}
