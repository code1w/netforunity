using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using System;
namespace Sfs2X.Requests
{
	public class PublicMessageRequest : GenericMessageRequest
	{
		public PublicMessageRequest(string message, ISFSObject parameters, Room targetRoom)
		{
			this.type = 0;
			this.message = message;
			this.room = targetRoom;
			this.parameters = parameters;
		}
		public PublicMessageRequest(string message, ISFSObject parameters) : this(message, parameters, null)
		{
		}
		public PublicMessageRequest(string message) : this(message, null, null)
		{
		}
	}
}
