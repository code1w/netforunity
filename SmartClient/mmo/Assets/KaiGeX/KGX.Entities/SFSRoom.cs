using KaiGeX.Entities.Data;
using KaiGeX.Entities.Managers;
using KaiGeX.Entities.Variables;
using KaiGeX.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
namespace KaiGeX.Entities
{
	public class SFSRoom : Room
	{
		protected int id;
		protected string name;
		protected string groupId;
		protected bool isGame;
		protected bool isHidden;
		protected bool isJoined;
		protected bool isPasswordProtected;
		protected bool isManaged;
		protected Dictionary<string, RoomVariable> variables;
		protected Hashtable properties;
		protected IUserManager userManager;
		protected int maxUsers;
		protected int maxSpectators;
		protected int userCount;
		protected int specCount;
		protected IRoomManager roomManager;
		public int Id
		{
			get
			{
				return this.id;
			}
		}
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}
		public string GroupId
		{
			get
			{
				return this.groupId;
			}
		}
		public bool IsGame
		{
			get
			{
				return this.isGame;
			}
			set
			{
				this.isGame = value;
			}
		}
		public bool IsHidden
		{
			get
			{
				return this.isHidden;
			}
			set
			{
				this.isHidden = value;
			}
		}
		public bool IsJoined
		{
			get
			{
				return this.isJoined;
			}
			set
			{
				this.isJoined = value;
			}
		}
		public bool IsPasswordProtected
		{
			get
			{
				return this.isPasswordProtected;
			}
			set
			{
				this.isPasswordProtected = value;
			}
		}
		public bool IsManaged
		{
			get
			{
				return this.isManaged;
			}
			set
			{
				this.isManaged = value;
			}
		}
		public int MaxSpectators
		{
			get
			{
				return this.maxSpectators;
			}
			set
			{
				this.maxSpectators = value;
			}
		}
		public Hashtable Properties
		{
			get
			{
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}
		public int UserCount
		{
			get
			{
				int count;
				if (!this.isJoined)
				{
					count = this.userCount;
				}
				else
				{
					if (this.isGame)
					{
						count = this.PlayerList.Count;
					}
					else
					{
						count = this.userManager.UserCount;
					}
				}
				return count;
			}
			set
			{
				this.userCount = value;
			}
		}
		public int MaxUsers
		{
			get
			{
				return this.maxUsers;
			}
			set
			{
				this.maxUsers = value;
			}
		}
		public int Capacity
		{
			get
			{
				return this.maxUsers + this.maxSpectators;
			}
		}
		public int SpectatorCount
		{
			get
			{
				int result;
				if (!this.isGame)
				{
					result = 0;
				}
				else
				{
					if (this.isJoined)
					{
						result = this.SpectatorList.Count;
					}
					else
					{
						result = this.specCount;
					}
				}
				return result;
			}
			set
			{
				this.specCount = value;
			}
		}
		public List<User> UserList
		{
			get
			{
				return this.userManager.GetUserList();
			}
		}
		public List<User> PlayerList
		{
			get
			{
				List<User> list = new List<User>();
				foreach (User current in this.userManager.GetUserList())
				{
					if (current.IsPlayerInRoom(this))
					{
						list.Add(current);
					}
				}
				return list;
			}
		}
		public List<User> SpectatorList
		{
			get
			{
				List<User> list = new List<User>();
				foreach (User current in this.userManager.GetUserList())
				{
					if (current.IsSpectatorInRoom(this))
					{
						list.Add(current);
					}
				}
				return list;
			}
		}
		public IRoomManager RoomManager
		{
			get
			{
				return this.roomManager;
			}
			set
			{
				if (this.roomManager != null)
				{
					throw new SFSError("Room manager already assigned. Room: " + this);
				}
				this.roomManager = value;
			}
		}
		public static Room FromSFSArray(ISFSArray sfsa)
		{
			bool flag = sfsa.Size() == 14;
			Room room;
			if (flag)
			{
				room = new MMORoom(sfsa.GetInt(0), sfsa.GetUtfString(1), sfsa.GetUtfString(2));
			}
			else
			{
				room = new SFSRoom(sfsa.GetInt(0), sfsa.GetUtfString(1), sfsa.GetUtfString(2));
			}
			room.IsGame = sfsa.GetBool(3);
			room.IsHidden = sfsa.GetBool(4);
			room.IsPasswordProtected = sfsa.GetBool(5);
			room.UserCount = (int)sfsa.GetShort(6);
			room.MaxUsers = (int)sfsa.GetShort(7);
			ISFSArray sFSArray = sfsa.GetSFSArray(8);
			if (sFSArray.Size() > 0)
			{
				List<RoomVariable> list = new List<RoomVariable>();
				for (int i = 0; i < sFSArray.Size(); i++)
				{
					list.Add(SFSRoomVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
				}
				room.SetVariables(list);
			}
			if (room.IsGame)
			{
				room.SpectatorCount = (int)sfsa.GetShort(9);
				room.MaxSpectators = (int)sfsa.GetShort(10);
			}
			return room;
		}
		public SFSRoom(int id, string name)
		{
			this.Init(id, name, "default");
		}
		public SFSRoom(int id, string name, string groupId)
		{
			this.Init(id, name, groupId);
		}
		private void Init(int id, string name, string groupId)
		{
			this.id = id;
			this.name = name;
			this.groupId = groupId;
			this.isJoined = (this.isGame = (this.isHidden = false));
			this.isManaged = true;
			this.userCount = (this.specCount = 0);
			this.variables = new Dictionary<string, RoomVariable>();
			this.properties = new Hashtable();
			this.userManager = new SFSUserManager(this);
		}
		public List<RoomVariable> GetVariables()
		{
			return new List<RoomVariable>(this.variables.Values);
		}
		public RoomVariable GetVariable(string name)
		{
			RoomVariable result;
			if (!this.variables.ContainsKey(name))
			{
				result = null;
			}
			else
			{
				result = this.variables[name];
			}
			return result;
		}
		public User GetUserByName(string name)
		{
			return this.userManager.GetUserByName(name);
		}
		public User GetUserById(int id)
		{
			return this.userManager.GetUserById(id);
		}
		public void RemoveUser(User user)
		{
			this.userManager.RemoveUser(user);
		}
		public void SetVariable(RoomVariable roomVariable)
		{
			if (roomVariable.IsNull())
			{
				this.variables.Remove(roomVariable.Name);
			}
			else
			{
				this.variables[roomVariable.Name] = roomVariable;
			}
		}
		public void SetVariables(ICollection<RoomVariable> roomVariables)
		{
			foreach (RoomVariable current in roomVariables)
			{
				this.SetVariable(current);
			}
		}
		public bool ContainsVariable(string name)
		{
			return this.variables.ContainsKey(name);
		}
		private void RemoveUserVariable(string varName)
		{
			this.variables.Remove(varName);
		}
		public void AddUser(User user)
		{
			this.userManager.AddUser(user);
		}
		public bool ContainsUser(User user)
		{
			return this.userManager.ContainsUser(user);
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[Room: ",
				this.name,
				", Id: ",
				this.id,
				", GroupId: ",
				this.groupId,
				"]"
			});
		}
		public void Merge(Room anotherRoom)
		{
			this.variables.Clear();
			foreach (RoomVariable current in anotherRoom.GetVariables())
			{
				this.variables[current.Name] = current;
			}
			this.userManager.ClearAll();
			foreach (User current2 in anotherRoom.UserList)
			{
				this.userManager.AddUser(current2);
			}
		}
	}
}
