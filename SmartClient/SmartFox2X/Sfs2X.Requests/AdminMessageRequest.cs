using Sfs2X.Entities.Data;
using System;
namespace Sfs2X.Requests
{
	public class AdminMessageRequest : GenericMessageRequest
	{
		public AdminMessageRequest(string message, MessageRecipientMode recipientMode, ISFSObject parameters)
		{
			if (recipientMode == null)
			{
				throw new ArgumentException("RecipientMode cannot be null!");
			}
			this.type = 3;
			this.message = message;
			this.parameters = parameters;
			this.recipient = recipientMode.Target;
			this.sendMode = recipientMode.Mode;
		}
		public AdminMessageRequest(string message, MessageRecipientMode recipientMode) : this(message, recipientMode, null)
		{
		}
	}
}
