using KaiGeX.Entities.Data;
using System;
namespace KaiGeX.Requests
{
	public class PrivateMessageRequest : GenericMessageRequest
	{
		public PrivateMessageRequest(string message, int recipientId, ISFSObject parameters)
		{
			this.type = 1;
			this.message = message;
			this.recipient = recipientId;
			this.parameters = parameters;
		}
		public PrivateMessageRequest(string message, int recipientId) : this(message, recipientId, null)
		{
		}
	}
}
