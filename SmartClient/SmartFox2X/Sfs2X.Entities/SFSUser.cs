using Sfs2X.Entities.Data;
using Sfs2X.Entities.Managers;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;
using System;
using System.Collections.Generic;
namespace Sfs2X.Entities
{
	public class SFSUser : User
	{
		protected int id = -1;
		protected int privilegeId = 0;
		protected string name;
		protected bool isItMe;
		protected Dictionary<string, UserVariable> variables;
		protected Dictionary<string, object> properties;
		protected bool isModerator;
		protected Dictionary<int, int> playerIdByRoomId;
		protected IUserManager userManager;
		protected Vec3D aoiEntryPoint;
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
		}
		public int PlayerId
		{
			get
			{
				return this.GetPlayerId(this.userManager.SmartFoxClient.LastJoinedRoom);
			}
		}
		public int PrivilegeId
		{
			get
			{
				return this.privilegeId;
			}
			set
			{
				this.privilegeId = value;
			}
		}
		public bool IsPlayer
		{
			get
			{
				return this.PlayerId > 0;
			}
		}
		public bool IsSpectator
		{
			get
			{
				return !this.IsPlayer;
			}
		}
		public bool IsItMe
		{
			get
			{
				return this.isItMe;
			}
		}
		public IUserManager UserManager
		{
			get
			{
				return this.userManager;
			}
			set
			{
				if (this.userManager != null)
				{
					throw new SFSError("Cannot re-assign the User manager. Already set. User: " + this);
				}
				this.userManager = value;
			}
		}
		public Dictionary<string, object> Properties
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
		public Vec3D AOIEntryPoint
		{
			get
			{
				return this.aoiEntryPoint;
			}
			set
			{
				this.aoiEntryPoint = value;
			}
		}
		public static User FromSFSArray(ISFSArray sfsa, Room room)
		{
			User user = new SFSUser(sfsa.GetInt(0), sfsa.GetUtfString(1));
			user.PrivilegeId = (int)sfsa.GetShort(2);
			if (room != null)
			{
				user.SetPlayerId((int)sfsa.GetShort(3), room);
			}
			ISFSArray sFSArray = sfsa.GetSFSArray(4);
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				user.SetVariable(SFSUserVariable.FromSFSArray(sFSArray.GetSFSArray(i)));
			}
			return user;
		}
		public static User FromSFSArray(ISFSArray sfsa)
		{
			return SFSUser.FromSFSArray(sfsa, null);
		}
		public SFSUser(int id, string name)
		{
			this.Init(id, name, false);
		}
		public SFSUser(int id, string name, bool isItMe)
		{
			this.Init(id, name, isItMe);
		}
		private void Init(int id, string name, bool isItMe)
		{
			this.id = id;
			this.name = name;
			this.isItMe = isItMe;
			this.variables = new Dictionary<string, UserVariable>();
			this.properties = new Dictionary<string, object>();
			this.isModerator = false;
			this.playerIdByRoomId = new Dictionary<int, int>();
		}
		public bool IsJoinedInRoom(Room room)
		{
			return room.ContainsUser(this);
		}
		public bool IsGuest()
		{
			return this.privilegeId == 0;
		}
		public bool IsStandardUser()
		{
			return this.privilegeId == 1;
		}
		public bool IsModerator()
		{
			return this.privilegeId == 2;
		}
		public bool IsAdmin()
		{
			return this.privilegeId == 3;
		}
		public int GetPlayerId(Room room)
		{
			int result = 0;
			if (this.playerIdByRoomId.ContainsKey(room.Id))
			{
				result = this.playerIdByRoomId[room.Id];
			}
			return result;
		}
		public void SetPlayerId(int id, Room room)
		{
			this.playerIdByRoomId[room.Id] = id;
		}
		public void RemovePlayerId(Room room)
		{
			this.playerIdByRoomId.Remove(room.Id);
		}
		public bool IsPlayerInRoom(Room room)
		{
			return this.playerIdByRoomId[room.Id] > 0;
		}
		public bool IsSpectatorInRoom(Room room)
		{
			return this.playerIdByRoomId[room.Id] < 0;
		}
		public List<UserVariable> GetVariables()
		{
			return new List<UserVariable>(this.variables.Values);
		}
		public UserVariable GetVariable(string name)
		{
			UserVariable result;
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
		public void SetVariable(UserVariable userVariable)
		{
			if (userVariable != null)
			{
				if (userVariable.IsNull())
				{
					this.variables.Remove(userVariable.Name);
				}
				else
				{
					this.variables[userVariable.Name] = userVariable;
				}
			}
		}
		public void SetVariables(ICollection<UserVariable> userVariables)
		{
			foreach (UserVariable current in userVariables)
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
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[User: ",
				this.name,
				", Id: ",
				this.id,
				", isMe: ",
				this.isItMe,
				"]"
			});
		}
	}
}
