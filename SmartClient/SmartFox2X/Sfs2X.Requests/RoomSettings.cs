using Sfs2X.Entities;
using Sfs2X.Entities.Variables;
using System;
using System.Collections.Generic;
namespace Sfs2X.Requests
{
	public class RoomSettings
	{
		private string name;
		private string password;
		private string groupId;
		private bool isGame;
		private short maxUsers;
		private short maxSpectators;
		private short maxVariables;
		private List<RoomVariable> variables;
		private RoomPermissions permissions;
		private RoomEvents events;
		private RoomExtension extension;
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
		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
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
		public short MaxUsers
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
		public short MaxVariables
		{
			get
			{
				return this.maxVariables;
			}
			set
			{
				this.maxVariables = value;
			}
		}
		public short MaxSpectators
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
		public List<RoomVariable> Variables
		{
			get
			{
				return this.variables;
			}
			set
			{
				this.variables = value;
			}
		}
		public RoomPermissions Permissions
		{
			get
			{
				return this.permissions;
			}
			set
			{
				this.permissions = value;
			}
		}
		public RoomEvents Events
		{
			get
			{
				return this.events;
			}
			set
			{
				this.events = value;
			}
		}
		public RoomExtension Extension
		{
			get
			{
				return this.extension;
			}
			set
			{
				this.extension = value;
			}
		}
		public string GroupId
		{
			get
			{
				return this.groupId;
			}
			set
			{
				this.groupId = value;
			}
		}
		public RoomSettings(string name)
		{
			this.name = name;
			this.password = "";
			this.isGame = false;
			this.maxUsers = 10;
			this.maxSpectators = 0;
			this.maxVariables = 5;
			this.groupId = SFSConstants.DEFAULT_GROUP_ID;
			this.variables = new List<RoomVariable>();
		}
	}
}
