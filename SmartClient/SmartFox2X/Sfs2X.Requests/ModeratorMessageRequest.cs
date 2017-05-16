using Sfs2X.Entities.Data;
using System;
namespace Sfs2X.Requests
{
	public class ModeratorMessageRequest : GenericMessageRequest
	{
		public ModeratorMessageRequest(string message, MessageRecipientMode recipientMode, ISFSObject parameters)
		{
			if (recipientMode == null)
			{
				throw new ArgumentException("RecipientMode cannot be null!");
			}
			this.type = 2;
			this.message = message;
			this.parameters = parameters;
			this.recipient = recipientMode.Target;
			this.sendMode = recipientMode.Mode;
		}
		public ModeratorMessageRequest(string message, MessageRecipientMode recipientMode) : this(message, recipientMode, null)
		{
		}
	}
}
