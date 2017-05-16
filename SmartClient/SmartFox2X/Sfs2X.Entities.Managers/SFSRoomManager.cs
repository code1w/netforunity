using System;
using System.Collections.Generic;
namespace Sfs2X.Entities.Managers
{
	public class SFSRoomManager : IRoomManager
	{
		private string ownerZone;
		private List<string> groups;
		private Dictionary<int, Room> roomsById;
		private Dictionary<string, Room> roomsByName;
		protected SmartFox smartFox;
		public string OwnerZone
		{
			get
			{
				return this.ownerZone;
			}
			set
			{
				this.ownerZone = value;
			}
		}
		public SmartFox SmartFoxClient
		{
			get
			{
				return this.smartFox;
			}
		}
		public SFSRoomManager(SmartFox sfs)
		{
			this.smartFox = sfs;
			this.groups = new List<string>();
			this.roomsById = new Dictionary<int, Room>();
			this.roomsByName = new Dictionary<string, Room>();
		}
		public void AddRoom(Room room)
		{
			this.AddRoom(room, true);
		}
		public void AddRoom(Room room, bool addGroupIfMissing)
		{
			this.roomsById[room.Id] = room;
			this.roomsByName[room.Name] = room;
			if (addGroupIfMissing)
			{
				if (!this.ContainsGroup(room.GroupId))
				{
					this.AddGroup(room.GroupId);
				}
			}
			else
			{
				room.IsManaged = false;
			}
		}
		public Room ReplaceRoom(Room room)
		{
			return this.ReplaceRoom(room, true);
		}
		public Room ReplaceRoom(Room room, bool addToGroupIfMissing)
		{
			Room roomById = this.GetRoomById(room.Id);
			Room result;
			if (roomById != null)
			{
				roomById.Merge(room);
				result = roomById;
			}
			else
			{
				this.AddRoom(room, addToGroupIfMissing);
				result = room;
			}
			return result;
		}
		public void ChangeRoomName(Room room, string newName)
		{
			string name = room.Name;
			room.Name = newName;
			this.roomsByName[newName] = room;
			this.roomsByName.Remove(name);
		}
		public void ChangeRoomPasswordState(Room room, bool isPassProtected)
		{
			room.IsPasswordProtected = isPassProtected;
		}
		public void ChangeRoomCapacity(Room room, int maxUsers, int maxSpect)
		{
			room.MaxUsers = maxUsers;
			room.MaxSpectators = maxSpect;
		}
		public List<string> GetRoomGroups()
		{
			return this.groups;
		}
		public void AddGroup(string groupId)
		{
			this.groups.Add(groupId);
		}
		public void RemoveGroup(string groupId)
		{
			this.groups.Remove(groupId);
			List<Room> roomListFromGroup = this.GetRoomListFromGroup(groupId);
			foreach (Room current in roomListFromGroup)
			{
				if (!current.IsJoined)
				{
					this.RemoveRoom(current);
				}
				else
				{
					current.IsManaged = false;
				}
			}
		}
		public bool ContainsGroup(string groupId)
		{
			return this.groups.Contains(groupId);
		}
		public bool ContainsRoom(object idOrName)
		{
			bool result;
			if (idOrName is int)
			{
				result = this.roomsById.ContainsKey((int)idOrName);
			}
			else
			{
				result = this.roomsByName.ContainsKey((string)idOrName);
			}
			return result;
		}
		public bool ContainsRoomInGroup(object idOrName, string groupId)
		{
			List<Room> roomListFromGroup = this.GetRoomListFromGroup(groupId);
			bool flag = idOrName is int;
			bool result;
			foreach (Room current in roomListFromGroup)
			{
				if (flag)
				{
					if (current.Id == (int)idOrName)
					{
						result = true;
						return result;
					}
				}
				else
				{
					if (current.Name == (string)idOrName)
					{
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}
		public Room GetRoomById(int id)
		{
			Room result;
			if (!this.roomsById.ContainsKey(id))
			{
				result = null;
			}
			else
			{
				result = this.roomsById[id];
			}
			return result;
		}
		public Room GetRoomByName(string name)
		{
			Room result;
			if (!this.roomsByName.ContainsKey(name))
			{
				result = null;
			}
			else
			{
				result = this.roomsByName[name];
			}
			return result;
		}
		public List<Room> GetRoomList()
		{
			return new List<Room>(this.roomsById.Values);
		}
		public int GetRoomCount()
		{
			return this.roomsById.Count;
		}
		public List<Room> GetRoomListFromGroup(string groupId)
		{
			List<Room> list = new List<Room>();
			foreach (Room current in this.roomsById.Values)
			{
				if (current.GroupId == groupId)
				{
					list.Add(current);
				}
			}
			return list;
		}
		public void RemoveRoom(Room room)
		{
			this.RemoveRoom(room.Id, room.Name);
		}
		public void RemoveRoomById(int id)
		{
			if (this.roomsById.ContainsKey(id))
			{
				Room room = this.roomsById[id];
				this.RemoveRoom(id, room.Name);
			}
		}
		public void RemoveRoomByName(string name)
		{
			if (this.roomsByName.ContainsKey(name))
			{
				Room room = this.roomsByName[name];
				this.RemoveRoom(room.Id, name);
			}
		}
		public List<Room> GetJoinedRooms()
		{
			List<Room> list = new List<Room>();
			foreach (Room current in this.roomsById.Values)
			{
				if (current.IsJoined)
				{
					list.Add(current);
				}
			}
			return list;
		}
		public List<Room> GetUserRooms(User user)
		{
			List<Room> list = new List<Room>();
			foreach (Room current in this.roomsById.Values)
			{
				if (current.ContainsUser(user))
				{
					list.Add(current);
				}
			}
			return list;
		}
		public void RemoveUser(User user)
		{
			foreach (Room current in this.roomsById.Values)
			{
				if (current.ContainsUser(user))
				{
					current.RemoveUser(user);
				}
			}
		}
		private void RemoveRoom(int id, string name)
		{
			this.roomsById.Remove(id);
			this.roomsByName.Remove(name);
		}
	}
}
