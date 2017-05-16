using Sfs2X.Entities;
using Sfs2X.Exceptions;
using System;
using System.Collections.Generic;
namespace Sfs2X.Requests.Game
{
	public class CreateSFSGameRequest : BaseRequest
	{
		public static readonly string KEY_IS_PUBLIC = "gip";
		public static readonly string KEY_MIN_PLAYERS = "gmp";
		public static readonly string KEY_INVITED_PLAYERS = "ginp";
		public static readonly string KEY_SEARCHABLE_ROOMS = "gsr";
		public static readonly string KEY_PLAYER_MATCH_EXP = "gpme";
		public static readonly string KEY_SPECTATOR_MATCH_EXP = "gsme";
		public static readonly string KEY_INVITATION_EXPIRY = "gie";
		public static readonly string KEY_LEAVE_ROOM = "glr";
		public static readonly string KEY_NOTIFY_GAME_STARTED = "gns";
		public static readonly string KEY_INVITATION_PARAMS = "ip";
		private CreateRoomRequest createRoomRequest;
		private SFSGameSettings settings;
		public CreateSFSGameRequest(SFSGameSettings settings) : base(RequestType.CreateSFSGame)
		{
			this.settings = settings;
			this.createRoomRequest = new CreateRoomRequest(settings, false, null);
		}
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			try
			{
				this.createRoomRequest.Validate(sfs);
			}
			catch (SFSValidationError sFSValidationError)
			{
				list = sFSValidationError.Errors;
			}
			if (this.settings.MinPlayersToStartGame > (int)this.settings.MaxUsers)
			{
				list.Add("minPlayersToStartGame cannot be greater than maxUsers");
			}
			if (this.settings.InvitationExpiryTime < InviteUsersRequest.MIN_EXPIRY_TIME || this.settings.InvitationExpiryTime > InviteUsersRequest.MAX_EXPIRY_TIME)
			{
				list.Add(string.Concat(new object[]
				{
					"Expiry time value is out of range (",
					InviteUsersRequest.MIN_EXPIRY_TIME,
					"-",
					InviteUsersRequest.MAX_EXPIRY_TIME,
					")"
				}));
			}
			if (this.settings.InvitedPlayers != null && this.settings.InvitedPlayers.Count > InviteUsersRequest.MAX_INVITATIONS_FROM_CLIENT_SIDE)
			{
				list.Add("Cannot invite more than " + InviteUsersRequest.MAX_INVITATIONS_FROM_CLIENT_SIDE + " players from client side");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("CreateSFSGame request error", list);
			}
		}
		public override void Execute(SmartFox sfs)
		{
			this.createRoomRequest.Execute(sfs);
			this.sfso = this.createRoomRequest.Message.Content;
			this.sfso.PutBool(CreateSFSGameRequest.KEY_IS_PUBLIC, this.settings.IsPublic);
			this.sfso.PutShort(CreateSFSGameRequest.KEY_MIN_PLAYERS, (short)this.settings.MinPlayersToStartGame);
			this.sfso.PutShort(CreateSFSGameRequest.KEY_INVITATION_EXPIRY, (short)this.settings.InvitationExpiryTime);
			this.sfso.PutBool(CreateSFSGameRequest.KEY_LEAVE_ROOM, this.settings.LeaveLastJoinedRoom);
			this.sfso.PutBool(CreateSFSGameRequest.KEY_NOTIFY_GAME_STARTED, this.settings.NotifyGameStarted);
			if (this.settings.PlayerMatchExpression != null)
			{
				this.sfso.PutSFSArray(CreateSFSGameRequest.KEY_PLAYER_MATCH_EXP, this.settings.PlayerMatchExpression.ToSFSArray());
			}
			if (this.settings.SpectatorMatchExpression != null)
			{
				this.sfso.PutSFSArray(CreateSFSGameRequest.KEY_SPECTATOR_MATCH_EXP, this.settings.SpectatorMatchExpression.ToSFSArray());
			}
			if (this.settings.InvitedPlayers != null)
			{
				List<int> list = new List<int>();
				foreach (object current in this.settings.InvitedPlayers)
				{
					if (current is User)
					{
						list.Add((current as User).Id);
					}
					else
					{
						if (current is Buddy)
						{
							list.Add((current as Buddy).Id);
						}
					}
				}
				this.sfso.PutIntArray(CreateSFSGameRequest.KEY_INVITED_PLAYERS, list.ToArray());
			}
			if (this.settings.SearchableRooms != null)
			{
				this.sfso.PutUtfStringArray(CreateSFSGameRequest.KEY_SEARCHABLE_ROOMS, this.settings.SearchableRooms.ToArray());
			}
			if (this.settings.InvitationParams != null)
			{
				this.sfso.PutSFSObject(CreateSFSGameRequest.KEY_INVITATION_PARAMS, this.settings.InvitationParams);
			}
		}
	}
}
