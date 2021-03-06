﻿using KaiGeX.Entities.Data;
using KaiGeX.Entities.Managers;
using KaiGeX.Entities.Variables;
using System;
using System.Collections.Generic;
namespace KaiGeX.Entities
{
	public interface User
	{
		int Id
		{
			get;
		}
		string Name
		{
			get;
		}
		int PlayerId
		{
			get;
		}
		bool IsPlayer
		{
			get;
		}
		bool IsSpectator
		{
			get;
		}
		int PrivilegeId
		{
			get;
			set;
		}
		IUserManager UserManager
		{
			get;
			set;
		}
		bool IsItMe
		{
			get;
		}
		Dictionary<string, object> Properties
		{
			get;
			set;
		}
		Vec3D AOIEntryPoint
		{
			get;
			set;
		}
		int GetPlayerId(Room room);
		void SetPlayerId(int id, Room room);
		void RemovePlayerId(Room room);
		bool IsGuest();
		bool IsStandardUser();
		bool IsModerator();
		bool IsAdmin();
		bool IsPlayerInRoom(Room room);
		bool IsSpectatorInRoom(Room room);
		bool IsJoinedInRoom(Room room);
		List<UserVariable> GetVariables();
		UserVariable GetVariable(string varName);
		void SetVariable(UserVariable userVariable);
		void SetVariables(ICollection<UserVariable> userVaribles);
		bool ContainsVariable(string name);
	}
}
