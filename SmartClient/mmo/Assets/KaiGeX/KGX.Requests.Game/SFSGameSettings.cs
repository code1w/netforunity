using KaiGeX.Entities.Data;
using KaiGeX.Entities.Match;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests.Game
{
	public class SFSGameSettings : RoomSettings
	{
		private bool isPublic;
		private int minPlayersToStartGame;
		private List<object> invitedPlayers;
		private List<string> searchableRooms;
		private MatchExpression playerMatchExpression;
		private MatchExpression spectatorMatchExpression;
		private int invitationExpiryTime;
		private bool leaveJoinedLastRoom;
		private bool notifyGameStarted;
		private ISFSObject invitationParams;
		public bool IsPublic
		{
			get
			{
				return this.isPublic;
			}
			set
			{
				this.isPublic = value;
			}
		}
		public int MinPlayersToStartGame
		{
			get
			{
				return this.minPlayersToStartGame;
			}
			set
			{
				this.minPlayersToStartGame = value;
			}
		}
		public List<object> InvitedPlayers
		{
			get
			{
				return this.invitedPlayers;
			}
			set
			{
				this.invitedPlayers = value;
			}
		}
		public List<string> SearchableRooms
		{
			get
			{
				return this.searchableRooms;
			}
			set
			{
				this.searchableRooms = value;
			}
		}
		public int InvitationExpiryTime
		{
			get
			{
				return this.invitationExpiryTime;
			}
			set
			{
				this.invitationExpiryTime = value;
			}
		}
		public bool LeaveLastJoinedRoom
		{
			get
			{
				return this.leaveJoinedLastRoom;
			}
			set
			{
				this.leaveJoinedLastRoom = value;
			}
		}
		public bool NotifyGameStarted
		{
			get
			{
				return this.notifyGameStarted;
			}
			set
			{
				this.notifyGameStarted = value;
			}
		}
		public MatchExpression PlayerMatchExpression
		{
			get
			{
				return this.playerMatchExpression;
			}
			set
			{
				this.playerMatchExpression = value;
			}
		}
		public MatchExpression SpectatorMatchExpression
		{
			get
			{
				return this.spectatorMatchExpression;
			}
			set
			{
				this.spectatorMatchExpression = value;
			}
		}
		public ISFSObject InvitationParams
		{
			get
			{
				return this.invitationParams;
			}
			set
			{
				this.invitationParams = value;
			}
		}
		public SFSGameSettings(string name) : base(name)
		{
			this.isPublic = true;
			this.minPlayersToStartGame = 2;
			this.invitationExpiryTime = 15;
			this.leaveJoinedLastRoom = true;
			this.invitedPlayers = new List<object>();
			this.searchableRooms = new List<string>();
		}
	}
}
