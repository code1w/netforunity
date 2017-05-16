using Sfs2X.Entities.Data;
using System;
namespace Sfs2X.Entities.Invitation
{
	public class SFSInvitation : Invitation
	{
		protected int id;
		protected User inviter;
		protected User invitee;
		protected int secondsForAnswer;
		protected ISFSObject parameters;
		public int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}
		public User Inviter
		{
			get
			{
				return this.inviter;
			}
		}
		public User Invitee
		{
			get
			{
				return this.invitee;
			}
		}
		public int SecondsForAnswer
		{
			get
			{
				return this.secondsForAnswer;
			}
		}
		public ISFSObject Params
		{
			get
			{
				return this.parameters;
			}
		}
		private void Init(User inviter, User invitee, int secondsForAnswer, ISFSObject parameters)
		{
			this.inviter = inviter;
			this.invitee = invitee;
			this.secondsForAnswer = secondsForAnswer;
			this.parameters = parameters;
		}
		public SFSInvitation(User inviter, User invitee)
		{
			this.Init(inviter, invitee, 15, null);
		}
		public SFSInvitation(User inviter, User invitee, int secondsForAnswer)
		{
			this.Init(inviter, invitee, secondsForAnswer, null);
		}
		public SFSInvitation(User inviter, User invitee, int secondsForAnswer, ISFSObject parameters)
		{
			this.Init(inviter, invitee, secondsForAnswer, parameters);
		}
	}
}
