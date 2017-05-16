﻿using KaiGeX.Entities;
using KaiGeX.Entities.Data;
using System;
using System.Collections.Generic;
namespace KaiGeX.Requests
{
	public class ObjectMessageRequest : GenericMessageRequest
	{
		public ObjectMessageRequest(ISFSObject obj, Room targetRoom, ICollection<User> recipients)
		{
			this.type = 4;
			this.parameters = obj;
			this.room = targetRoom;
			this.recipient = recipients;
		}
		public ObjectMessageRequest(ISFSObject obj, Room targetRoom) : this(obj, targetRoom, null)
		{
		}
		public ObjectMessageRequest(ISFSObject obj) : this(obj, null, null)
		{
		}
	}
}
