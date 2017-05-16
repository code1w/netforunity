using KaiGeX.Entities;
using KaiGeX.Entities.Data;
using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests.Game
{
	public class InviteUsersRequest : BaseRequest
	{
		public static readonly string KEY_USER = "u";
		public static readonly string KEY_USER_ID = "ui";
		public static readonly string KEY_INVITATION_ID = "ii";
		public static readonly string KEY_TIME = "t";
		public static readonly string KEY_PARAMS = "p";
		public static readonly string KEY_INVITEE_ID = "ee";
		public static readonly string KEY_INVITED_USERS = "iu";
		public static readonly string KEY_REPLY_ID = "ri";
		public static readonly int MAX_INVITATIONS_FROM_CLIENT_SIDE = 8;
		public static readonly int MIN_EXPIRY_TIME = 5;
		public static readonly int MAX_EXPIRY_TIME = 300;
		private List<object> invitedUsers;
		private int secondsForAnswer;
		private ISFSObject parameters;
		public InviteUsersRequest(List<object> invitedUsers, int secondsForReply, ISFSObject parameters) : base(RequestType.InviteUser)
		{
			this.invitedUsers = invitedUsers;
			this.secondsForAnswer = secondsForReply;
			this.parameters = parameters;
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (this.invitedUsers == null || this.invitedUsers.Count < 1)
			{
				list.Add("No invitation(s) to send");
			}
			if (this.invitedUsers.Count > InviteUsersRequest.MAX_INVITATIONS_FROM_CLIENT_SIDE)
			{
				list.Add("Too many invitations. Max allowed from client side is: " + InviteUsersRequest.MAX_INVITATIONS_FROM_CLIENT_SIDE);
			}
			if (this.secondsForAnswer < 5 || this.secondsForAnswer > 300)
			{
				list.Add(string.Concat(new object[]
				{
					"SecondsForAnswer value is out of range (",
					InviteUsersRequest.MIN_EXPIRY_TIME,
					"-",
					InviteUsersRequest.MAX_EXPIRY_TIME,
					")"
				}));
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("InvitationReply request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			List<int> list = new List<int>();
			foreach (object current in this.invitedUsers)
			{
				if (current is User)
				{
					if (current as User != sfs.MySelf)
					{
						list.Add((current as User).Id);
					}
				}
				else
				{
					if (current is Buddy)
					{
						list.Add((current as Buddy).Id);
					}
				}
			}
			this.sfso.PutIntArray(InviteUsersRequest.KEY_INVITED_USERS, list.ToArray());
			this.sfso.PutShort(InviteUsersRequest.KEY_TIME, (short)this.secondsForAnswer);
			if (this.parameters != null)
			{
				this.sfso.PutSFSObject(InviteUsersRequest.KEY_PARAMS, this.parameters);
			}
		}
	}
}
