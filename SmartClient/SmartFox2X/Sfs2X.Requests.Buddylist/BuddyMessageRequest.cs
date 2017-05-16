using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using System;
namespace Sfs2X.Requests.Buddylist
{
	public class BuddyMessageRequest : GenericMessageRequest
	{
		public BuddyMessageRequest(string message, Buddy targetBuddy, ISFSObject parameters)
		{
			this.type = 5;
			this.message = message;
			this.recipient = ((targetBuddy == null) ? -1 : targetBuddy.Id);
			this.parameters = parameters;
		}
		public BuddyMessageRequest(string message, Buddy targetBuddy) : this(message, targetBuddy, null)
		{
		}
	}
}
