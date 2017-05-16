using KaiGeX.Entities.Data;
using KaiGeX.Entities.Invitation;
using KaiGeX.Exceptions;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests.Game
{
	public class InvitationReplyRequest : BaseRequest
	{
		public static readonly string KEY_INVITATION_ID = "i";
		public static readonly string KEY_INVITATION_REPLY = "r";
		public static readonly string KEY_INVITATION_PARAMS = "p";
		private Invitation invitation;
		private InvitationReply reply;
		private ISFSObject parameters;
		private void Init(Invitation invitation, InvitationReply reply, ISFSObject parameters)
		{
			this.invitation = invitation;
			this.reply = reply;
			this.parameters = parameters;
		}
		public InvitationReplyRequest(Invitation invitation, InvitationReply reply, ISFSObject parameters) : base(RequestType.InvitationReply)
		{
			this.Init(invitation, reply, parameters);
		}
		public InvitationReplyRequest(Invitation invitation, InvitationReply reply) : base(RequestType.InvitationReply)
		{
			this.Init(invitation, reply, null);
		}
		public override void Validate(KaiGeNet sfs)
		{
			List<string> list = new List<string>();
			if (this.invitation == null)
			{
				list.Add("Missing invitation object");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("InvitationReply request error", list);
			}
		}
		public override void Execute(KaiGeNet sfs)
		{
			this.sfso.PutInt(InvitationReplyRequest.KEY_INVITATION_ID, this.invitation.Id);
			this.sfso.PutByte(InvitationReplyRequest.KEY_INVITATION_REPLY, (byte)this.reply);
			if (this.parameters != null)
			{
				this.sfso.PutSFSObject(InvitationReplyRequest.KEY_INVITATION_PARAMS, this.parameters);
			}
		}
	}
}
