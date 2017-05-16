using KaiGeX.Entities;
using KaiGeX.Entities.Data;
using System;
namespace KaiGeX.Requests.Buddylist
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
